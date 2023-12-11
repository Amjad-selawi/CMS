using CMS.Application.DTOs;
using CMS.Services.Interfaces;
using CMS.Services.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace CMS.Web.Controllers
{
    public class CompanyController : Controller
    {
        private readonly ICompanyService _companyService;
        private readonly ICountryService _countryService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CompanyController(ICompanyService companyService, ICountryService countryService,IHttpContextAccessor httpContextAccessor)
        {
            _companyService = companyService;
            _countryService = countryService;
            _httpContextAccessor = httpContextAccessor;
        }

        public void LogException(string methodName, Exception ex, string additionalInfo = null)
        {
            
            _companyService.LogException(methodName, ex, additionalInfo);
        }
       
        public IActionResult Index()
        {
            try
            {
                return View();
            }
            catch (Exception ex)
            {
                LogException(nameof(Index), ex,"not working");
                throw ex;
            }
        }
        [HttpGet]
        public async Task<IActionResult> AddCompany()
        {
            try
            {
                

                var CountriesDTOs = await _countryService.GetAll();
                ViewBag.CountriesDTOs = new SelectList(CountriesDTOs.Value, "Id", "Name");

                return View();
            }
            catch (Exception ex)
            {
                LogException(nameof(AddCompany), ex,"not working");
                throw ex;
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddCompany(CompanyDTO companyDTO)
        {
            try
            {
                

                var CountriesDTOs = await _countryService.GetAll();
                ViewBag.CountriesDTOs = new SelectList(CountriesDTOs.Value, "Id", "Name");

                if (ModelState.IsValid)
                {
                    if (_companyService.DoesCompanyNameExist(companyDTO.Name, companyDTO.CountryId))
                    {
                        ModelState.AddModelError("Name", "A company with the same name already exists in the selected country.");
                        return View(companyDTO);
                    }

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
            catch (Exception ex)
            {
                LogException(nameof(AddCompany), ex, "Faild to create a company");
                throw ex;
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetCompanies()
        {
            try
            {
                

                if (User.IsInRole("Admin") || User.IsInRole("HR Manager"))
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
                else
                {
                    return View("AccessDenied");
                }
            }
            catch (Exception ex)
            {
                LogException(nameof(GetCompanies), ex, "Faild to load the company page");
                throw ex;
            }
        }

        [HttpGet]
        public async Task<IActionResult> ConfirmDelete(int id)
        {
            try
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

                return View(companyDTO);
            }
            catch (Exception ex)
            {
                LogException(nameof(ConfirmDelete), ex, $"Faild to load Company ID: {id} delete page");
                throw ex;
            }
        }

        [HttpGet]
        [HttpPost]
        public async Task<IActionResult> DeleteCompany(int id)
        {
            try
            {
                

                if (id <= 0)
                {
                    return BadRequest("Invalid company id");
                }

                if (HttpContext.Request.Method == "POST")
                {
                    var result = await _companyService.Delete(id);

                    if (result.IsSuccess)
                    {
                        return RedirectToAction("GetCompanies");
                    }

                    ModelState.AddModelError("", result.Error);
                }
                else
                {
                    return RedirectToAction("ConfirmDelete", new { id });
                }

                return View();
            }
            catch (Exception ex)
            {
                LogException(nameof(DeleteCompany), ex, $"Faild to delete Company ID: {id}");
                throw ex;
            }
        }

        [HttpGet]
        public async Task<IActionResult> UpdateCompany(int id)
        {
            try
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
            catch (Exception ex)
            {
                LogException(nameof(UpdateCompany), ex, $"Faild to load Company ID: {id} edit page");
                throw ex;
            }
        }

        [HttpPost]
        public async Task<IActionResult> UpdateCompany(CompanyDTO companyDTO)
        {
            try
            {
                

                if (companyDTO == null)
                {
                    ModelState.AddModelError("", $"The company DTO you are trying to update is null.");
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
                    ModelState.AddModelError("", $"The model state is not valid.");
                }

                return View(companyDTO);
            }
            catch (Exception ex)
            {
                LogException(nameof(UpdateCompany), ex, $"Faild to edit Company ID: {companyDTO?.Id}");
                throw ex;
            }
        }

        public async Task<ActionResult> Details(int id)
        {
            try
            {
                

                var result = await _companyService.GetById(id);

                if (result.IsSuccess)
                {
                    return View(result.Value);
                }
                else
                {
                    return NotFound();
                }
            }
            catch (Exception ex)
            {
                LogException(nameof(Details), ex, $"Faild to load Company ID: {id} details");
                throw ex;
            }
        }

        [HttpPost]
        public IActionResult CheckCompanyName([FromBody] CompanyDTO companyDTO)
        {
            try
            {
                if (companyDTO != null)
                {
                    bool exists = _companyService.DoesCompanyNameExist(companyDTO.Name, companyDTO.CountryId);
                    return Ok(new { exists });
                }

                return BadRequest("Invalid data");
            }
            catch (Exception ex)
            {
                LogException(nameof(CheckCompanyName), ex, "Faild to Check Company Name validation");
                throw ex;
            }
        }


    }
}
