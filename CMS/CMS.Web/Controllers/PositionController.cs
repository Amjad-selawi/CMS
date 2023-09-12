using CMS.Application.DTOs;
using CMS.Domain.Entities;
using CMS.Services.Interfaces;
using CMS.Services.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace CMS.Web.Controllers
{
    public class PositionController : Controller
    {
        private readonly IPositionService _positionService;
        public PositionController(IPositionService positionService)
        {
            _positionService = positionService;
        }

        public IActionResult Index()
        {
            return View();
        }
        public IActionResult AddPosition()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddPosition(PositionDTO positionDTO)
        {
            if (ModelState.IsValid)
            {
                var result = await _positionService.Insert(positionDTO);

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

            //return View("Index", positionDTO);
            return View(positionDTO);
        }
        [HttpGet]
        public async Task<IActionResult> GetPositions()
        {

            var position = await _positionService.GetAll();
            
            return View(position);


            //var result = await _positionService.GetAll();
            //if (result.IsSuccess)
            //{
            //    var positionsDTOs = result.Value;
            //    return View(positionsDTOs);
            //}
            //else
            //{
            //    ModelState.AddModelError("", result.Error);
            //    return View();
            //}
        }
        [HttpPost]
        public async Task<IActionResult> DeletePosition(int id)
        {
            if (id <= 0)
            {
                return BadRequest("invalid position id");
            }
            var result = await _positionService.Delete(id);
            if (result.IsSuccess)
            {
                return RedirectToAction("GetPositions");
            }
            ModelState.AddModelError("", result.Error);
            // return RedirectToAction("GetPositions");
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> UpdatePosition(int id)
        {
            var poisiton = await _positionService.GetById(id);

            if (poisiton == null)
            {
                return NotFound();
            }
         
            return View(poisiton);

        }

        [HttpPost]
        public async Task<IActionResult> UpdatePosition(PositionDTO positionDTO)
        {
            if (ModelState.IsValid)
            {
                var result = await _positionService.Update(positionDTO);

                if (result.IsSuccess)
                {
                    return RedirectToAction("GetPositions");
                }
                ModelState.AddModelError("", result.Error);
               // return RedirectToAction("GetPositions");
               return View(positionDTO);
            }
            else
            {
                ModelState.AddModelError("", $"the model state is not valid");
            }
            return View(positionDTO);

        }
    }
}
