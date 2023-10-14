using Castle.DynamicProxy.Generators.Emitters.SimpleAST;
using CMS.Application.DTOs;
using CMS.Domain.Entities;
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
using System.Linq;
using System.Reflection.Metadata;
using System.Threading.Tasks;

namespace CMS.Web.Controllers
{

    public class CandidatesController : Controller
    {
        private readonly ICandidateService _candidateService;
        private readonly string _attachmentStoragePath;
        private readonly IPositionService _positionService;
        private readonly ICompanyService _companyService;
        private readonly ICountryService _countryService;
        private readonly IAttachmentService _attachmentService;

        public CandidatesController(ICandidateService candidateService,
            IWebHostEnvironment env,
            IPositionService positionService,
            ICompanyService companyService, ICountryService countryService,IAttachmentService attachmentService)
        {
            _candidateService = candidateService;
            _attachmentStoragePath = Path.Combine(env.WebRootPath, "attachments");

            if (!Directory.Exists(_attachmentStoragePath))
            {
                Directory.CreateDirectory(_attachmentStoragePath);
            }
            _positionService = positionService;
            _companyService = companyService;
            _countryService = countryService;
            _attachmentService = attachmentService;
        }

        public async Task<IActionResult> Index(string FullName, string Phone)
        {
            if (User.IsInRole("Admin") || User.IsInRole("HR Manager"))
            {
                var candidates = await _candidateService.GetAllCandidatesAsync();

                if (!string.IsNullOrEmpty(Phone))
                {
                    // Filter by phone number containing the provided input
                    candidates = candidates
                        .Where(i => i.Phone.ToString().Contains(Phone))
                        .ToList();
                }

                if (!string.IsNullOrEmpty(FullName))
                {
                    candidates = candidates
                        .Where(i => i.FullName.Contains(FullName, StringComparison.OrdinalIgnoreCase))
                        .ToList();
                }

                return View(candidates);
            }
            else
            {
                return View("AccessDenied");
            }
        }
        public async Task<IActionResult> Details(int id)
        {
            var Country = await _countryService.GetAll();
            ViewBag.CountryDTOs = new SelectList(Country.Value, "Id", "Name");


            var candidate = await _candidateService.GetCandidateByIdAsync(id);

            if (candidate == null)
            {
                return NotFound();
            }
            return View(candidate);
        }

        public async Task<IActionResult> Create()
        {
            var positions = await _positionService.GetAll();
            ViewBag.positions = new SelectList(positions.Value, "Id", "Name");

            var CompaniesDTOs=await _companyService.GetAll();
            ViewBag.CompaniesDTOs = new SelectList(CompaniesDTOs.Value, "Id", "Name");

            var Country = await _countryService.GetAll();
            ViewBag.CountryDTOs = new SelectList(Country.Value, "Id", "Name");

            return View();
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CandidateCreateDTO candidateDTO, IFormFile file)
        {
            var positions = await _positionService.GetAll();
            ViewBag.positions = new SelectList(positions.Value, "Id", "Name");

            var CompaniesDTOs = await _companyService.GetAll();
            ViewBag.CompaniesDTOs = new SelectList(CompaniesDTOs.Value, "Id", "Name");

            var Country = await _countryService.GetAll();
            ViewBag.CountryDTOs = new SelectList(Country.Value, "Id", "Name");

            if (file != null && file.Length > 0)
            {
                // Check file extension and size
                var allowedExtensions = new[] { ".pdf", ".docx", ".png", ".jpg" };
                var maxFileSize = 4 * 1024 * 1024; // 4MB

                var fileExtension = Path.GetExtension(file.FileName).ToLower();
                if (!allowedExtensions.Contains(fileExtension))
                {
                    ModelState.AddModelError("File", "Invalid file format. Allowed formats are PDF, DOCX, PNG, and JPG.");
                }
              
                else if (file.Length > maxFileSize)
                {
                    ModelState.AddModelError("File", "File size exceeds the maximum allowed size (4MB).");
                }

                if (ModelState.IsValid)
                {
                    FileStream attachmentStream = null;

                    try
                    {
                        attachmentStream = await AttachmentHelper.handleUpload(file, _attachmentStoragePath);
                        candidateDTO.FileName = file.FileName;
                        candidateDTO.FileSize = file.Length;
                        candidateDTO.FileData = attachmentStream;

                        await _candidateService.CreateCandidateAsync(candidateDTO);
                        return RedirectToAction(nameof(Index));
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    finally
                    {
                        if (attachmentStream != null)
                        {
                            attachmentStream.Close();
                            attachmentStream.Dispose(); // Dispose the stream to release the file
                            AttachmentHelper.removeFile(file.FileName, _attachmentStoragePath);
                        }
                    }
                }
            }
            else
            {
                ModelState.AddModelError("File", "Please choose a file to upload.");
            }

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
            ViewBag.positions = new SelectList(positions.Value, "Id", "Name");
            var CompaniesDTOs = await _companyService.GetAll();
            ViewBag.CompaniesDTOs = new SelectList(CompaniesDTOs.Value, "Id", "Name");

            var Country = await _countryService.GetAll();
            ViewBag.CountryDTOs = new SelectList(Country.Value, "Id", "Name");

            return View(candidate);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CandidateDTO candidateDTO, IFormFile file)
        {
            if (id != candidateDTO.Id)
            {
                return NotFound();
            }

            var positions = await _positionService.GetAll();
            ViewBag.positions = new SelectList(positions.Value, "Id", "Name");

            var CompaniesDTOs = await _companyService.GetAll();
            ViewBag.CompaniesDTOs = new SelectList(CompaniesDTOs.Value, "Id", "Name");

            var Country = await _countryService.GetAll();
            ViewBag.CountryDTOs = new SelectList(Country.Value, "Id", "Name");

            if (file != null && file.Length > 0)
            {
                // Check file extension and size
                var allowedExtensions = new[] { ".pdf", ".docx", ".png", ".jpg" };
                var maxFileSize = 4 * 1024 * 1024; // 4MB

                var fileExtension = Path.GetExtension(file.FileName).ToLower();
                if (!allowedExtensions.Contains(fileExtension))
                {
                    ModelState.AddModelError("File", "Invalid file format. Allowed formats are PDF, DOCX, PNG, and JPG.");
                }
                else if (file.Length < 1024 * 1024) // 1MB
                {
                    ModelState.AddModelError("File", "File size is too small. Minimum size allowed is 1MB.");
                }
                else if (file.Length > maxFileSize)
                {
                    ModelState.AddModelError("File", "File size exceeds the maximum allowed size (4MB).");
                }

                if (ModelState.IsValid)
                {
                    var stream = await AttachmentHelper.handleUpload(file, _attachmentStoragePath);
                    try
                    {
                        await _candidateService.UpdateCandidateCVAsync(id, file.FileName, file.Length, stream);
                    }
                    finally
                    {
                        stream.Close();
                        AttachmentHelper.removeFile(file.FileName, _attachmentStoragePath);
                    }
                }
            }
            else if (ModelState.IsValid) // No file uploaded, but other data is valid
            {
                await _candidateService.UpdateCandidateAsync(id, candidateDTO);
            }

            return RedirectToAction(nameof(Index));
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
                try
                {
                    await _candidateService.UpdateCandidateCVAsync(id, file.FileName, file.Length, stream);
                    return RedirectToAction(nameof(Index));
                }
                finally
                {
                    stream.Close();
                    AttachmentHelper.removeFile(file.FileName, _attachmentStoragePath);
                }
                
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




