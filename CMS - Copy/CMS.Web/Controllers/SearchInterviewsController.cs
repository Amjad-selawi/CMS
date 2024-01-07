using CMS.Application.DTOs;
using CMS.Domain.Entities;
using CMS.Services.Interfaces;
using CMS.Services.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using OfficeOpenXml.Style;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using System.Security.Claims;
using CMS.Domain.Migrations;

namespace CMS.Web.Controllers
{
    public class SearchInterviewsController : Controller
    {


        private readonly ICandidateService _candidateService;
        private readonly IPositionService _positionService;
        private readonly IStatusService _StatusService;
        private readonly INotificationsService _notificationsService;
        private readonly ISearchInterviewsService _searchInterviewsService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly string _attachmentStoragePath;

        public SearchInterviewsController(ICandidateService candidateService, IPositionService positionService, IStatusService statusService, IWebHostEnvironment env,
            INotificationsService notificationsService, 
            ISearchInterviewsService searchInterviewsService, IHttpContextAccessor httpContextAccessor)
        {
            _candidateService = candidateService;
            _positionService = positionService;
            _StatusService = statusService;
            _notificationsService = notificationsService;
            _searchInterviewsService = searchInterviewsService;
            _httpContextAccessor = httpContextAccessor;
            _attachmentStoragePath = Path.Combine(env.WebRootPath, "attachments");

            if (!Directory.Exists(_attachmentStoragePath))
            {
                Directory.CreateDirectory(_attachmentStoragePath);
            }
        }

        public void LogException(string methodName, Exception ex, string additionalInfo = null)
        {
            
            _positionService.LogException(methodName, ex, additionalInfo);
        }


        public async Task<ActionResult> Index(string positionFilter, int? scoreFilter, int? statusFilter, string candidateFilter, string interviewerFilter, DateTime? fromDate, DateTime? toDate, string export)
        {
            try
            {

            
            ViewBag.positionFilter = positionFilter;
            ViewBag.scoreFilter = scoreFilter;
            ViewBag.statusFilter = statusFilter;
            ViewBag.candidateFilter = candidateFilter;
            ViewBag.interviewerFilter = interviewerFilter;
            ViewBag.fromDate = fromDate;
            ViewBag.toDate = toDate;

            if (User.IsInRole("Admin") || User.IsInRole("HR Manager") || User.IsInRole("General Manager"))
            {
                var positionsDTO = await _positionService.GetAll();
                ViewBag.PositionList = new SelectList(positionsDTO.Value, "Id", "Name");

                var statusesResult = await _StatusService.GetAll();
                if (!statusesResult.IsSuccess)
                {
                    ModelState.AddModelError("", statusesResult.Error);
                    return View(new List<InterviewsDTO>()); // Return an empty list if there was an error
                }

                var statuses = statusesResult.Value;
                ViewBag.StatusList = new SelectList(statuses, "Id", "Name");

                // Get all candidates
                var candidatesDTO = await _candidateService.GetAllCandidatesAsync();
                ViewBag.CandidateList = new SelectList(candidatesDTO, "Id", "FullName");

                // Get all interviewers
                var interviewersDTO = await _searchInterviewsService.GetAllInterviewers();
                ViewBag.InterviewerList = new SelectList(interviewersDTO, "Id", "Name");


                if (!string.IsNullOrEmpty(export) && export == "excel")
                {
                    var filteredInterviews = await ApplyFiltersAndRetrieveData(positionFilter, scoreFilter, statusFilter, candidateFilter, interviewerFilter, fromDate, toDate);
                    var excelData = GenerateExcelFile(filteredInterviews);
                    return File(excelData, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Interviews.xlsx");
                }
                else
                {
                    var filteredInterviews = await ApplyFiltersAndRetrieveData(positionFilter, scoreFilter, statusFilter, candidateFilter, interviewerFilter, fromDate, toDate);
                    return View(filteredInterviews);
                }
            }
            else
            {
                return View("AccessDenied");
            }
            }
            catch (Exception ex)
            {
                LogException(nameof(Index), ex, "Index page for SearchInterviews filter not working");
                throw ex;
            }
        }

        private async Task<IEnumerable<InterviewsDTO>> ApplyFiltersAndRetrieveData(string positionFilter, int? scoreFilter, int? statusFilter, string candidateFilter, string interviewerFilter, DateTime? fromDate, DateTime? toDate)
        {
            try
            {

            
            var interviewsResult = await _searchInterviewsService.GetAll();

            if (!interviewsResult.IsSuccess)
            {
                ModelState.AddModelError("", interviewsResult.Error);
                return new List<InterviewsDTO>();
            }

            var interviews = interviewsResult.Value;

            // Apply your filters to the data
            int positionId = Convert.ToInt32(positionFilter);
            int statusId = Convert.ToInt32(statusFilter);

            var filteredInterviews = interviews
                .GroupBy(i => i.CandidateId)
                .Select(group => group.OrderByDescending(i => i.InterviewsId).FirstOrDefault())
                .ToList();

            // If a position filter is selected, filter the interviews
            if (!string.IsNullOrEmpty(positionFilter) && positionFilter != "All Positions")
            {
                filteredInterviews = filteredInterviews
                    .Where(i => i.PositionId == positionId)
                    .ToList();
            }

            // Filter by score if the scoreFilter parameter is provided
            if (scoreFilter.HasValue)
            {
                filteredInterviews = filteredInterviews
                    .Where(i => i.Score == scoreFilter.Value)
                    .ToList();
            }

            // Filter by status if the statusFilter parameter is provided
            if (statusFilter.HasValue && statusFilter.Value > 0)
            {
                filteredInterviews = filteredInterviews
                    .Where(i => i.StatusId == statusFilter.Value)
                    .ToList();
            }

            // Filter by candidate if the candidateFilter parameter is provided
            if (!string.IsNullOrEmpty(candidateFilter))
            {
                filteredInterviews = filteredInterviews
                    .Where(i => i.FullName.Contains(candidateFilter, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            // Filter by interviewer if the interviewerFilter parameter is provided
            if (!string.IsNullOrEmpty(interviewerFilter) && interviewerFilter != "All Interviewers")
            {
                filteredInterviews = filteredInterviews
                    .Where(i => i.InterviewerId == interviewerFilter)
                    .ToList();
            }

            // Apply date filtering
            if (fromDate.HasValue && toDate.HasValue)
            {
                toDate = toDate.Value.AddDays(1);

                filteredInterviews = filteredInterviews
                    .Where(i => i.Date >= fromDate.Value && i.Date <= toDate.Value)
                    .OrderBy(i => i.Date)
                    .ToList();
            }

            // Fetch CV attachment ID from corresponding Candidate
            foreach (var interview in filteredInterviews)
            {
                var candidateCVAttachmentId = await _candidateService.GetCVAttachmentIdByCandidateId(interview.CandidateId);
                interview.CandidateCVAttachmentId = candidateCVAttachmentId;
            }

            return filteredInterviews;
            }
            catch (Exception ex)
            {
                LogException(nameof(ApplyFiltersAndRetrieveData), ex, "Faild to apply filter");
                throw ex;
            }
        }

        public async Task<ActionResult> ShowHistory(int id)
        {
            try
            {


                var result = await _searchInterviewsService.ShowHistory(id);

                if (result.IsSuccess)
                {
                    var interviewsDTOs = result.Value;
                    return View(interviewsDTOs);
                }
                else
                {
                    ModelState.AddModelError("", result.Error);
                    return View();
                }
            }
            catch (Exception ex)
            {
                LogException(nameof(ShowHistory), ex, "Faild to load show history page");
                throw ex;
            }
        }

        public async Task<ActionResult> ExportFilteredData(string positionFilter, int? scoreFilter, int? statusFilter, string candidateFilter, string interviewerFilter, DateTime? fromDate, DateTime? toDate)
        {
            try
            {

           
            var filteredInterviews = await ApplyFiltersAndRetrieveData(positionFilter, scoreFilter, statusFilter, candidateFilter, interviewerFilter, fromDate, toDate);
            var excelData = GenerateExcelFile(filteredInterviews);
            return File(excelData, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "FilteredInterviews.xlsx");
            }
            catch (Exception ex)
            {
                LogException(nameof(ExportFilteredData), ex, "Faild to Export Filtered Data");
                throw ex;
            }
        }



        private byte[] GenerateExcelFile(IEnumerable<InterviewsDTO> data)
        {
            try
            {

          
            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Interviews");

                // Add headers
                worksheet.Cells["A1"].Value = "Candidate Name";
                worksheet.Cells["B1"].Value = "Position";
                worksheet.Cells["C1"].Value = "Interviewer Name";
                worksheet.Cells["D1"].Value = "Date";
                worksheet.Cells["E1"].Value = "Score";
                worksheet.Cells["F1"].Value = "Status";
                worksheet.Cells["G1"].Value = "Notes";

                // Style headers
                using (var headerRange = worksheet.Cells["A1:G1"])
                {
                    headerRange.Style.Font.Bold = true;
                    headerRange.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    headerRange.Style.Fill.BackgroundColor.SetColor(Color.LightGray);
                    headerRange.Style.Font.Color.SetColor(Color.Black);
                }

                // Fill data rows
                int row = 2;
                foreach (var item in data)
                {
                    worksheet.Cells[row, 1].Value = item.FullName;
                    worksheet.Cells[row, 2].Value = item.Name;
                    worksheet.Cells[row, 3].Value = item.InterviewerName;
                    worksheet.Cells[row, 4].Style.Numberformat.Format = "yyyy-mm-dd";
                    worksheet.Cells[row, 4].Value = item.Date;
                    worksheet.Cells[row, 5].Value = item.Score;
                    worksheet.Cells[row, 6].Value = item.StatusName;
                    worksheet.Cells[row, 7].Value = item.Notes;

                    row++;
                }

                return package.GetAsByteArray();
            }
            }
            catch (Exception ex)
            {
                LogException(nameof(GenerateExcelFile), ex, "Faild to Generate Excel File");
                throw ex;
            }
        }


        public async Task<ActionResult> Details(int id)
        {
            try
            {
                


                var result = await _searchInterviewsService.GetById(id);

            var positionsDTO = await _positionService.GetAll();
            ViewBag.positionDTOs = new SelectList(positionsDTO.Value, "Id", "Name");

            var candidateDTOs = await _candidateService.GetAllCandidatesAsync();
            ViewBag.candidateDTOs = new SelectList(candidateDTOs, "Id", "FullName");

            //var interviewersDTOs = await _searchInterviewsService.GetInterviewers();
            //ViewBag.interviewersDTOs = new SelectList(interviewersDTOs, "Id", "Name");


            if (result.IsSuccess)
            {
                var interviewsDTO = result.Value;

                interviewsDTO.InterviewerName = await _searchInterviewsService.GetInterviewerName(interviewsDTO.InterviewerId);


                return View(interviewsDTO);
            }


            else
            {
                ModelState.AddModelError("", result.Error);
                return View();
            }
            }
            catch (Exception ex)
            {
                LogException(nameof(Details), ex, $"Faild to load {id} details");
                throw ex;
            }
        }







        public async Task<ActionResult> Edit(int id)
        {
            try
            {
                


                if (id <= 0)
            {
                return NotFound();
            }
            var result = await _searchInterviewsService.GetById(id);
            var interviewDTO = result.Value;
            if (interviewDTO == null)
            {
                return NotFound();
            }
            var positionDTOs = await _positionService.GetAll();
            ViewBag.positionDTOs = new SelectList(positionDTOs.Value, "Id", "Name");

            var candidateDTOs = await _candidateService.GetAllCandidatesAsync();
            ViewBag.candidateDTOs = new SelectList(candidateDTOs, "Id", "FullName");

            var StatusDTOs = await _StatusService.GetAll();
            ViewBag.StatusDTOs = new SelectList(StatusDTOs.Value, "Id", "Name");

            var interviewersDTOs = await _searchInterviewsService.GetInterviewers();
            ViewBag.interviewersDTOs = new SelectList(interviewersDTOs, "Id", "Name");

            return View(interviewDTO);
            }
            catch (Exception ex)
            {
                LogException(nameof(Edit), ex, $"Faild to load ID: {id} edit page");
                throw ex;
            }
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(int id, InterviewsDTO collection)
        {
            try
            {
                


                if (collection == null)
            {
                ModelState.AddModelError("", $"the interview dto you are trying to update is null ");
                return RedirectToAction("Index");
            }

            var positionDTOs = await _positionService.GetAll();
            ViewBag.positionDTOs = new SelectList(positionDTOs.Value, "Id", "Name");

            var candidateDTOs = await _candidateService.GetAllCandidatesAsync();
            ViewBag.candidateDTOs = new SelectList(candidateDTOs, "Id", "FullName");

            var StatusDTOs = await _StatusService.GetAll();
            ViewBag.StatusDTOs = new SelectList(StatusDTOs.Value, "Id", "Name");

            var interviewersDTOs = await _searchInterviewsService.GetInterviewers();
            ViewBag.interviewersDTOs = new SelectList(interviewersDTOs, "Id", "Name");


            if (ModelState.IsValid)
            {
                var result = await _searchInterviewsService.Update(collection);

                if (result.IsSuccess)
                {
                    return RedirectToAction("Index");
                }

                ModelState.AddModelError("", result.Error);
                return View(collection);
            }
            else
            {
                ModelState.AddModelError("", $"the model state is not valid");
            }
            return View(collection);
            }
            catch (Exception ex)
            {
                LogException(nameof(Edit), ex, $"Faild to edit ID: {id}");
                throw ex;
            }
        }




        public async Task<ActionResult> Delete(int id)
        {
            try
            {

                

                var result = await _searchInterviewsService.GetById(id);
            if (result.IsSuccess)
            {
                var interviewDTO = result.Value;

                interviewDTO.InterviewerName = await _searchInterviewsService.GetInterviewerName(interviewDTO.InterviewerId);

                return View(interviewDTO);
            }


            else
            {
                ModelState.AddModelError("", result.Error);
                return View();
            }
            }
            catch (Exception ex)
            {
                LogException(nameof(Delete), ex, $"Faild to load ID: {id} delete page" );
                throw ex;
            }
        }

        // POST: InterviewsController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(int id, InterviewsDTO collection)
        {
            try
            {

                

                if (id <= 0)
            {
                return BadRequest("invalid career offer id");
            }
            var result = await _searchInterviewsService.Delete(id);
            if (result.IsSuccess)
            {
                return RedirectToAction("Index");
            }
            ModelState.AddModelError("", result.Error);
            return View();
            }
            catch (Exception ex)
            {
                LogException(nameof(Delete), ex, $"Faild to delete ID: {id}");
                throw ex;
            }
        }









    }
}
