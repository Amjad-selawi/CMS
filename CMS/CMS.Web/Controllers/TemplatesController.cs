using CMS.Application.DTOs;
using CMS.Domain.Entities;
using CMS.Domain.Enums;
using CMS.Services.Interfaces;
using CMS.Services.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace CMS.Web.Controllers
{
    public class TemplatesController : Controller
    {
        private readonly ITemplatesService _templatesService;

        public TemplatesController(ITemplatesService templatesService)
        {
            _templatesService = templatesService;
        }

        // GET: TemplatesController
        public async Task<ActionResult> Index()
        {
            var templates = await _templatesService.GetAllTemplatesAsync();
            return View(templates);
        }

        // GET: TemplatesController/Details/5
        public async Task<ActionResult> Details(int id)
        {
            var template = await _templatesService.GetTemplateByIdAsync(id);
            if (template == null)
            {
                return NotFound();
            }
            return View(template);
        }

        // GET: TemplatesController/Create
        public ActionResult Create()
        {
            var templatename = Enum.GetValues(typeof(TemplatesName))
           .Cast<TemplatesName>()
           .Select(name => new SelectListItem
           {
               Text = name.ToString(),
               Value = name.ToString()
           })
            .ToList();

            ViewBag.TemplateName = templatename;
            return View();
        }

        // POST: TemplatesController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(TemplatesDTO collection)
        {
            if (ModelState.IsValid)
            {
                await _templatesService.Create(collection);
                return RedirectToAction(nameof(Index));
            }


            var templatename = Enum.GetValues(typeof(TemplatesName))
           .Cast<TemplatesName>()
           .Select(name => new SelectListItem
           {
               Text = name.ToString(),
               Value = name.ToString()
           })
            .ToList();

            ViewBag.TemplateName = templatename;
            return View(collection);
        }

        // GET: TemplatesController/Edit/5
        public async Task<ActionResult> Edit(int id)
        {
            var template = await _templatesService.GetTemplateByIdAsync(id);
            if (template == null)
            {
                return NotFound();
            }
            return View(template);
        }

        // POST: TemplatesController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(int id, TemplatesDTO collection)
        {
            if (id != collection.TemplatesId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                await _templatesService.Update(id, collection);
                return RedirectToAction(nameof(Index));
            }
            return View(collection);
        }


        // GET: TemplatesController/Delete/5
        public async Task<ActionResult> Delete(int id)
        {
            var template = await _templatesService.GetTemplateByIdAsync(id);

            return View(template);
        }

        // POST: TemplatesController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(int id, TemplatesDTO collection)
        {
            try
            {
                await _templatesService.Delete(id);
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

    }
}
