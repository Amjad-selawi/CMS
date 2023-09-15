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
        public readonly IAccountService _accountService;

        public InterviewsController(IInterviewsService interviewsService,ICandidateService candidateService,IPositionService positionService, IAccountService accountService)
        {
            _interviewsService = interviewsService;
            _candidateService = candidateService;
            _positionService = positionService;
            _accountService = accountService;
        }

        private async Task LoadSelectionLists()
        {
            var positions = await _positionService.GetAll();
            ViewBag.positionList = new SelectList(positions, "Id", "Name");
            var candidates = await _candidateService.GetAllCandidatesAsync();
            ViewBag.candidateList = new SelectList(candidates, "Id", "FullName");
            var interviewers = await _accountService.GetAllInterviewers();
            ViewBag.interviewersList = new SelectList(interviewers.Value, "Id", "UserName");
        }

        // GET: InterviewsController
        public async Task<ActionResult> Index()
        {
            var result = await _interviewsService.GetAllWithLabels();
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
            var result = await _interviewsService.GetByIdWithLabels(id);
            await LoadSelectionLists();

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
            await LoadSelectionLists();
            return View();
        }

        // POST: InterviewsController/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(InterviewsDTO collection)
        {
            
            if (ModelState.IsValid)
            {
                await LoadSelectionLists();
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
            var result = await _interviewsService.GetById(id)
;
            var interviewDTO = result.Value;
            if (interviewDTO == null)
            {
                return NotFound();
            }
            await LoadSelectionLists();

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

            await LoadSelectionLists();


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
            var result = await _interviewsService.GetById(id)
;
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
            var result = await _interviewsService.Delete(id)
;
            if (result.IsSuccess)
            {
                return RedirectToAction("Index");
            }
            ModelState.AddModelError("", result.Error);
            return View();
        }

    }
}
