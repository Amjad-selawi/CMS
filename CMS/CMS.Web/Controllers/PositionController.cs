using CMS.Application.DTOs;
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

            return View(positionDTO);
        }
        [HttpGet]
        public async Task<IActionResult> GetPositions()
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

        [HttpGet]
        public async Task<IActionResult> GetDeleteConfirmation(int id)
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



        [HttpPost]
        public async Task<IActionResult> DeletePosition(PositionDTO positionDTO)
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

        [HttpGet]
        public async Task<IActionResult> UpdatePosition(int id)
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
                return RedirectToAction("GetPositions");
            }
            else
            {
                ModelState.AddModelError("", $"the model state is not valid");
            }
            return View(positionDTO);

        }
    }
}


//using CMS.Application.DTOs;
//using CMS.Domain.Entities;
//using CMS.Services.Interfaces;
//using CMS.Services.Services;
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Mvc;
//using System.Threading.Tasks;

//namespace CMS.Web.Controllers
//{
//    public class PositionController : Controller
//    {
//        private readonly IPositionService _positionService;
//        public PositionController(IPositionService positionService)
//        {
//            _positionService = positionService;
//        }

//        public IActionResult Index()
//        {
//            return View();
//        }
//        public IActionResult AddPosition()
//        {
//            return View();
//        }

//        [HttpPost]
//        public async Task<IActionResult> AddPosition(PositionDTO positionDTO)
//        {
//            //if (ModelState.IsValid)
//            //{
//            //    await _positionService.Insert(positionDTO);
//            //    return RedirectToAction(nameof(GetPositions));

//            //}

//            //return View(positionDTO);

//            if (ModelState.IsValid)
//            {
//                var result = await _positionService.Insert(positionDTO);

//                if (result.IsSuccess)
//                {
//                    return RedirectToAction("GetPositions");
//                }

//                ModelState.AddModelError("", result.Error);
//            }
//            else
//            {
//                ModelState.AddModelError("", "error validating the model");
//            }

//            return View(positionDTO);

//        }
//        [HttpGet]
//        public async Task<IActionResult> GetPositions()
//        {

//            var position = await _positionService.GetAll();

//            return View(position);



//        }
//        //public async Task<ActionResult> Delete(int id)
//        //{
//        //    var poistion = await _positionService.GetById(id);

//        //    return View(poistion);
//        //}

//        //// POST: TemplatesController/Delete/5
//        //[HttpPost]
//        //[ValidateAntiForgeryToken]
//        //public async Task<ActionResult> Delete(int id, PositionDTO collection)
//        //{
//        //    try
//        //    {
//        //        await _positionService.Delete(id);
//        //        return RedirectToAction(nameof(Index));
//        //    }
//        //    catch
//        //    {
//        //        return View();
//        //    }
//        //}

//        [HttpPost]
//        public async Task<IActionResult> DeletePosition(int id)
//        {
//            if (id <= 0)
//            {
//                return BadRequest("invalid position id");
//            }
//            var result = await _positionService.Delete(id);
//            if (result.IsSuccess)
//            {
//                return RedirectToAction("GetPositions");
//            }
//            ModelState.AddModelError("", result.Error);
//            // return RedirectToAction("GetPositions");
//            return View();
//        }

//        [HttpGet]
//        public async Task<IActionResult> UpdatePosition(int id)
//        {
//            //var poisiton = await _positionService.GetById(id);

//            //if (poisiton == null)
//            //{
//            //    return NotFound();
//            //}

//            //return View(poisiton);

//            if (id <= 0)
//            {
//                return NotFound();
//            }
//            var result = await _positionService.GetById(id);
//            var positionDTO = result.Value;
//            if (positionDTO == null)
//            {
//                return NotFound();
//            }
//            return View(positionDTO);
//        }

//        [HttpPost]
//        public async Task<IActionResult> UpdatePosition(int id,PositionDTO positionDTO)
//        {

//            //if (id != positionDTO.PositionId)
//            //{
//            //    return NotFound();
//            //}

//            //if (ModelState.IsValid)
//            //{
//            //    await _positionService.Update(id, positionDTO);
//            //    return RedirectToAction(nameof(Index));
//            //}
//            //return View(positionDTO);


//            if (ModelState.IsValid)
//            {
//                var result = await _positionService.Update(positionDTO);

//                if (result.IsSuccess)
//                {
//                    return RedirectToAction("GetPositions");
//                }
//                ModelState.AddModelError("", result.Error);
//                // return RedirectToAction("GetPositions");
//                return View(positionDTO);
//            }
//            else
//            {
//                ModelState.AddModelError("", $"the model state is not valid");
//            }
//            return View(positionDTO);

//        }


//        [HttpGet]
//        public async Task<IActionResult> ShowCompanies(int id)
//        {

//            var result = await _positionService.GetById(id);
//            if (result.IsSuccess)
//            {
//                var positionDTO = result.Value;
//                return View(positionDTO);
//            }


//            else
//            {
//                ModelState.AddModelError("", result.Error);
//                return View();
//            }
//        }




//    }
//}
