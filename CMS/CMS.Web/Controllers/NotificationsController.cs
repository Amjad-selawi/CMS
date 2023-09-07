using CMS.Application.DTOs;
using CMS.Domain.Entities;
using CMS.Domain.Enums;
using CMS.Services.Interfaces;
using CMS.Services.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CMS.Web.Controllers
{
    public class NotificationsController : Controller
    {
        private readonly INotificationsService _notificationsService;
        private readonly ITemplatesService _templatesService;

        public NotificationsController(INotificationsService notificationsService, ITemplatesService templatesService)
        {
            _notificationsService = notificationsService;
            _templatesService = templatesService;
        }

        // GET: NotificationsController
        public async Task<ActionResult> Index()
        {
            var notifications = await _notificationsService.GetAllNotificationsAsync();
            return View(notifications);
        }

        // GET: NotificationsController/Details/5
        public async Task<ActionResult> Details(int id)
        {
            var notifications = await _notificationsService.GetNotificationByIdAsync(id);

            var templets = await _templatesService.GetAllTemplatesAsync();
            ViewBag.templetList = new SelectList(templets, "TemplatesId", "Title", notifications.NotificationsId);


            return View(notifications);
        }

        // GET: NotificationsController/Create
        public async Task<ActionResult> Create()
        {
            var templets = await _templatesService.GetAllTemplatesAsync();
            ViewBag.templetList = new SelectList(templets, "TemplatesId", "Title");

            var notificationtype = Enum.GetValues(typeof(NotificationsType))
          .Cast<NotificationsType>()
          .Select(name => new SelectListItem
          {
              Text = name.ToString(),
              Value = name.ToString()
          })
           .ToList();

            ViewBag.NotificationType = notificationtype;

            return View();
        }

        // POST: NotificationsController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(NotificationsDTO collection)
        {
            if (ModelState.IsValid)
            {
                await _notificationsService.Create(collection);
                return RedirectToAction(nameof(Index));
            }

            var templets = await _templatesService.GetAllTemplatesAsync();
            ViewBag.templetList = new SelectList(templets, "TemplatesId", "Title");


            var notificationtype = Enum.GetValues(typeof(NotificationsType))
          .Cast<NotificationsType>()
          .Select(name => new SelectListItem
          {
              Text = name.ToString(),
              Value = name.ToString()
          })
           .ToList();

            ViewBag.NotificationType = notificationtype;

            return View(collection);
        }

        // GET: NotificationsController/Edit/5
        public async Task<ActionResult> Edit(int id)
        {
            var notifications = await _notificationsService.GetNotificationByIdAsync(id);

            var templets = await _templatesService.GetAllTemplatesAsync();
            ViewBag.templetList = new SelectList(templets, "TemplatesId", "Title", notifications.NotificationsId);


            return View(notifications);
        }

        // POST: NotificationsController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(int id, NotificationsDTO collection)
        {
            if (id != collection.NotificationsId)
            {
                return NotFound();
            }

            var temp = await _templatesService.GetTemplateByIdAsync(id);
          
            if (ModelState.IsValid)
            {
                await _notificationsService.Update(id, collection);
                return RedirectToAction(nameof(Index));
            }
            var templets = await _templatesService.GetAllTemplatesAsync();
            ViewBag.templetList = new SelectList(templets, "TemplatesId", "Title", temp.TemplatesId);


            return View(collection);
        }



        // GET: NotificationsController/Delete/5
        public async Task<ActionResult> Delete(int id)
        {

            var notifications = await _notificationsService.GetNotificationByIdAsync(id);

            return View(notifications);
        }

        // POST: NotificationsController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(int id, NotificationsDTO collection)
        {
            try
            {
                await _notificationsService.Delete(id);
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }


    }
}

