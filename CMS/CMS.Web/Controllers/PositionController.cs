using CMS.Application.DTOs;
using CMS.Services.Interfaces;
using CMS.Services.Services;
using CMS.Web.Utils;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Security.Claims;
using System.Threading.Tasks;

namespace CMS.Web.Controllers
{
    public class PositionController : Controller
    {
        private readonly IPositionService _positionService;
        private readonly string _attachmentStoragePath;
        public PositionController(IPositionService positionService, IWebHostEnvironment env)
        {
            _positionService = positionService;
            _attachmentStoragePath = Path.Combine(env.WebRootPath, "attachments");

            if (!Directory.Exists(_attachmentStoragePath))
            {
                Directory.CreateDirectory(_attachmentStoragePath);
            }
        }

        public void LogException(string methodName, Exception ex, string additionalInfo = null)
        {
            var createdByUserId = GetUserId();
            _positionService.LogException(methodName, ex, createdByUserId, additionalInfo);
        }
        public string GetUserId()
        {
            try
            {
            var userId = _positionService.GetUserId();
            return userId;
            }
            catch (Exception ex)
            {
                LogException(nameof(GetUserId), ex, null);
                throw ex;
            }
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult AddPosition()
        {
            try
            {
            return View();
            }
            catch (Exception ex)
            {
                LogException(nameof(AddPosition),ex,null);
                throw ex;

            }
        }

        [HttpPost]
        public async Task<IActionResult> AddPosition(PositionDTO positionDTO, IFormFile file)
        {
            try
            {

            
            if (ModelState.IsValid)
            {

                if (_positionService.DoesPositionNameExist(positionDTO.Name))
                {
                    ModelState.AddModelError("Name", "A position with the same name already exists.");
                    return View(positionDTO);
                }
                FileStream attachmentStream = null;
                if (file!=null && file.Length != 0)
                {
                    attachmentStream = await AttachmentHelper.handleUpload(file, _attachmentStoragePath);
                    positionDTO.FileName = file.FileName;
                    positionDTO.FileSize = file.Length;
                    positionDTO.FileData = attachmentStream;
                }
              

                var result = await _positionService.Insert(positionDTO);
                if (attachmentStream != null)
                {
                    attachmentStream.Close();
                    attachmentStream.Dispose();
                    AttachmentHelper.removeFile(file.FileName, _attachmentStoragePath);

                }


                if (result.IsSuccess)
                {
                    return RedirectToAction("GetPositions");
                }

                ModelState.AddModelError("", result.Error);
            }
            else
            {
                ModelState.AddModelError("", "error validating the model");
            }

            return View(positionDTO);
            }
            catch (Exception ex)
            {
                LogException(nameof(AddPosition), ex, null);
                throw ex;
            }
        }
        [HttpGet]
        public async Task<IActionResult> GetPositions()
        {
            try
            {

            
            if (User.IsInRole("Admin") || User.IsInRole("HR Manager"))
            {
                var result = await _positionService.GetAll();
                if (result.IsSuccess)
                {
                    var positionsDTOs = result.Value;
                    return View(positionsDTOs);
                }
                else
                {
                    ModelState.AddModelError("", result.Error);
                    return View();
                }
            }
            else
            {
                return View("AccessDenied");
            }
            }
            catch(Exception ex)
            {
                LogException(nameof(GetPositions), ex, null);
                throw ex;
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetDeleteConfirmation(int id)
        {
            try
            {

            
            if (id <= 0)
            {
                return NotFound();
            }

            var result = await _positionService.GetById(id);
            var positionDTO = result.Value;

            if (positionDTO == null)
            {
                return NotFound();
            }

            return View(positionDTO);}
            catch(Exception ex)
            {
                LogException(nameof(GetDeleteConfirmation), ex, null);
                throw ex;
            }
        }



        [HttpPost]
        public async Task<IActionResult> DeletePosition(PositionDTO positionDTO)
        {
            try
            {

            
            if (positionDTO == null || positionDTO.Id <= 0)
            {
                return BadRequest("invalid position id");
            }
            var result = await _positionService.Delete(positionDTO.Id);
            if (result.IsSuccess)
            {
                return RedirectToAction("GetPositions");
            }

            ModelState.AddModelError("", result.Error);
            return View("DeleteConfirmation", positionDTO);
            }
            catch(Exception ex)
            {
                LogException(nameof(DeletePosition), ex, null);
                throw ex;
            }
        }

        [HttpGet]
        public async Task<IActionResult> UpdatePosition(int id)
        {
            try
            {

            
            if (id <= 0)
            {
                return NotFound();
            }
            var result = await _positionService.GetById(id);
            var positionDTO = result.Value;
            if (positionDTO == null)
            {
                return NotFound();
            }
            return View(positionDTO);
}
            catch(Exception ex)
            {
                LogException(nameof(UpdatePosition), ex, null);
                throw ex;
            }
        }

        [HttpPost]
        public async Task<IActionResult> UpdatePosition(PositionDTO positionDTO)
        {
            try
            {

            
            if (ModelState.IsValid)
            {
                var result = await _positionService.Update(positionDTO);

                if (result.IsSuccess)
                {
                    return RedirectToAction("GetPositions");
                }
                ModelState.AddModelError("", result.Error);
                return RedirectToAction("GetPositions");
            }
            else
            {
                ModelState.AddModelError("", $"the model state is not valid");
            }
            return View(positionDTO);
}
            catch (Exception ex)
            {
                LogException(nameof(UpdatePosition), ex, null);
                throw ex;
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateAttachment(int id, IFormFile file)
        {
            try
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
                    await _positionService.UpdatePositionEvaluationAsync(id, file.FileName, file.Length, stream);
                    return RedirectToAction(nameof(GetPositions));
                }
                finally
                {
                    stream.Close();
                    AttachmentHelper.removeFile(file.FileName, _attachmentStoragePath);
                }

            }
            return RedirectToAction(nameof(UpdatePosition), new { id = id });
            }
            catch (Exception ex)
            {
                LogException(nameof(UpdatePosition), ex, null);
                throw ex;
            }
        }



        [HttpPost]
        public IActionResult CheckPositionName([FromBody] string name)
        {
            try
            {

            
            bool exists = _positionService.DoesPositionNameExist(name);
            return Ok(new { exists });
            }
            catch (Exception ex){
                LogException(nameof(UpdatePosition), ex, null);
                throw ex;
            }
        }
    }
}


