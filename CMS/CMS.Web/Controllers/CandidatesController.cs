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


        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Create(CandidateCreateDTO candidateDTO, IFormFile file)
        //{

        //        var positions = await _positionService.GetAll();
        //        ViewBag.positions = new SelectList(positions.Value, "Id", "Name");

        //        var CompaniesDTOs = await _companyService.GetAll();
        //        ViewBag.CompaniesDTOs = new SelectList(CompaniesDTOs.Value, "Id", "Name");

        //        var Country = await _countryService.GetAll();
        //        ViewBag.CountryDTOs = new SelectList(Country.Value, "Id", "Name");

        //    //if (file == null || file.Length == 0)
        //    //    {
        //    //        ModelState.AddModelError("File", "Please choose a file to upload.");
        //    //        return View();
        //    //    }
        //    FileStream attachmentStream=null;
        //    if (file!=null && file.Length != 0)
        //    {
        //        attachmentStream = await AttachmentHelper.handleUpload(file, _attachmentStoragePath);
        //        candidateDTO.FileName = file.FileName;
        //        candidateDTO.FileSize = file.Length;
        //        candidateDTO.FileData = attachmentStream;
        //    }

        //       // FileStream attachmentStream = await AttachmentHelper.handleUpload(file, _attachmentStoragePath);
        //    try
        //    {
        //        //candidateDTO.FileName = file.FileName;
        //        //candidateDTO.FileSize = file.Length;
        //        //candidateDTO.FileData = attachmentStream;
        //        if (ModelState.IsValid)
        //        {
        //            await _candidateService.CreateCandidateAsync(candidateDTO);

        //            return RedirectToAction(nameof(Index));
        //        }
        //        Console.WriteLine(ModelState.ValidationState);
        //        return View(candidateDTO);
        //    } catch(Exception ex) {
        //        throw ex;

        //    } finally
        //    {
        //        if (attachmentStream != null)
        //        {
        //            attachmentStream.Close();
        //            AttachmentHelper.removeFile(file.FileName, _attachmentStoragePath);
        //        }



        //    }

        //}

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


        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Edit(int id, CandidateDTO candidateDTO)
        //{
        //    if (id != candidateDTO.Id)
        //    {
        //        return NotFound();
        //    }
        //    var positions = await _positionService.GetAll();
        //    ViewBag.positions = new SelectList(positions.Value, "Id", "Name");

        //    var CompaniesDTOs = await _companyService.GetAll();
        //    ViewBag.CompaniesDTOs = new SelectList(CompaniesDTOs.Value, "Id", "Name");

        //    var Country = await _countryService.GetAll();
        //    ViewBag.CountryDTOs = new SelectList(Country.Value, "Id", "Name");

        //    if (ModelState.IsValid)
        //    {
        //        await _candidateService.UpdateCandidateAsync(id, candidateDTO);
        //        return RedirectToAction(nameof(Index));
        //    }
        //    return View(candidateDTO);
        //}
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



//using CMS.Application.DTOs;
//using CMS.Services.Interfaces;
//using CMS.Services.Services;
//using CMS.Web.Utils;
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Hosting;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.AspNetCore.Mvc.Rendering;
//using System;
//using System.Diagnostics;
//using System.IO;
//using System.Threading.Tasks;

//namespace CMS.Web.Controllers
//{

//    public class CandidatesController : Controller
//    {
//        private readonly ICandidateService _candidateService;
//        private readonly string _attachmentStoragePath;
//        private readonly IPositionService _positionService;

//        public CandidatesController(ICandidateService candidateService, IWebHostEnvironment env, IPositionService positionService)
//        {
//            _candidateService = candidateService;
//            _attachmentStoragePath = Path.Combine(env.WebRootPath, "attachments");

//            if (!Directory.Exists(_attachmentStoragePath))
//            {
//                Directory.CreateDirectory(_attachmentStoragePath);
//            }
//            _positionService = positionService;
//        }


//        //public async Task<IActionResult> Index()
//        //{
//        //    var result = await _candidateService.GetAll();
//        //    if (result.IsSuccess)
//        //    {
//        //        var candidateDTOs = result.Value;
//        //        return View(candidateDTOs);
//        //    }
//        //    else
//        //    {
//        //        ModelState.AddModelError("", result.Error);
//        //        return View();
//        //    }

//        //}
//        //public async Task<IActionResult> Details(int id)
//        //{
//        //    var candidate = await _candidateService.GetById(id);

//        //    if (candidate == null)
//        //    {
//        //        return NotFound();
//        //    }
//        //    return View(candidate);
//        //}

//        //public IActionResult Create()
//        //{
//        //    return View();
//        //}

//        //[HttpPost]
//        //[ValidateAntiForgeryToken]
//        //public async Task<IActionResult> Create(CandidateCreateDTO candidateDTO, IFormFile file)
//        //{
//        //    if (file == null || file.Length == 0)
//        //    {
//        //        ModelState.AddModelError("File", "Please choose a file to upload.");
//        //        return View();
//        //    }

//        //    FileStream attachmentStream = await AttachmentHelper.handleUpload(file, _attachmentStoragePath);
//        //    candidateDTO.FileName = file.FileName;
//        //    candidateDTO.FileSize = file.Length;
//        //    candidateDTO.FileData = attachmentStream;

//        //    if (ModelState.IsValid)
//        //    {
//        //        var result = await _candidateService.Insert(candidateDTO);
//        //        attachmentStream.Close();

//        //        if (result.IsSuccess)
//        //        {
//        //            return RedirectToAction("Index");
//        //        }

//        //        ModelState.AddModelError("", result.Error);
//        //    }
//        //    else
//        //    {
//        //        ModelState.AddModelError("", "error validating the model");
//        //    }

//        //    return View(candidateDTO);
//        //    //if (ModelState.IsValid)
//        //    //{
//        //    //    await _candidateService.CreateCandidateAsync(candidateDTO);
//        //    //    attachmentStream.Close();
//        //    //    return RedirectToAction(nameof(Index));
//        //    //}
//        //    //Console.WriteLine(ModelState.ValidationState);
//        //    //return View(candidateDTO);
//        //}

//        //public async Task<IActionResult> Edit(int id)
//        //{
//        //    if (id <= 0)
//        //    {
//        //        return NotFound();
//        //    }
//        //    var result = await _candidateService.GetById(id);
//        //    var candidateDTO = result.Value;
//        //    if (candidateDTO == null)
//        //    {
//        //        return NotFound();
//        //    }
//        //    return View(candidateDTO);


//        //}

//        //[HttpPost]
//        //[ValidateAntiForgeryToken]
//        //public async Task<IActionResult> Edit(int id, CandidateCreateDTO candidateDTO)
//        //{
//        //    if (ModelState.IsValid)
//        //    {
//        //        var result = await _candidateService.Update(candidateDTO);

//        //        if (result.IsSuccess)
//        //        {
//        //            return RedirectToAction("Index");
//        //        }
//        //        ModelState.AddModelError("", result.Error);
//        //        return View(candidateDTO);
//        //    }
//        //    else
//        //    {
//        //        ModelState.AddModelError("", $"the model state is not valid");
//        //    }
//        //    return View(candidateDTO);

//        //}
//        //[HttpPost]
//        //[ValidateAntiForgeryToken]
//        //public async Task<IActionResult> UpdateAttachment(int id, IFormFile file)
//        //{
//        //    if (file == null || file.Length == 0)
//        //    {
//        //        ModelState.AddModelError("File", "Please choose a file to upload.");
//        //        return View();
//        //    }
//        //    if (ModelState.IsValid)
//        //    {
//        //        var stream = await AttachmentHelper.handleUpload(file, _attachmentStoragePath);
//        //        await _candidateService.UpdateCandidateCVAsync(id, file.FileName, file.Length, stream);
//        //        return RedirectToAction(nameof(Index));
//        //    }
//        //    return RedirectToAction(nameof(Edit), new { id = id });
//        //}

//        //public async Task<IActionResult> Delete(int id)
//        //{
//        //    var result = await _candidateService.GetById(id);
//        //    if (result.IsSuccess)
//        //    {
//        //        var positionDTO = result.Value;
//        //        return View(positionDTO);
//        //    }


//        //    else
//        //    {
//        //        ModelState.AddModelError("", result.Error);
//        //        return View();
//        //    }
//        //    //    var candidate = await _candidateService.GetById(id);
//        //    //    if (candidate == null)
//        //    //    {
//        //    //        return NotFound();
//        //    //    }
//        //    //    return View(candidate);
//        //}

//        //[HttpPost, ActionName("Delete")]
//        //[ValidateAntiForgeryToken]
//        //public async Task<IActionResult> DeleteConfirmed(int id)
//        //{
//        //    if (id <= 0)
//        //    {
//        //        return BadRequest("invalid career offer id");
//        //    }
//        //    var result = await _candidateService.Delete(id);
//        //    if (result.IsSuccess)
//        //    {
//        //        return RedirectToAction("Index");
//        //    }
//        //    ModelState.AddModelError("", result.Error);
//        //    return View();

//        //    //await _candidateService.Delete(id);
//        //    //return RedirectToAction(nameof(Index));
//        //}





//        public async Task<IActionResult> Index()
//        {
//            var candidates = await _candidateService.GetAllCandidatesAsync();
//            return View(candidates);
//        }
//        public async Task<IActionResult> Details(int id)
//        {
//            var candidate = await _candidateService.GetCandidateByIdAsync(id);

//            if (candidate == null)
//            {
//                return NotFound();
//            }
//            return View(candidate);
//        }

//        public async Task<IActionResult>Create()
//        {
//            var positions=await _positionService.GetAll();
//            ViewBag.positions = new SelectList(positions,"Id","Name");
//            return View();
//        }

//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public async Task<IActionResult> Create(CandidateCreateDTO candidateDTO, IFormFile file)
//        {
//            var positions = await _positionService.GetAll();
//            ViewBag.positions = new SelectList(positions, "Id", "Name");

//            if (file == null || file.Length == 0)
//            {
//                ModelState.AddModelError("File", "Please choose a file to upload.");
//                return View();
//            }

//            FileStream attachmentStream = await AttachmentHelper.handleUpload(file, _attachmentStoragePath);
//            candidateDTO.FileName = file.FileName;
//            candidateDTO.FileSize = file.Length;
//            candidateDTO.FileData = attachmentStream;
//            if (ModelState.IsValid)
//            {
//                await _candidateService.CreateCandidateAsync(candidateDTO);
//                attachmentStream.Close();
//                return RedirectToAction(nameof(Index));
//            }
//            Console.WriteLine(ModelState.ValidationState);
//            return View(candidateDTO);
//        }

//        public async Task<IActionResult> Edit(int id)
//        {
//            var candidate = await _candidateService.GetCandidateByIdAsync(id);
//            if (candidate == null)
//            {
//                return NotFound();
//            }
//            var positions = await _positionService.GetAll();
//            ViewBag.positions = new SelectList(positions, "Id", "Name");

//            return View(candidate);
//        }

//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public async Task<IActionResult> Edit(int id, CandidateDTO candidateDTO)
//        {
//            if (id != candidateDTO.Id)
//            {
//                return NotFound();
//            }
//            var positions = await _positionService.GetAll();
//            ViewBag.positions = new SelectList(positions, "Id", "Name");
//            if (ModelState.IsValid)
//            {
//                await _candidateService.UpdateCandidateAsync(id, candidateDTO);
//                return RedirectToAction(nameof(Index));
//            }
//            return View(candidateDTO);
//        }
//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public async Task<IActionResult> UpdateAttachment(int id, IFormFile file)
//        {
//            if (file == null || file.Length == 0)
//            {
//                ModelState.AddModelError("File", "Please choose a file to upload.");
//                return View();
//            }
//            if (ModelState.IsValid)
//            {
//                var stream = await AttachmentHelper.handleUpload(file, _attachmentStoragePath);
//                await _candidateService.UpdateCandidateCVAsync(id, file.FileName, file.Length, stream);
//                return RedirectToAction(nameof(Index));
//            }
//            return RedirectToAction(nameof(Edit), new { id = id });
//        }

//        public async Task<IActionResult> Delete(int id)
//        {
//            var candidate = await _candidateService.GetCandidateByIdAsync(id);
//            if (candidate == null)
//            {
//                return NotFound();
//            }
//            return View(candidate);
//        }

//        [HttpPost, ActionName("Delete")]
//        [ValidateAntiForgeryToken]
//        public async Task<IActionResult> DeleteConfirmed(int id)
//        {
//            await _candidateService.DeleteCandidateAsync(id);
//            return RedirectToAction(nameof(Index));
//        }
//    }
//}
