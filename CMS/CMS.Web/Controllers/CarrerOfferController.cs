using CMS.Application.DTOs;
using CMS.Domain;
using CMS.Domain.Entities;
using CMS.Services.Interfaces;
using CMS.Services.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace CMS.Web.Controllers
{
    public class CarrerOfferController : Controller
    {
        private readonly ICarrerOfferService _carrerOfferService;
        private readonly IPositionService _positionService;
        

        public CarrerOfferController(ICarrerOfferService carrerOfferService,IPositionService positionService)
        {
            _carrerOfferService = carrerOfferService;
            _positionService = positionService;
          
        }

        public async Task<IActionResult> Index(CarrerOfferDTO crrerOfferDTO)
        {
            if(User.IsInRole("None"))
            {

            
            var result = await _carrerOfferService.GetAll();
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
        public async Task<IActionResult> Details(int id)
        {
            if (User.IsInRole("None"))
            {
                var result = await _carrerOfferService.GetById(id);

                var PositionsDTOs = await _positionService.GetAll();
                ViewBag.positionDTOs = new SelectList(PositionsDTOs.Value, "PositionId", "Name");

                if (result.IsSuccess)
                {
                    var positionDTO = result.Value;
                    return View(positionDTO);
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


        public async Task<IActionResult> Create()
        {
            if (User.IsInRole("None"))
            {
                var PositionsDTOs = await _positionService.GetAll();
                ViewBag.positionDTOs = new SelectList(PositionsDTOs.Value, "PositionId", "Name");
                return View();
            }
            else
            {
                return View("AccessDenied");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CarrerOfferDTO carrerOfferDTO)
        {

            var positionDTOs = await _positionService.GetAll();
            ViewBag.positionDTOs = new SelectList(positionDTOs.Value, "PositionId", "Name");
            if (ModelState.IsValid)
            {
                var result = await _carrerOfferService.Insert(carrerOfferDTO);

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

            return View(carrerOfferDTO);
        }

        public async Task<IActionResult> Edit(int id)
        {
            if (User.IsInRole("None"))
            {
                if (id <= 0)
            {
                return NotFound();
            }
            var result = await _carrerOfferService.GetById(id);
            var positionDTO = result.Value;
            if (positionDTO == null)
            {
                return NotFound();
            }
            var PositionsDTOs = await _positionService.GetAll();
            ViewBag.positionDTOs = new SelectList(PositionsDTOs.Value, "PositionId", "Name");
            return View(positionDTO);
            }
            else
            {
                return View("AccessDenied");
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CarrerOfferDTO carrerOfferDTO)
        {
            if (carrerOfferDTO == null)
            {
                ModelState.AddModelError("", $"the career offer dto you are trying to update is null ");
                return RedirectToAction("Index");
            }

            var PositionsDTOs = await _positionService.GetAll();
            ViewBag.positionDTOs = new SelectList(PositionsDTOs.Value, "PositionId", "Name");
            if (ModelState.IsValid)
            {
                var result = await _carrerOfferService.Update(carrerOfferDTO);

                if (result.IsSuccess)
                {
                    return RedirectToAction("Index");
                }

                ModelState.AddModelError("", result.Error);
                return View(carrerOfferDTO);
            }
            else
            {
                ModelState.AddModelError("", $"the model state is not valid");
            }
            return View(carrerOfferDTO);
        }

        public async Task<IActionResult> Delete(int id)
        {
            //var carrerOffer = await _carrerOfferService.GetById(id);
            //if (carrerOffer == null)
            //{
            //    return NotFound();
            //}
            //return View(carrerOffer);.
            if (User.IsInRole("None"))
            {
                var result = await _carrerOfferService.GetById(id);
            if (result.IsSuccess)
            {
                var positionDTO = result.Value;
                return View(positionDTO);
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

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (id <= 0)
            {
                return BadRequest("invalid career offer id");
            }
            var result = await _carrerOfferService.Delete(id);
            if (result.IsSuccess)
            {
                return RedirectToAction("Index");
            }
            ModelState.AddModelError("", result.Error);
            return View();
        }











        //public async Task<IActionResult> Index()
        //{
        //    var carrerOffers = await _carrerOfferService.GetAllCarrerOffersAsync();
        //    return View(carrerOffers);
        //}
        //public async Task<IActionResult> Details(int id)
        //{
        //    var carrerOffer = await _carrerOfferService.GetCarrerOfferByIdAsync(id);
        //    if (carrerOffer == null)
        //    {
        //        return NotFound();
        //    }

        //    var position = await _positionService.GetAll();
        //    ViewBag.positionList = new SelectList(position, "PositionId", "Name", carrerOffer.Id);

        //    return View(carrerOffer);
        //}


        //public async Task<IActionResult> Create()
        //{
        //    var position = await _positionService.GetAll();
        //    ViewBag.positionList = new SelectList(position, "PositionId", "Name");

        //    return View();
        //}

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Create(CarrerOfferDTO carrerOfferDTO)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        await _carrerOfferService.CreateCarrerOfferAsync(carrerOfferDTO);
        //        return RedirectToAction(nameof(Index));
        //    }

        //    var position = await _positionService.GetAll();
        //    ViewBag.positionList = new SelectList(position, "PositionId", "Name");
        //    return View(carrerOfferDTO);
        //}

        //public async Task<IActionResult> Edit(int id)
        //{
        //    var carrerOffer = await _carrerOfferService.GetCarrerOfferByIdAsync(id);
        //    if (carrerOffer == null)
        //        return NotFound();


        //    var position = await _positionService.GetAll();
        //    ViewBag.positionList = new SelectList(position, "PositionId", "Name", carrerOffer.Id);

        //    return View(carrerOffer);
        //}
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Edit(int id, CarrerOfferDTO carrerOfferDTO)
        //{
        //    if (id != carrerOfferDTO.Id)
        //    {
        //        return NotFound();
        //    }

        //    var posi = await _positionService.GetById(id);

        //    if (!ModelState.IsValid)
        //    {
        //        await _carrerOfferService.UpdateCarrerOfferAsync(id, carrerOfferDTO);
        //        return RedirectToAction(nameof(Index));
        //    }


        //    var position = await _positionService.GetAll();
        //    ViewBag.positionList = new SelectList(position, "PositionId", "Name", posi.PositionId);


        //    return View(carrerOfferDTO);
        //}

        //public async Task<IActionResult> Delete(int id)
        //{
        //    var carrerOffer = await _carrerOfferService.GetCarrerOfferByIdAsync(id);
        //    if (carrerOffer == null)
        //    {
        //        return NotFound();
        //    }
        //    return View(carrerOffer);
        //}

        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> DeleteConfirmed(int id)
        //{
        //    await _carrerOfferService.DeleteCarrerOfferAsync(id);
        //    return RedirectToAction(nameof(Index));
        //}







        //public async Task<IActionResult> SubmitCareerOffer(string position)
        //{
        //    var userRole = "General Manager"; 

        //    if (userRole == "General Manager")
        //    {
        //        var hrRoleId = "5935a081-f473-42c6-9940-1da862c61a42"; 

        //       
        //        var notification = new Notifications
        //        {
        //            ReceiverId = hrRoleId, // Send to HR role
        //            Message = $"I need a new Employee with position {position}",
        //            SendDate = DateTime.Now
        //        };

        //       
        //        Db.Notifications.Add(notification);
        //        await Db.SaveChangesAsync();
        //    }

        //    // Continue with your career offer submission logic

        //    return RedirectToAction("Index", "Home");
        //}








    }
}
