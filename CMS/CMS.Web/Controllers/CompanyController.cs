using CMS.Application.DTOs;
using CMS.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace CMS.Web.Controllers
{
    public class CompanyController : Controller
    {
        private readonly ICompanyService _companyService;
        private readonly ICountryService _countryService;
     
        public CompanyController(ICompanyService companyService, ICountryService countryService)
        {
            _companyService = companyService;
            _countryService = countryService;
            
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> AddCompany()
        {

            var CountriesDTOs = await _countryService.GetAll();
            ViewBag.CountriesDTOs = new SelectList(CountriesDTOs.Value, "Id", "Name");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddCompany(CompanyDTO companyDTO)
        {

            var CountriesDTOs= await _countryService.GetAll();
            ViewBag.CountriesDTOs = new SelectList(CountriesDTOs.Value, "Id", "Name");
            if (ModelState.IsValid)
            {
                var result = await _companyService.Insert(companyDTO);

                if (result.IsSuccess)
                {
                    return RedirectToAction("GetCompanies");
                }

                ModelState.AddModelError("", result.Error);
            }
            else
            {
                ModelState.AddModelError("", "error validating the model");
            }

            return View(companyDTO);
        }
        [HttpGet]
        public async Task<IActionResult> GetCompanies()
        {
            var result = await _companyService.GetAll();
            if (result.IsSuccess)
            {
                var companiesDTOs = result.Value;
                return View(companiesDTOs);
            }
            else
            {
                ModelState.AddModelError("", result.Error);
                return View();
            }
        }
  

        [HttpPost]
        public async Task<IActionResult> DeleteCompany(int id)
        {
            if (id <= 0)
            {
                return BadRequest("invalid company id");
            }
            var result = await _companyService.Delete(id);
            if (result.IsSuccess)
            {
                return RedirectToAction("GetCompanies");
            }
            ModelState.AddModelError("", result.Error);
            //return RedirectToAction("GetCompanies");
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> UpdateCompany(int id)
        {
            if (id <= 0)
            {
                return NotFound();
            }
            var result = await _companyService.GetById(id);
            var companyDTO = result.Value;
            if (companyDTO == null)
            {
                return NotFound();
            }
            var CountriesDTOs = await _countryService.GetAll();
            ViewBag.CountriesDTOs = new SelectList(CountriesDTOs.Value, "Id", "Name");
            return View(companyDTO);

        }

        [HttpPost]
        public async Task<IActionResult> UpdateCompany(CompanyDTO companyDTO)
        {
            if(companyDTO == null)
            {
                ModelState.AddModelError("", $"the company dto you are trying to update is null ");
                return RedirectToAction("Index");
            }
            
            var CountriesDTOs = await _countryService.GetAll();
            ViewBag.CountriesDTOs = new SelectList(CountriesDTOs.Value, "Id", "Name");
            if (ModelState.IsValid)
            {
                var result = await _companyService.Update(companyDTO);

                if (result.IsSuccess)
                {
                    return RedirectToAction("GetCompanies");
                }
              
                ModelState.AddModelError("", result.Error);
                return View(companyDTO);
            }
            else
            {
                ModelState.AddModelError("", $"the model state is not valid");
            }
            return View(companyDTO);

        }
    }
}
