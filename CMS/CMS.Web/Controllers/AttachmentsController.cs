using CMS.Services.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Threading.Tasks;
using System;
using CMS.Application.DTOs;
using CMS.Web.Utils;

namespace CMS.Web.Controllers
{
    public class AttachmentsController : Controller
    {
        private readonly IAttachmentService _attachmentService;
        private readonly string _attachmentStoragePath;

        public AttachmentsController(IAttachmentService attachmentService, IWebHostEnvironment env)
        {
            _attachmentService = attachmentService;
            _attachmentStoragePath = Path.Combine(env.WebRootPath, "attachments");

            if (!Directory.Exists(_attachmentStoragePath))
            {
                Directory.CreateDirectory(_attachmentStoragePath);
            }
        }

        public async Task<IActionResult> Index()
        {
            var attachments = await _attachmentService.GetAllAttachmentsAsync();
            return View(attachments);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                ModelState.AddModelError("File", "Please choose a file to upload.");
                return View();
            }

            FileStream attachmentStream = await AttachmentHelper.handleUpload(file, _attachmentStoragePath);
            var attachmentId = await _attachmentService.CreateAttachmentAsync(file.FileName, file.Length, attachmentStream);
            attachmentStream.Close();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Download(int id)
        {
            var attachment = await _attachmentService.GetAttachmentByIdAsync(id);
            if (attachment == null)
            {
                return NotFound();
            }
            Console.WriteLine(attachment.FileName);
            if(attachment.FileData == null)
            {
                Console.WriteLine("notfound");
            }
            string contentType = "application/octet-stream";
            var result = new FileContentResult(attachment.FileData, contentType)
            {
                FileDownloadName = attachment.FileName
            };

            return result;

        }
        public async Task<IActionResult> Edit(int id)
        {
            var attachment = await _attachmentService.GetAttachmentByIdAsync(id);
            if (attachment == null)
            {
                return NotFound();
            }

            return View(attachment);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, AttachmentDTO attachmentDTO)
        {
            if (id != attachmentDTO.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                await _attachmentService.UpdateAttachmentAsync(id, attachmentDTO);
                return RedirectToAction(nameof(Index));
            }

            return View(attachmentDTO);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var attachment = await _attachmentService.GetAttachmentByIdAsync(id);
            if (attachment == null)
            {
                return NotFound();
            }

            return View(attachment);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _attachmentService.DeleteAttachmentAsync(id);
            return RedirectToAction(nameof(Index));
        }

     
    }
}
