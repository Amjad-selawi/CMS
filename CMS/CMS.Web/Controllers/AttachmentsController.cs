using CMS.Services.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Threading.Tasks;
using System;
using CMS.Application.DTOs;
using CMS.Web.Utils;
using CMS.Services.Services;
using System.Security.Claims;

namespace CMS.Web.Controllers
{
    public class AttachmentsController : Controller
    {
        private readonly IAttachmentService _attachmentService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly string _attachmentStoragePath;

        public AttachmentsController(IAttachmentService attachmentService, IWebHostEnvironment env,IHttpContextAccessor httpContextAccessor)
        {
            _attachmentService = attachmentService;
            _httpContextAccessor = httpContextAccessor;
            _attachmentStoragePath = Path.Combine(env.WebRootPath, "attachments");

            if (!Directory.Exists(_attachmentStoragePath))
            {
                Directory.CreateDirectory(_attachmentStoragePath);
            }
        }
        public void LogException(string methodName, Exception ex, string additionalInfo = null)
        {
            
            _attachmentService.LogException(methodName, ex, additionalInfo);
        }
        
        public async Task<IActionResult> Index()
        {
            try
            {
                var attachments = await _attachmentService.GetAllAttachmentsAsync();
                return View(attachments);
            }
            catch (Exception ex)
            {
                LogException(nameof(Index), ex,"not working");
                throw ex;
            }
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(IFormFile file)
        {
            try
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
            catch (Exception ex)
            {
                LogException(nameof(Create), ex, "Faild to create an Attachment");
                throw ex;
            }
        }
        public async Task<IActionResult> Download(int id)
        {
            try
            {
                var attachment = await _attachmentService.GetAttachmentByIdAsync(id);
                if (attachment == null)
                {
                    return NotFound();
                }

                string contentType = "application/octet-stream";
                var result = new FileContentResult(attachment.FileData, contentType)
                {
                    FileDownloadName = attachment.FileName
                };

                return result;
            }
            catch (Exception ex)
            {
                LogException(nameof(Download), ex, $"Faild to download Attachment ID: {id}");
                throw ex;
            }
        }
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var attachment = await _attachmentService.GetAttachmentByIdAsync(id);
                if (attachment == null)
                {
                    return NotFound();
                }

                return View(attachment);
            }
            catch (Exception ex)
            {
                LogException(nameof(Edit), ex, $"Faild to load Attachment ID: {id} edit page");
                throw ex;
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, AttachmentDTO attachmentDTO)
        {
            try
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
            catch (Exception ex)
            {
                LogException(nameof(Edit), ex, $"Faild to edit Attachment ID: {id}");
                throw ex;
            }
        }
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var attachment = await _attachmentService.GetAttachmentByIdAsync(id);
                if (attachment == null)
                {
                    return NotFound();
                }

                return View(attachment);
            }
            catch (Exception ex)
            {
                LogException(nameof(Delete), ex, $"Faild to load Attachment ID: {id} page");
                throw ex;
            }
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                await _attachmentService.DeleteAttachmentAsync(id);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                LogException(nameof(DeleteConfirmed), ex, $"Faild to delete Attachment ID: {id}");
                throw ex;
            }
        }


    }
}
