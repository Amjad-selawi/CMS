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

namespace CMS.Web.Controllers
{
    
    public class InterviewsController : Controller
    {
        private readonly IInterviewsService _interviewsService;
        private readonly ICandidateService _candidateService;
        private readonly IPositionService _positionService;

        public InterviewsController(IInterviewsService interviewsService,ICandidateService candidateService,IPositionService positionService)
        {
            _interviewsService = interviewsService;
            _candidateService = candidateService;
            _positionService = positionService;
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

            var interviewsDTOs = await _positionService.GetAll();
            ViewBag.positionDTOs = new SelectList(interviewsDTOs.Value, "PositionId", "Name");

            var positionsDTO = await _candidateService.GetAllCandidatesAsync();
            ViewBag.candidateDTOs = new SelectList(positionsDTO, "CandidateId", "FullName");

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
            var interviewsDTOs = await _positionService.GetAll();
            ViewBag.positionDTOs = new SelectList(interviewsDTOs.Value, "PositionId", "Name");

            var positionsDTO = await _candidateService.GetAllCandidatesAsync();
            ViewBag.candidateDTOs = new SelectList(positionsDTO, "CandidateId", "FullName");
            return View();
        }

        // POST: InterviewsController/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(InterviewsDTO collection)
        {
            var interviewsDTOs = await _positionService.GetAll();
            ViewBag.positionDTOs = new SelectList(interviewsDTOs.Value, "PositionId", "Name");

            var positionsDTO = await _candidateService.GetAllCandidatesAsync();
            ViewBag.candidateDTOs = new SelectList(positionsDTO, "CandidateId", "FullName");

            if (!ModelState.IsValid)
            {
                var result = await _interviewsService.Insert(collection);

                if (result.IsSuccess)
                {
                    return RedirectToAction("Index");
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
            var interviewsDTOs = await _positionService.GetAll();
            ViewBag.positionDTOs = new SelectList(interviewsDTOs.Value, "PositionId", "Name");

            var positionsDTO = await _candidateService.GetAllCandidatesAsync();
            ViewBag.candidateDTOs = new SelectList(positionsDTO, "CandidateId", "FullName");

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

            var PositionsDTOs = await _positionService.GetAll();
            ViewBag.positionDTOs = new SelectList(PositionsDTOs.Value, "PositionId", "Name");

            var positionsDTO = await _candidateService.GetAllCandidatesAsync();
            ViewBag.candidateDTOs = new SelectList(positionsDTO, "CandidateId", "FullName");


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




        //// GET: InterviewsController
        //public async Task<ActionResult> Index()
        //{
        //    var interviews = await _interviewsService.GetAllInterviewsAsync();
        //    return View(interviews);
        //}

        //// GET: InterviewsController/Details/5
        //public async Task<ActionResult> Details(int id)
        //{
        //    var interview = await _interviewsService.GetInterviewByIdAsync(id);

        //    var candidate = await _candidateService.GetAllCandidatesAsync();
        //    ViewBag.candidateList = new SelectList(candidate, "CandidateId", "FullName", interview.InterviewsId);

        //    //var position = await _positionService.GetAll();
        //    //ViewBag.positionList = new SelectList(position, "PositionId", "Name", interview.InterviewsId);



        //    return View(interview);
        //}

        ////[Authorize(Roles = "General Manager")]
        //// GET: InterviewsController/Create
        //public async Task<ActionResult> Create()
        //{
        //    var candidate = await _candidateService.GetAllCandidatesAsync();
        //    ViewBag.candidateList = new SelectList(candidate, "CandidateId", "FullName");

        //    //var position = await _positionService.GetAll();
        //    //ViewBag.positionList = new SelectList(position, "PositionId", "Name");


        //    var interviewStatuses = Enum.GetValues(typeof(InterviewStatus))
        //    .Cast<InterviewStatus>()
        //    .Select(status => new SelectListItem
        //    {
        //        Text = status.ToString(),
        //        Value = status.ToString()
        //    })
        //.ToList();


        //    ViewBag.InterviewStatuses = interviewStatuses;

        //    return View();
        //}

        //// POST: InterviewsController/Create

        ////[Authorize(Roles = "General Manager")]
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> Create(InterviewsDTO collection)
        //{

        //    if (!ModelState.IsValid)
        //    {

        //        await _interviewsService.Create(collection);
        //        return RedirectToAction(nameof(Index));
        //    }

        //    var candidate = await _candidateService.GetAllCandidatesAsync();
        //    ViewBag.candidateList = new SelectList(candidate, "CandidateId", "FullName");

        //    //var position = await _positionService.GetAll();
        //    //ViewBag.positionList = new SelectList(position, "PositionId", "Name");


        //    var interviewStatuses = Enum.GetValues(typeof(InterviewStatus))
        //    .Cast<InterviewStatus>()
        //    .Select(s => new SelectListItem
        //    {
        //    Text = s.ToString(),
        //    Value = s.ToString()
        //    })
        //.ToList();

        //    ViewBag.InterviewStatuses = interviewStatuses;


        //    return View(collection);


        //}

        //// GET: InterviewsController/Edit/5
        //public async Task<ActionResult> Edit(int id)
        //{
        //    var interview = await _interviewsService.GetInterviewByIdAsync(id);

        //    var candidate = await _candidateService.GetAllCandidatesAsync();
        //    ViewBag.candidateList = new SelectList(candidate, "CandidateId", "FullName", interview.InterviewsId);

        //    //var position = await _positionService.GetAll();
        //    //ViewBag.positionList = new SelectList(position, "PositionId", "Name", interview.InterviewsId);

        //    return View(interview);
        //}

        //// POST: InterviewsController/Edit/5
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> Edit(int id, InterviewsDTO collection)
        //{
        //    if (id != collection.InterviewsId)
        //    {
        //        return NotFound();
        //    }

        //    var cand = await _candidateService.GetCandidateByIdAsync(id);
        //    var posi = await _positionService.GetById(id);

        //    if (!ModelState.IsValid)
        //    {
        //        await _interviewsService.Update(id, collection);
        //        return RedirectToAction(nameof(Index));
        //    }

        //    var candidate = await _candidateService.GetAllCandidatesAsync();
        //    ViewBag.candidateList = new SelectList(candidate, "CandidateId", "FullName", cand.CandidateId);

        //    //var position = await _positionService.GetAll();
        //    //ViewBag.positionList = new SelectList(position, "PositionId", "Name", posi.PositionId);

        //    return View(collection);
        //}


        //// GET: InterviewsController/Delete/5
        //public async Task<ActionResult> Delete(int id)
        //{
        //    var interview = await _interviewsService.GetInterviewByIdAsync(id);

        //    return View(interview);
        //}

        //// POST: InterviewsController/Delete/5
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> Delete(int id, InterviewsDTO collection)
        //{
        //    try
        //    {
        //        await _interviewsService.Delete(id);
        //        return RedirectToAction(nameof(Index));
        //    }
        //    catch
        //    {
        //        return View();
        //    }
        //}

    }
}
