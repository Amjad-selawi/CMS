using CMS.Application.DTOs;
using CMS.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace CMS.Web.Controllers
{
    public class CarrerOfferController : Controller
    {
        private readonly ICarrerOfferService _carrerOfferService;

        public CarrerOfferController(ICarrerOfferService carrerOfferService)
        {
            _carrerOfferService = carrerOfferService;
        }

        public async Task<IActionResult> Index()
        {
            var carrerOffers = await _carrerOfferService.GetAllCarrerOffersAsync();
            return View(carrerOffers);
        }
        public async Task<IActionResult> Details(int id)
        {
            var carrerOffer = await _carrerOfferService.GetCarrerOfferByIdAsync(id);
            if (carrerOffer == null)
            {
                return NotFound();
            }
            return View(carrerOffer);
        }


        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CarrerOfferDTO carrerOfferDTO)
        {
            if (ModelState.IsValid)
            {
                await _carrerOfferService.CreateCarrerOfferAsync(carrerOfferDTO);
                return RedirectToAction(nameof(Index));
            }
            return View(carrerOfferDTO);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var carrerOffer = await _carrerOfferService.GetCarrerOfferByIdAsync(id);
            if (carrerOffer == null)
                return NotFound();

            return View(carrerOffer);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CarrerOfferDTO carrerOfferDTO)
        {
            if (id != carrerOfferDTO.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                await _carrerOfferService.UpdateCarrerOfferAsync(id, carrerOfferDTO);
                return RedirectToAction(nameof(Index));
            }
            return View(carrerOfferDTO);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var carrerOffer = await _carrerOfferService.GetCarrerOfferByIdAsync(id);
            if (carrerOffer == null)
            {
                return NotFound();
            }
            return View(carrerOffer);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _carrerOfferService.DeleteCarrerOfferAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
