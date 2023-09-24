using Microsoft.AspNetCore;
using CMS.Application.DTOs;
using CMS.Domain.Entities;
using CMS.Domain.Enums;
using CMS.Services.Interfaces;
using CMS.Services.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.VisualBasic;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using CMS.Web.Utils;
using System.Collections.ObjectModel;

namespace CMS.Web.Controllers
{

    public class InterviewsController : Controller
    {
        private readonly IInterviewsService _interviewsService;
        private readonly ICandidateService _candidateService;
        private readonly IPositionService _positionService;
        private readonly IStatusService _StatusService;
        private readonly INotificationsService _notificationsService;
        private readonly string _attachmentStoragePath;

        public InterviewsController(IInterviewsService interviewsService, ICandidateService candidateService, IPositionService positionService, IStatusService statusService, IWebHostEnvironment env,
            INotificationsService notificationsService)
        {
            _interviewsService = interviewsService;
            _candidateService = candidateService;
            _positionService = positionService;
            _StatusService = statusService;
            _notificationsService = notificationsService;
            _attachmentStoragePath = Path.Combine(env.WebRootPath, "attachments");

            if (!Directory.Exists(_attachmentStoragePath))
            {
                Directory.CreateDirectory(_attachmentStoragePath);
            }
        }

        public async Task<ActionResult> MyInterviews()
        {
            var result = await _interviewsService.MyInterviews();
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

        // GET: InterviewsController
        public async Task<ActionResult> Index()
        {
            var result = await _interviewsService.GetAll();
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

        // GET: InterviewsController/Details/5
        public async Task<ActionResult> Details(int id)
        {
            var result = await _interviewsService.GetById(id);

            var positionsDTO = await _positionService.GetAll();
            ViewBag.positionDTOs = new SelectList(positionsDTO.Value, "PositionId", "Name");

            var candidateDTOs = await _candidateService.GetAllCandidatesAsync();
            ViewBag.candidateDTOs = new SelectList(candidateDTOs, "CandidateId", "FullName");

            if (result.IsSuccess)
            {
                var interviewsDTO = result.Value;
                return View(interviewsDTO);
            }


            else
            {
                ModelState.AddModelError("", result.Error);
                return View();
            }
        }

        //[Authorize(Roles = "General Manager")]
        // GET: InterviewsController/Create
        public async Task<ActionResult> Create()
        {
            var positionDTOs = await _positionService.GetAll();
            ViewBag.positionDTOs = new SelectList(positionDTOs.Value, "PositionId", "Name");

            var candidateDTOs = await _candidateService.GetAllCandidatesAsync();
            ViewBag.candidateDTOs = new SelectList(candidateDTOs, "CandidateId", "FullName");

            var StatusDTOs = await _StatusService.GetAll();
            ViewBag.StatusDTOs = new SelectList(StatusDTOs.Value, "Id", "Name");

            var interviewersDTOs = await _interviewsService.GetInterviewers();
            ViewBag.interviewersDTOs = new SelectList(interviewersDTOs, "Id", "Name");



            return View();
        }

        // POST: InterviewsController/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(InterviewsDTO collection)
        {
            var positionDTOs = await _positionService.GetAll();
            ViewBag.positionDTOs = new SelectList(positionDTOs.Value, "PositionId", "Name");

            var candidateDTOs = await _candidateService.GetAllCandidatesAsync();
            ViewBag.candidateDTOs = new SelectList(candidateDTOs, "CandidateId", "FullName");

            var StatusDTOs = await _StatusService.GetAll();
            ViewBag.StatusDTOs = new SelectList(StatusDTOs.Value, "Id", "Name");

            var interviewersDTOs = await _interviewsService.GetInterviewers();
            ViewBag.interviewersDTOs = new SelectList(interviewersDTOs, "Id", "Name");


            if (ModelState.IsValid)
            {
                var result = await _interviewsService.Insert(collection);


                if (result.IsSuccess)
                {
                    if (User.IsInRole("HR"))
                    {
                        await _notificationsService.CreateInterviewNotificationForInterviewerAsync(collection.Date);
                        return RedirectToAction("Index");
                    }
                    else if (User.IsInRole("Interviewer"))
                    {
                        if (collection.StatusName == "Approved" || collection.StatusName == "Rejected")
                        {
                            await _notificationsService.CreateNotificationForGeneralManagerAsync(collection.StatusId, collection.Notes);
                        }

                        return RedirectToAction("Index");
                    }
                    else
                    {
                        return RedirectToAction("Index");
                    }


                }


                ModelState.AddModelError("", result.Error);
            }
            else
            {
                ModelState.AddModelError("", "error validating the model");
            }

            return View(collection);


        }

        // GET: InterviewsController/Edit/5
        public async Task<ActionResult> Edit(int id)
        {
            if (id <= 0)
            {
                return NotFound();
            }
            var result = await _interviewsService.GetById(id);
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

            var interviewersDTOs = await _interviewsService.GetInterviewers();
            ViewBag.interviewersDTOs = new SelectList(interviewersDTOs, "Id", "Name");

            return View(interviewDTO);
        }

        // POST: InterviewsController/Edit/5
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

            var interviewersDTOs = await _interviewsService.GetInterviewers();
            ViewBag.interviewersDTOs = new SelectList(interviewersDTOs, "Id", "Name");


            if (ModelState.IsValid)
            {
                var result = await _interviewsService.Update(collection);

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


        // GET: InterviewsController/Delete/5
        public async Task<ActionResult> Delete(int id)
        {
            var result = await _interviewsService.GetById(id);
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
            var result = await _interviewsService.Delete(id);
            if (result.IsSuccess)
            {
                return RedirectToAction("Index");
            }
            ModelState.AddModelError("", result.Error);
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateAttachment(int id, IFormFile file)
        {


            if (file == null || file.Length == 0)
            {
                ModelState.AddModelError("File", "Please choose a file to upload.");
                return View();
            }
            if (ModelState.IsValid)
            {
                var stream = await AttachmentHelper.handleUpload(file, _attachmentStoragePath);
                await _interviewsService.UpdateInterviewAttachmentAsync(id, file.FileName, file.Length, stream);
                return RedirectToAction(nameof(Index));
            }
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> UpdateAfterInterview(int id)
        {
            var StatusDTOs = await _StatusService.GetAll();
            ViewBag.StatusDTOs = new SelectList(StatusDTOs.Value, "Id", "Name");

            var result = await _interviewsService.GetById(id);
            var InterviewsDTO = result.Value;
           
            return View(InterviewsDTO);
        }
        [HttpPost]
        public async Task<IActionResult> UpdateAfterInterview(InterviewsDTO interviewsDTO, IFormFile file)
        {
            var StatusDTOs = await _StatusService.GetAll();
            ViewBag.StatusDTOs = new SelectList(StatusDTOs.Value, "Id", "Name");

            if (file == null || file.Length == 0)
            {
                ModelState.AddModelError("File", "Please choose a file to upload.");
                return View();
            }
            if (interviewsDTO.Notes == null)
            {
                ModelState.AddModelError("", "Please Add Notes.");
                return View();
            }
            if (interviewsDTO.Score == null)
            {
                ModelState.AddModelError("", "Please Add Score.");
                return View();
            }
            if (interviewsDTO.StatusId == null)
            {
                ModelState.AddModelError("", "Please Add Status.");
                return View();
            }
            FileStream attachmentStream = await AttachmentHelper.handleUpload(file, _attachmentStoragePath);
            interviewsDTO.FileName = file.FileName;
            interviewsDTO.FileSize = file.Length;
            interviewsDTO.FileData = attachmentStream;

            if (ModelState.IsValid)
            {

         


                await _interviewsService.ConductInterview(interviewsDTO);

                attachmentStream.Close();

                if (User.IsInRole("Interviewer"))
                {
                    if (interviewsDTO.StatusId == 7 || interviewsDTO.StatusId == 8)
                    {
                        await _notificationsService.CreateNotificationForGeneralManagerAsync(interviewsDTO.StatusId, interviewsDTO.Notes);
                        return RedirectToAction(nameof(MyInterviews));
                    }
                }

                else if (User.IsInRole("General Manger"))
                {
                    if (interviewsDTO.StatusId == 7 || interviewsDTO.StatusId == 8)
                    {
                        await _notificationsService.CreateInterviewNotificationForHRInterview(interviewsDTO.StatusId, interviewsDTO.Notes);
                        return RedirectToAction(nameof(MyInterviews));
                    }

                }
                else
                {
                    return RedirectToAction(nameof(MyInterviews));
                }

                    
                

                
            }
            return View(interviewsDTO);


        }





    }
}
