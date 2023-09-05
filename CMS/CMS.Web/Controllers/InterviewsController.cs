﻿using CMS.Application.DTOs;
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
        

        public InterviewsController(IInterviewsService interviewsService)
        {
            _interviewsService = interviewsService;
            
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
            return View(interview);
        }

        // GET: InterviewsController/Create
        public async Task<ActionResult> Create()
        {
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

            if (ModelState.IsValid)
            {
                await _interviewsService.Update(id, collection);
                return RedirectToAction(nameof(Index));
            }

     
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

    }
}