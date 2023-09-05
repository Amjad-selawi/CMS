using CMS.Application.DTOs;
using CMS.Services.Interfaces;
using CMS.Services.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace CMS.Web.Controllers
{
    public class CountryController : Controller
    {
        private readonly ICountryService _countryService;
        public CountryController(ICountryService countryService)
        {
            _countryService = countryService;
        }

        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public IActionResult AddCountry()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddCountry(CountryDTO countryDTO)
        {
            if (ModelState.IsValid)
            {
                var result = await _countryService.Insert(countryDTO);

                if (result.IsSuccess)
                {
                    return RedirectToAction("GetCountries");
                }

                ModelState.AddModelError("", result.Error);
            }
            else
            {
                ModelState.AddModelError("", "error validating the model");
            }

            return View(countryDTO);
        }
        [HttpGet]
        public async Task<IActionResult> GetCountries()
        {
            var result = await _countryService.GetAll();
            if (result.IsSuccess)
            {
                var CountriesDTOs = result.Value;
              
                return View(CountriesDTOs);
            }
            else
            {
                ModelState.AddModelError("", result.Error);
                return View();
            }
        }
        [HttpPost]
        public async Task<IActionResult> DeleteCountry(int id)
        {
            if (id <= 0)
            {
                return BadRequest("invalid country id");
            }
            var result = await _countryService.Delete(id);
            if (result.IsSuccess)
            {
                return RedirectToAction("GetCountries");
            }
            ModelState.AddModelError("", result.Error);
            // return RedirectToAction("GetCountries");
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> UpdateCountry(int id)
        {
            if (id <= 0)
            {
                return NotFound();
            }
            var result = await _countryService.GetById(id);
            var countryDTO = result.Value;
            if (countryDTO == null)
            {
                return NotFound();
            }
            return View(countryDTO);

        }

        [HttpPost]
        public async Task<IActionResult> UpdateCountry(CountryDTO countryDTO)
        {
            if (ModelState.IsValid)
            {
                var result = await _countryService.Update(countryDTO);

                if (result.IsSuccess)
                {
                    return RedirectToAction("GetCountries");
                }
                ModelState.AddModelError("", result.Error);
                return View(countryDTO);  
            }
            else
            {
                ModelState.AddModelError("", $"the model state is not valid");
            }
            return View(countryDTO);

        }
        [HttpGet]
        public async Task<IActionResult> ShowCompanies(int id)
        {

            var result = await _countryService.GetById(id);
            if (result.IsSuccess)
            {
                var countryDTO = result.Value;
                return View(countryDTO);
            }
           
           
             else
            {
                ModelState.AddModelError("", result.Error);
                return View();
            }
        }
    }
}
