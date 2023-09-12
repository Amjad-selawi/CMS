﻿using CMS.Application.DTOs;
using CMS.Services.Interfaces;
using CMS.Web.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace CMS.Web.Controllers
{
    
    public class CandidatesController : Controller
    {
        private readonly ICandidateService _candidateService;
        private readonly string _attachmentStoragePath;
        private readonly IPositionService _positionService;

        public CandidatesController(ICandidateService candidateService, IWebHostEnvironment env, IPositionService positionService)
        {
            _candidateService = candidateService;
            _attachmentStoragePath = Path.Combine(env.WebRootPath, "attachments");

            if (!Directory.Exists(_attachmentStoragePath))
            {
                Directory.CreateDirectory(_attachmentStoragePath);
            }
            _positionService = positionService;
        }

        public async Task<IActionResult> Index()
        {
            var candidates = await _candidateService.GetAllCandidatesAsync();
            return View(candidates);
        }
        public async Task<IActionResult> Details(int id)
        {
            var candidate = await _candidateService.GetCandidateByIdAsync(id);
            
            if (candidate == null)
            {
                return NotFound();
            }
            return View(candidate);
        }

        public async Task<IActionResult>Create()
        {
            var positions=await _positionService.GetAll();
            ViewBag.positions = new SelectList(positions,"Id","Name");
            return View();
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CandidateCreateDTO candidateDTO, IFormFile file)
        {
            var positions = await _positionService.GetAll();
            ViewBag.positions = new SelectList(positions, "Id", "Name");

            if (file == null || file.Length == 0)
            {
                ModelState.AddModelError("File", "Please choose a file to upload.");
                return View();
            }

            FileStream attachmentStream = await AttachmentHelper.handleUpload(file, _attachmentStoragePath);
            candidateDTO.FileName = file.FileName;
            candidateDTO.FileSize = file.Length;
            candidateDTO.FileData = attachmentStream;
            if (ModelState.IsValid)
            {
                await _candidateService.CreateCandidateAsync(candidateDTO);
                attachmentStream.Close();
                return RedirectToAction(nameof(Index));
            }
            Console.WriteLine(ModelState.ValidationState);
            return View(candidateDTO);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var candidate = await _candidateService.GetCandidateByIdAsync(id);
            if (candidate == null)
            {
                return NotFound();
            }
            var positions = await _positionService.GetAll();
            ViewBag.positions = new SelectList(positions, "Id", "Name");

            return View(candidate);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CandidateDTO candidateDTO)
        {
            //Edit
            if (id != candidateDTO.Id)
            {
                return NotFound();
            }
            var positions = await _positionService.GetAll();
            ViewBag.positions = new SelectList(positions, "Id", "Name");
            if (ModelState.IsValid)
            {
                await _candidateService.UpdateCandidateAsync(id, candidateDTO);
                return RedirectToAction(nameof(Index));
            }
            return View(candidateDTO);
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
                await _candidateService.UpdateCandidateCVAsync(id, file.FileName,file.Length, stream);
                return RedirectToAction(nameof(Index));
            }
            return RedirectToAction(nameof(Edit), new { id = id });
        }

        public async Task<IActionResult> Delete(int id)
        {
            var candidate = await _candidateService.GetCandidateByIdAsync(id);
            if (candidate == null)
            {
                return NotFound();
            }
            return View(candidate);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _candidateService.DeleteCandidateAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
