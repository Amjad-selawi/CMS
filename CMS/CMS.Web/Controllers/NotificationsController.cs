using CMS.Application.DTOs;
using CMS.Domain.Entities;
using CMS.Domain.Enums;
using CMS.Services.Interfaces;
using CMS.Services.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.SignalR;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace CMS.Web.Controllers
{
    public class NotificationsController : Controller
    {
        private readonly INotificationsService _notificationsService;
        private readonly ITemplatesService _templatesService;
        private readonly UserManager<IdentityUser> _userManager;

        public NotificationsController
            (
            INotificationsService notificationsService,
            ITemplatesService templatesService,
            UserManager<IdentityUser> userManager
            )
        {
            _notificationsService = notificationsService;
            _templatesService = templatesService;
            _userManager = userManager;
        }



        // GET: NotificationsController
        public async Task<ActionResult> Index()
        {
            if (User.IsInRole("Admin") || User.IsInRole("HR Manager"))
            {
                var notifications = await _notificationsService.GetAllNotificationsAsync();
                return View(notifications);
            }
            else
            {
                return View("AccessDenied");
            }
        }

        public async Task<ActionResult> IndexGMnotification()
        {
            var notifications = await _notificationsService.GetNotificationsForGeneralManager();
            return View(notifications);
        }

        public async Task<ActionResult> IndexInterviewernotification()
        {
            var userId = _userManager.GetUserId(User);

            var notifications = await _notificationsService.GetNotificationsForInterviewers(userId);

            return View(notifications);
        }

        public async Task<ActionResult> IndexHRnotification()
        {
            var notifications = await _notificationsService.GetNotificationsForHRAsync();
            return View(notifications);
        }

        // GET: NotificationsController/Details/5
        public async Task<ActionResult> Details(int id)
        {
            var notifications = await _notificationsService.GetNotificationByIdforDetails(id);

            if (notifications != null)
            {
                notifications.IsRead = true;

                await _notificationsService.Update(id, notifications);
                return View(notifications);
            }
            else
            {
                return NotFound();
            }
        }

        // GET: NotificationsController/Create
        public async Task<ActionResult> Create()
        {
         
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


            return View(collection);
        }

        // GET: NotificationsController/Edit/5
        public async Task<ActionResult> Edit(int id)
        {
            var notifications = await _notificationsService.GetNotificationByIdAsync(id);

    
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




        public async Task<IActionResult> GetNotificationsForHR()
        {
            var notifications = await _notificationsService.GetNotificationsForHRAsync();
            return Json(notifications);
        }

        public async Task<IActionResult> GetNotificationsForInterviewers()
        {
            var userId = _userManager.GetUserId(User);

            var notifications = await _notificationsService.GetNotificationsForInterviewers(userId);

            return Json(notifications);
        }

        public async Task<IActionResult> GetNotificationsForGeneralManager()
        {
            var notifications = await _notificationsService.GetNotificationsForGeneralManager();
            return Json(notifications);
        }




    }
}

