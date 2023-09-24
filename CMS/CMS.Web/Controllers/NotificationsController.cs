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
            var notifications = await _notificationsService.GetAllNotificationsAsync();
            return View(notifications);
        }

        public async Task<ActionResult> IndexGMnotification()
        {
            var notifications = await _notificationsService.GetNotificationsForGeneralManager();
            return View(notifications);
        }

        public async Task<ActionResult> IndexInterviewernotification()
        {
            var notifications = await _notificationsService.GetNotificationsForInterviewers();
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

            var templets = await _templatesService.GetAllTemplatesAsync();
            ViewBag.templetList = new SelectList(templets, "TemplatesId", "Title", notifications.NotificationsId);

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
            var templets = await _templatesService.GetAllTemplatesAsync();
            ViewBag.templetList = new SelectList(templets, "TemplatesId", "Title");



            //var enumSelectList = new SelectList(Enum.GetValues(typeof(TemplatesName)).Cast<TemplatesName>().Select(e => new SelectListItem
            //{
            //    Text = e.ToString(),
            //    Value = ((int)e).ToString(),
            //}), "Value", "Text");

            //ViewBag.TemplateEnumList = enumSelectList;

            //var notificationsDto = new NotificationsDTO();

            //return View(notificationsDto);
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




            //var enumSelectList = new SelectList(Enum.GetValues(typeof(TemplatesName)).Cast<TemplatesName>().Select(e => new SelectListItem
            //{
            //    Text = e.ToString(),
            //    Value = ((int)e).ToString()
            //}), "Value", "Text");

            //ViewBag.TemplateEnumList = enumSelectList;





            return View(collection);
        }

        // GET: NotificationsController/Edit/5
        public async Task<ActionResult> Edit(int id)
        {
            var notifications = await _notificationsService.GetNotificationByIdAsync(id);

            var templets = await _templatesService.GetAllTemplatesAsync();
            ViewBag.templetList = new SelectList(templets, "TemplatesId", "Title", notifications.NotificationsId);


            //var enumSelectList = new SelectList(Enum.GetValues(typeof(TemplatesName)).Cast<TemplatesName>().Select(e => new SelectListItem
            //{
            //    Text = e.ToString(),
            //    Value = ((int)e).ToString(),
            //}), "Value", "Text");

            //ViewBag.TemplateEnumList = enumSelectList;

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



            //var enumSelectList = new SelectList(Enum.GetValues(typeof(TemplatesName)).Cast<TemplatesName>().Select(e => new SelectListItem
            //{
            //    Text = e.ToString(),
            //    Value = ((int)e).ToString(),
            //}), "Value", "Text");

            //ViewBag.TemplateEnumList = enumSelectList;

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
            var notifications = await _notificationsService.GetNotificationsForInterviewers();
            return Json(notifications);
        }

        public async Task<IActionResult> GetNotificationsForGeneralManager()
        {
            var notifications = await _notificationsService.GetNotificationsForGeneralManager();
            return Json(notifications);
        }




    }
}

