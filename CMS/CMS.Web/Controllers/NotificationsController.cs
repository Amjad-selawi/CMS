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
        private readonly IHttpContextAccessor _httpContextAccessor;

        public NotificationsController
            (
            INotificationsService notificationsService,
            ITemplatesService templatesService,
            UserManager<IdentityUser> userManager,
            IHttpContextAccessor httpContextAccessor
            )
        {
            _notificationsService = notificationsService;
            _templatesService = templatesService;
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
        }

        public void LogException(string methodName, Exception ex, string additionalInfo = null)
        {
            
            _notificationsService.LogException(methodName, ex, additionalInfo);
        }
     

        // GET: NotificationsController
        public async Task<ActionResult> Index()
        {
            try
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
            catch (Exception ex)
            {
                LogException(nameof(Index), ex,"not working");
                throw ex;
            }
        }

        public async Task<ActionResult> IndexGMnotification()
        {
            try
            {
                var notifications = await _notificationsService.GetNotificationsForGeneralManager();
                return View(notifications);
            }
            catch (Exception ex)
            {
                LogException(nameof(IndexGMnotification), ex, "Faild to load GM notifiacations");
                throw ex;
            }
        }

        public async Task<ActionResult> IndexArchinotification()
        {
            try
            {
                var notifications = await _notificationsService.GetNotificationsForArchitecture();
                return View(notifications);
            }
            catch (Exception ex)
            {
                LogException(nameof(IndexArchinotification), ex, "Faild to load Archi notifiacations");
                throw ex;
            }
        }

        public async Task<ActionResult> IndexInterviewernotification()
        {
            try
            {
                var userId = _userManager.GetUserId(User);
                var notifications = await _notificationsService.GetNotificationsForInterviewers(userId);
                return View(notifications);
            }
            catch (Exception ex)
            {
                LogException(nameof(IndexInterviewernotification), ex, "Faild to load Interviewer notifiacations");
                throw ex;
            }
        }

        public async Task<ActionResult> IndexHRnotification()
        {
            try
            {
                var notifications = await _notificationsService.GetNotificationsForHRAsync();
                return View(notifications);
            }
            catch (Exception ex)
            {
                LogException(nameof(IndexHRnotification), ex, "Faild to load HR notifiacations");
                throw ex;
            }
        }

        // GET: NotificationsController/Details/5
        public async Task<ActionResult> Details(int id)
        {
            try
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
            catch (Exception ex)
            {
                LogException(nameof(Details), ex, $"Faild to load  ID: {id} details");
                throw ex;
            }
        }

        // GET: NotificationsController/Create
        public async Task<ActionResult> Create()
        {
            try
            {

            
            return View();
            }
            catch (Exception ex)
            {
                LogException(nameof(Create), ex,"not working");
                throw ex;
            }
        }

        // POST: NotificationsController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(NotificationsDTO collection)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    await _notificationsService.Create(collection);
                    return RedirectToAction(nameof(Index));
                }
                return View(collection);
            }
            catch (Exception ex)
            {
                LogException(nameof(Create), ex, "Faild to create a notification");
                throw ex;
            }
        }

        // GET: NotificationsController/Edit/5
        public async Task<ActionResult> Edit(int id)
        {
            try
            {
                var notifications = await _notificationsService.GetNotificationByIdAsync(id);
                return View(notifications);
            }
            catch (Exception ex)
            {
                LogException(nameof(Edit), ex,"not working");
                throw ex;
            }
        }

        // POST: NotificationsController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(int id, NotificationsDTO collection)
        {
            try
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
            catch (Exception ex)
            {
                LogException(nameof(Edit), ex, "Faild to edit a notification");
                throw ex;
            }
        }



        // GET: NotificationsController/Delete/5
        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                var notifications = await _notificationsService.GetNotificationByIdAsync(id);
                return View(notifications);
            }
            catch (Exception ex)
            {
                LogException(nameof(Delete), ex,"not working");
                throw ex;
            }
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
            catch (Exception ex)
            {
                LogException(nameof(Delete), ex, "Faild to delete a notification");
                throw ex;
            }
        }




        public async Task<IActionResult> GetNotificationsForHR()
        {
            try
            {
                var notifications = await _notificationsService.GetNotificationsForHRAsyncicon();
                return Json(notifications);
            }
            catch (Exception ex)
            {
                LogException(nameof(GetNotificationsForHR), ex, "Faild to get the notification for the HR");
                throw ex;
            }
        }

        public async Task<IActionResult> GetNotificationsForInterviewers()
        {
            try
            {
                var userId = _userManager.GetUserId(User);
                var notifications = await _notificationsService.GetNotificationsForInterviewersicon(userId);
                return Json(notifications);
            }
            catch (Exception ex)
            {
                LogException(nameof(GetNotificationsForInterviewers), ex, "Faild to get the notification for the Interviewers");
                throw ex;
            }
        }


        public async Task<IActionResult> GetNotificationsForGeneralManager()
        {
            try
            {
                var notifications = await _notificationsService.GetNotificationsForGeneralManagericon();
                return Json(notifications);
            }
            catch (Exception ex)
            {
                LogException(nameof(GetNotificationsForGeneralManager), ex, "Faild to get the notification for the GM");
                throw ex;
            }
        }

        public async Task<IActionResult> GetNotificationsForArchitecture()
        {
            try
            {
                var notifications = await _notificationsService.GetNotificationsForArchitectureicon();
                return Json(notifications);
            }
            catch (Exception ex)
            {
                LogException(nameof(GetNotificationsForArchitecture), ex, "Faild to get the notification for the Architecture");
                throw ex;
            }
        }


    }
}

