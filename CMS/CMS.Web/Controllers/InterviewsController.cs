using Microsoft.AspNetCore;
using CMS.Application.DTOs;
using CMS.Domain.Entities;
using CMS.Domain.Enums;
using CMS.Services.Interfaces;
using CMS.Services.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.VisualBasic;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace CMS.Web.Controllers
{
    public class InterviewsController : Controller
    {
        private readonly IInterviewsService _interviewsService;
        private readonly ICandidateService _candidateService;
        private readonly IPositionService _positionService;
        private readonly IStatusService _StatusService;

        public InterviewsController(IInterviewsService interviewsService,ICandidateService candidateService,IPositionService positionService,IStatusService statusService)
        {
            _interviewsService = interviewsService;
            _candidateService = candidateService;
            _positionService = positionService;
            _StatusService = statusService;
        }

        // GET: InterviewsController
        public async Task<ActionResult> Index()
        {
            var interviews = await _interviewsService.GetAllInterviewsAsync();
            return View(interviews);
        }

        // GET: InterviewsController/Details/5
        public async Task<ActionResult> Details(int id)
        {
            var interview = await _interviewsService.GetInterviewByIdAsync(id);

            var candidate = await _candidateService.GetAllCandidatesAsync();
            ViewBag.candidateList = new SelectList(candidate,"Id", "FullName", interview.InterviewsId);

            var position = await _positionService.GetAll();
            ViewBag.positionList = new SelectList(position, "Id", "Name", interview.InterviewsId);

        

            return View(interview);
        }

        // GET: InterviewsController/Create
        public async Task<ActionResult> Create()
        {
            var candidate = await _candidateService.GetAllCandidatesAsync();
            ViewBag.candidateList = new SelectList(candidate, "Id", "FullName");

            var position = await _positionService.GetAll();
            ViewBag.positionList = new SelectList(position, "Id", "Name");

            var StatusDTOs = await _StatusService.GetAll();
            ViewBag.StatusDTOs = new SelectList(StatusDTOs.Value, "Id", "Name");


            var interviewStatuses = Enum.GetValues(typeof(InterviewStatus))
            .Cast<InterviewStatus>()
            .Select(status => new SelectListItem
            {
                Text = status.ToString(),
                Value = status.ToString()
            })
          .ToList();


            ViewBag.InterviewStatuses = interviewStatuses;

            return View();
        }

        // POST: InterviewsController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(InterviewsDTO collection)
        {

            if (ModelState.IsValid)
            {

                await _interviewsService.Create(collection);
                return RedirectToAction(nameof(Index));
            }
            else
            {
                ModelState.AddModelError("", "error validating the model");
            }

            var candidate = await _candidateService.GetAllCandidatesAsync();
            ViewBag.candidateList = new SelectList(candidate,"Id","FullName");

            var position = await _positionService.GetAll();
            ViewBag.positionList = new SelectList(position,"Id","Name");

            var StatusDTOs = await _StatusService.GetAll();
            ViewBag.StatusDTOs = new SelectList(StatusDTOs.Value, "Id", "Name");


            var interviewStatuses = Enum.GetValues(typeof(InterviewStatus))
            .Cast<InterviewStatus>()
            .Select(s => new SelectListItem
            {
            Text = s.ToString(),
            Value = s.ToString()
            })
        .ToList();

            ViewBag.InterviewStatuses = interviewStatuses;

            
            return View(collection);


        }

        // GET: InterviewsController/Edit/5
        public async Task<ActionResult> Edit(int id)
        {
            var interview = await _interviewsService.GetInterviewByIdAsync(id);

            var candidate = await _candidateService.GetAllCandidatesAsync();
            ViewBag.candidateList = new SelectList(candidate, "Id", "FullName", interview.InterviewsId);

            var position = await _positionService.GetAll();
            ViewBag.positionList = new SelectList(position, "Id", "Name", interview.InterviewsId);
            var StatusDTOs = await _StatusService.GetAll();
            ViewBag.StatusDTOs = new SelectList(StatusDTOs.Value, "Id", "Name");

            return View(interview);
        }

        // POST: InterviewsController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(int id, InterviewsDTO collection)
        {
            if (id != collection.InterviewsId)
            {
                return NotFound();
            }

            var cand = await _candidateService.GetCandidateByIdAsync(collection.candidateId);
            var posi = await _positionService.GetById(collection.positionId);

            if (ModelState.IsValid)
            {
                await _interviewsService.Update(id, collection);
                return RedirectToAction(nameof(Index));
            }
            else
            {
                ModelState.AddModelError("", "error validating the model...");
            }

            var candidate = await _candidateService.GetAllCandidatesAsync();
            ViewBag.candidateList = new SelectList(candidate, "Id", "FullName", cand.Id);

            var position = await _positionService.GetAll();
            ViewBag.positionList = new SelectList(position, "Id", "Name", posi.Id);

            var StatusDTOs = await _StatusService.GetAll();
            ViewBag.StatusDTOs = new SelectList(StatusDTOs.Value, "Id", "Name");

            return View(collection);
        }


        // GET: InterviewsController/Delete/5
        public async Task<ActionResult> Delete(int id)
        {
            var interview = await _interviewsService.GetInterviewByIdAsync(id);

            return View(interview);
        }

        // POST: InterviewsController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(int id, InterviewsDTO collection)
        {

            try
            {
                await _interviewsService.Delete(id);
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        [HttpGet]
        public async Task<IActionResult> UpdateAfterInterview(int id)
        {
            var StatusDTOs = await _StatusService.GetAll();
            ViewBag.StatusDTOs = new SelectList(StatusDTOs.Value, "Id", "Name");

            var interview = await _interviewsService.GetInterviewByIdAsync(id);
            if(interview == null)
            {
                return NotFound();

            }
            return View(interview);


        }

        [HttpPost]
        public async Task<IActionResult> UpdateAfterInterview(InterviewsDTO interviewsDTO)
        {
            var StatusDTOs = await _StatusService.GetAll();
            ViewBag.StatusDTOs = new SelectList(StatusDTOs.Value, "Id", "Name");
            if (ModelState.IsValid)
            {
                await _interviewsService.UpdateInterviewResult(interviewsDTO);
                
                return RedirectToAction("Index");
            }
            return View(interviewsDTO);

        }


    }
}
