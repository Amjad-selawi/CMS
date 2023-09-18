using CMS.Application.DTOs;
using CMS.Application.Extensions;
using CMS.Domain;
using CMS.Domain.Entities;
using CMS.Domain.Enums;
using CMS.Repository.Implementation;
using CMS.Repository.Interfaces;
using CMS.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMS.Services.Services
{
    public class NotificationsService : INotificationsService
    {
        private readonly INotificationsRepository _notificationsRepository;
        private readonly ITemplatesService _templatesService;
        private readonly ApplicationDbContext _dbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<IdentityUser> _userManager;

        public NotificationsService
            (
            INotificationsRepository notificationsRepository,
            ITemplatesService templatesService,
            ApplicationDbContext dbContext,
            IHttpContextAccessor httpContextAccessor,
            RoleManager<IdentityRole> roleManager,
             UserManager<IdentityUser> userManager
            )
        {
            _notificationsRepository = notificationsRepository;
            _templatesService = templatesService;
            _dbContext = dbContext;
            _httpContextAccessor = httpContextAccessor;
            _roleManager = roleManager;
            _userManager = userManager;
        }

        public async Task<IEnumerable<NotificationsDTO>> GetAllNotificationsAsync()
        {
            var notifications = await _notificationsRepository.GetAllNotifications();
                    
            return notifications.Select(i => new NotificationsDTO
            {
                NotificationsId = i.NotificationsId,
                SendDate= i.SendDate,
                ReceiverId = i.ReceiverId,
                IsReceived=i.IsReceived,
                Title = i.Title,
                BodyDesc= i.BodyDesc,
                IsRead = i.IsRead,
                //TemplateName = i.TemplateName.ToString(),

                //templatesDTO = new TemplatesDTO 
                //{
                //    //TemplatesId = i.TemplatesId,
                //    Title = i.Title,
                //    //Name = _templatesService.GetTemplateByIdAsync(i.TemplatesId).Result?.Name.ToString()
                //},

            });

          
            }

        public async Task<NotificationsDTO> GetNotificationByIdAsync(int notificationsId)
        {
            var notification = await _notificationsRepository.GetNotificationsById(notificationsId);
            if (notification == null)
                return null;

            var templatesDTO = new TemplatesDTO
            {
                Title = notification.Title,
                BodyDesc= notification.BodyDesc,
                //    TemplatesId = notification.TemplatesId,
                //    Title = _templatesService.GetTemplateByIdAsync(notification.TemplatesId).Result?.Title,
                //    //Name = _templatesService.GetTemplateByIdAsync(notification.TemplatesId).Result?.Name.ToString() 

            };


            return new NotificationsDTO
            {
                NotificationsId = notification.NotificationsId,
                SendDate = notification.SendDate,
                ReceiverId = notification.ReceiverId,
                IsReceived = notification.IsReceived,
                IsRead=notification.IsRead,
                //templatesDTO = templatesDTO,
                Title = notification.Title,
                BodyDesc = notification.BodyDesc,
                //TemplateName = templatesDTO.Name

            };
        
            }

        public async Task<NotificationsDTO> GetNotificationByIdforDetails(int notificationsId)
        {
            var notification = await _notificationsRepository.GetNotificationsById(notificationsId);
            if (notification == null)
                return null;

      


            return new NotificationsDTO
            {
                NotificationsId = notification.NotificationsId,
                SendDate = notification.SendDate,
                ReceiverId = notification.ReceiverId,
                IsReceived = notification.IsReceived,
                IsRead = true,
                Title = notification.Title,
                BodyDesc = notification.BodyDesc,

            };

        }

        public async Task Create(NotificationsDTO entity)
        {
          
             
            
                var notification = new Notifications
                {
                    SendDate = DateTime.Now,
                    ReceiverId = entity.ReceiverId,
                    IsReceived = entity.IsReceived,
                    Title = entity.Title,
                    BodyDesc=entity.BodyDesc,
                    //TemplatesId = entity.templatesDTO.TemplatesId,
                    //TemplateName = entity.TemplateName.ToString()

                };
                await _notificationsRepository.Create(notification);
           
        }

        public async Task Update(int notificationId, NotificationsDTO entity)
        {
            
                var existingNotification = await _notificationsRepository.GetNotificationsById(notificationId);
                if (existingNotification == null)
                    throw new Exception("Notifications not found");

                existingNotification.SendDate = DateTime.Now;
                existingNotification.ReceiverId = entity.ReceiverId;
                existingNotification.IsReceived = entity.IsReceived;
            existingNotification.Title = entity.Title;
            existingNotification.BodyDesc = entity.BodyDesc;
            existingNotification.IsRead = entity.IsRead;
            //existingNotification.TemplatesId = entity.templatesDTO.TemplatesId;
            //existingNotification.TemplateName = entity.templatesDTO.Name;

            await _notificationsRepository.Update(existingNotification);
           
        }

        public async Task Delete(int notificationId)
        {
            var notification = await _notificationsRepository.GetNotificationsById(notificationId);
            if (notification != null)
                await _notificationsRepository.Delete(notification);
        }



        //public async Task SendNotification(NotificationsDTO notificationDTO)
        //{
        //    if (notificationDTO != null && notificationDTO.templatesDTO != null)
        //    {
        //        // Convert DTO to Entity
        //        var notification = new Notifications
        //        {
        //            ReceiverId = notificationDTO.ReceiverId,
        //            SendDate = notificationDTO.SendDate,
        //            IsReceived = notificationDTO.IsReceived,
        //            TemplatesId = notificationDTO.templatesDTO.TemplatesId,
        //        };

        //        _dbContext.Notifications.Add(notification);
        //        await _dbContext.SaveChangesAsync();
        //    }
        //    else
        //    {
        //        throw new Exception("NotificationDTO or TemplatesDTO is null.");
        //    }
        //}



        public async Task<List<NotificationsDTO>> GetNotificationsForUserAsync(string userId)
        {

            var notifications = await _notificationsRepository.GetSpacificNotificationsforHR();

            var notificationsDTOList = notifications
         .Where(notification => notification.ReceiverId == "7d86a8a0-2015-4348-bbef-089c8ba7706f") //hrId
         .Select(notification => new NotificationsDTO
         {
             IsReceived = true,
             SendDate = DateTime.Now,
             Title = notification.Title,
             
         })
         .ToList();

            return notificationsDTOList;


        }




        public async Task<IEnumerable<NotificationsDTO>> GetNotificationsForHRAsync()
        {

            var HrId = "";

            var Hr = await _roleManager.FindByNameAsync("HR");

            HrId = (await _userManager.GetUsersInRoleAsync(Hr.Name)).FirstOrDefault().Id;


            var notifications = await _notificationsRepository.GetSpacificNotificationsforHR();
            var notificationsDTOList = notifications
                .Where(notification => notification.ReceiverId == HrId)//hrId
                .Select(notification => new NotificationsDTO
                {
                    NotificationsId = notification.NotificationsId,
                    SendDate = notification.SendDate,
                    Title = notification.Title,
                    BodyDesc = notification.BodyDesc,
                    IsReceived = true
                })
                .ToList();

            return notificationsDTOList;
        }




        public async Task<IEnumerable<NotificationsDTO>> GetNotificationsForInterviewers()
        {

            var interviewerId = "";

            var interviewer = await _roleManager.FindByNameAsync("Interviewer");

            interviewerId = (await _userManager.GetUsersInRoleAsync(interviewer.Name)).FirstOrDefault().Id;



            var notifications = await _notificationsRepository.GetSpacificNotificationsforInterviewer();
            var notificationsDTOList = notifications
                .Where(notification => notification.ReceiverId == interviewerId)//InterviewerId
                .Select(notification => new NotificationsDTO
                {
                    NotificationsId = notification.NotificationsId,
                    SendDate = notification.SendDate,
                    Title = notification.Title,
                    BodyDesc = notification.BodyDesc,
                    IsReceived = true
                })
                .ToList();

            return notificationsDTOList;
        }




        public async Task<IEnumerable<NotificationsDTO>> GetNotificationsForGeneralManager()
        {
            var managerId = "";

            var manager = await _roleManager.FindByNameAsync("General Manger");

            managerId = (await _userManager.GetUsersInRoleAsync(manager.Name)).FirstOrDefault().Id;



            var notifications = await _notificationsRepository.GetSpacificNotificationsforGeneral();
            var notificationsDTOList = notifications
                .Where(notification => notification.ReceiverId == managerId)//GMId
                .Select(notification => new NotificationsDTO
                {
                    NotificationsId = notification.NotificationsId,
                    SendDate = notification.SendDate,
                    Title = notification.Title,
                    BodyDesc = notification.BodyDesc,
                    IsReceived = true
                })
                .ToList();

            return notificationsDTOList;
        }



        public async Task CreateNotificationForGeneralManagerAsync(int status, string notes)
        {
            var managerId = "";

            var manager = await _roleManager.FindByNameAsync("General Manger");

            managerId = (await _userManager.GetUsersInRoleAsync(manager.Name)).FirstOrDefault().Id;



            string userName = GetLoggedInUserName(); 


            // Create the notification.
            var notification = new Notifications
            {
                SendDate = DateTime.Now,
                ReceiverId = managerId,
                IsReceived = true,
                IsRead = false,
                Title = status == 7 ? $"You have a Second Interview" : $"The First Interview Rejected by {userName}",
                BodyDesc = notes
            };

            await _notificationsRepository.Create(notification);
        }





        public async Task CreateInterviewNotificationForInterviewerAsync(DateTime interviewDate)
        {

            var interviewerId = "";

            var interviewer = await _roleManager.FindByNameAsync("Interviewer");

            interviewerId = (await _userManager.GetUsersInRoleAsync(interviewer.Name)).FirstOrDefault().Id;

            var notification = new Notifications
            {
                ReceiverId = interviewerId,
                SendDate = DateTime.Now,
                IsReceived = true,
                IsRead = false,
                Title = "New interview",
                BodyDesc = $"You have an interview on {interviewDate}"
            };

            await _notificationsRepository.Create(notification);
        }




        public async Task CreateInterviewNotificationForHRInterview(int status, string notes)
        {

            var HrId = "";

            var Hr = await _roleManager.FindByNameAsync("HR");

            HrId = (await _userManager.GetUsersInRoleAsync(Hr.Name)).FirstOrDefault().Id;

            string userName = GetLoggedInUserName();

            var notification = new Notifications
            {
                ReceiverId = HrId,
                SendDate = DateTime.Now,
                IsReceived = true,
                IsRead = false,
                Title = status == 7 ? $"You have a Thierd Interview" : $"The Second Interview Rejected by {userName}",
                BodyDesc = notes
            };

            await _notificationsRepository.Create(notification);
        }



        public string GetLoggedInUserName()
        {
            return _httpContextAccessor.HttpContext.User.Identity.Name;
        }





    }



}

