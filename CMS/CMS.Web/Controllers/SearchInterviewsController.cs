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

namespace CMS.Web.Controllers
{
    public class SearchInterviewsController : Controller
    {


        private readonly ICandidateService _candidateService;
        private readonly IPositionService _positionService;
        private readonly IStatusService _StatusService;
        private readonly INotificationsService _notificationsService;
        private readonly ISearchInterviewsService _searchInterviewsService;
        private readonly string _attachmentStoragePath;

        public SearchInterviewsController( ICandidateService candidateService, IPositionService positionService, IStatusService statusService, IWebHostEnvironment env,
            INotificationsService notificationsService, ISearchInterviewsService searchInterviewsService )
        {
            _candidateService = candidateService;
            _positionService = positionService;
            _StatusService = statusService;
            _notificationsService = notificationsService;
            _searchInterviewsService = searchInterviewsService;
            _attachmentStoragePath = Path.Combine(env.WebRootPath, "attachments");

            if (!Directory.Exists(_attachmentStoragePath))
            {
                Directory.CreateDirectory(_attachmentStoragePath);
            }
        }




        public async Task<ActionResult> Index(string positionFilter, int? scoreFilter, int? statusFilter, int? candidateFilter, string interviewerFilter, DateTime? fromDate, DateTime? toDate)
        {
            var positionsDTO = await _positionService.GetAll();
            ViewBag.PositionList = new SelectList(positionsDTO.Value, "PositionId", "Name");

            // Get all statuses
            var statusesResult = await _StatusService.GetAll();
            if (!statusesResult.IsSuccess)
            {
                ModelState.AddModelError("", statusesResult.Error);
                return View(new List<InterviewsDTO>()); // Return an empty list if there was an error
            }

            var statuses = statusesResult.Value;
            //statuses.Insert(0, new StatusDTO { Id = 0, Name = "All Statuses" });

            ViewBag.StatusList = new SelectList(statuses, "Id", "Name");

            // Get all candidates
            var candidatesDTO = await _candidateService.GetAllCandidatesAsync();
            ViewBag.CandidateList = new SelectList(candidatesDTO, "CandidateId", "FullName");

            // Get all interviewers
            var interviewersDTO = await _searchInterviewsService.GetInterviewers();
            ViewBag.InterviewerList = new SelectList(interviewersDTO, "Id", "Name");

            // Get all interviews
            var interviewsResult = await _searchInterviewsService.GetAll();

            if (interviewsResult.IsSuccess)
            {
                var interviews = interviewsResult.Value;

                int positionId = Convert.ToInt32(positionFilter);
                int statusId = Convert.ToInt32(statusFilter);
                int candidateId = Convert.ToInt32(candidateFilter);

                // If a position filter is selected, filter the interviews
                if (!string.IsNullOrEmpty(positionFilter) && positionFilter != "All Positions")
                {
                    interviews = interviews
                        .Where(i => i.positionId == positionId)
                        .ToList(); // Materialize the filtered interviews
                }

                // Filter by score if the scoreFilter parameter is provided
                if (scoreFilter.HasValue)
                {
                    interviews = interviews
                        .Where(i => i.Score == scoreFilter.Value)
                        .ToList(); // Materialize the filtered interviews
                }

                // Filter by status if the statusFilter parameter is provided
                if (statusFilter.HasValue && statusFilter.Value > 0)
                {
                    interviews = interviews
                        .Where(i => i.StatusId == statusFilter.Value)
                        .ToList(); // Materialize the filtered interviews
                }

                // Filter by candidate if the candidateFilter parameter is provided
                if (candidateFilter.HasValue && candidateFilter.Value > 0)
                {
                    interviews = interviews
                        .Where(i => i.candidateId == candidateFilter.Value)
                        .ToList(); // Materialize the filtered interviews
                }

                // Filter by interviewer if the interviewerFilter parameter is provided
                if (!string.IsNullOrEmpty(interviewerFilter) && interviewerFilter != "All Interviewers")
                {
                    interviews = interviews
                        .Where(i => i.InterviewerId == interviewerFilter)
                        .ToList(); // Materialize the filtered interviews
                }
                if (fromDate.HasValue && toDate.HasValue)
                {
                    interviews = interviews
                        .Where(i => i.Date >= fromDate.Value && i.Date <= toDate.Value)
                        .ToList(); // Materialize the filtered interviews
                }

                if (Request.Query["export"].ToString() == "excel")
                {
                    // Export data to Excel
                    var filteredData = interviews; // Use the filtered data
                    var excelData = GenerateExcelFile(filteredData);

                    return File(excelData, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Interviews.xlsx");
                }

                return View(interviews);
            }
            else
            {
                ModelState.AddModelError("", interviewsResult.Error);
                return View(new List<InterviewsDTO>()); // Return an empty list if there was an error
            }
        }


        private byte[] GenerateExcelFile(IEnumerable<InterviewsDTO> data)
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


        public async Task<ActionResult> Details(int id)
        {
            var result = await _searchInterviewsService.GetById(id);

            var positionsDTO = await _positionService.GetAll();
            ViewBag.positionDTOs = new SelectList(positionsDTO.Value, "PositionId", "Name");

            var candidateDTOs = await _candidateService.GetAllCandidatesAsync();
            ViewBag.candidateDTOs = new SelectList(candidateDTOs, "CandidateId", "FullName");

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






        
        public async Task<ActionResult> Edit(int id)
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
            ViewBag.positionDTOs = new SelectList(positionDTOs.Value, "PositionId", "Name");

            var candidateDTOs = await _candidateService.GetAllCandidatesAsync();
            ViewBag.candidateDTOs = new SelectList(candidateDTOs, "CandidateId", "FullName");

            var StatusDTOs = await _StatusService.GetAll();
            ViewBag.StatusDTOs = new SelectList(StatusDTOs.Value, "Id", "Name");

            var interviewersDTOs = await _searchInterviewsService.GetInterviewers();
            ViewBag.interviewersDTOs = new SelectList(interviewersDTOs, "Id", "Name");

            return View(interviewDTO);
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(int id, InterviewsDTO collection)
        {
            if (collection == null)
            {
                ModelState.AddModelError("", $"the interview dto you are trying to update is null ");
                return RedirectToAction("Index");
            }

            var positionDTOs = await _positionService.GetAll();
            ViewBag.positionDTOs = new SelectList(positionDTOs.Value, "PositionId", "Name");

            var candidateDTOs = await _candidateService.GetAllCandidatesAsync();
            ViewBag.candidateDTOs = new SelectList(candidateDTOs, "CandidateId", "FullName");

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




        public async Task<ActionResult> Delete(int id)
        {
            var result = await _searchInterviewsService.GetById(id);
            if (result.IsSuccess)
            {
                var interviewDTO = result.Value;
                return View(interviewDTO);
            }


            else
            {
                ModelState.AddModelError("", result.Error);
                return View();
            }
        }

        // POST: InterviewsController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(int id, InterviewsDTO collection)
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



        //[HttpPost]
        //public async Task<IActionResult> FilterByDateRange(DateTime fromDate, DateTime toDate)
        //{
        //    try
        //    {
        //        var interviews = await _searchInterviewsService.GetByDateRange(fromDate, toDate);

        //        // Return the filtered data to your view or client-side code
        //        return View(interviews); // Use appropriate view name and model
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        //    }
        //}



     




    }
}
