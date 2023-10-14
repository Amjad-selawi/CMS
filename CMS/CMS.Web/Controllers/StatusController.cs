using CMS.Application.DTOs;
using CMS.Services.Interfaces;
using CMS.Services.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
namespace CMS.Web.Controllers
{
    public class StatusController:Controller
    {
        private readonly IStatusService _statusService;
        public StatusController(IStatusService statusService)
        {
            _statusService = statusService;
        }

        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Add(StatusDTO statusDTO)
        {
            if (ModelState.IsValid)
            {
                var result = await _statusService.Insert(statusDTO);

                if (result.IsSuccess)
                {
                    return RedirectToAction("Get");
                }

                ModelState.AddModelError("", result.Error);
            }
            else
            {
                ModelState.AddModelError("", "error validating the model");
            }

            return View(statusDTO);
        }
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var result = await _statusService.GetAll();
            if (result.IsSuccess)
            {
                var StatusDTOs = result.Value;

                return View(StatusDTOs);
            }
            else
            {
                ModelState.AddModelError("", result.Error);
                return View();
            }
        }
    }
}
