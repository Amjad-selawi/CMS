﻿using CMS.Application.DTOs;
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
        private readonly ICarrerOfferRepository _carrerOfferRepository;
        private readonly ICandidateService _candidateService;
        private readonly IPositionService _positionService;

        public NotificationsService
            (
            INotificationsRepository notificationsRepository,
            ITemplatesService templatesService,
            ApplicationDbContext dbContext,
            IHttpContextAccessor httpContextAccessor,
            RoleManager<IdentityRole> roleManager,
             UserManager<IdentityUser> userManager,
            ICarrerOfferRepository carrerOfferRepository,
            ICandidateService candidateService,
            IPositionService positionService
            )
        {
            _notificationsRepository = notificationsRepository;
            _templatesService = templatesService;
            _dbContext = dbContext;
            _httpContextAccessor = httpContextAccessor;
            _roleManager = roleManager;
            _userManager = userManager;
            _carrerOfferRepository = carrerOfferRepository;
            _candidateService = candidateService;
            _positionService = positionService;
        }

        public async Task<IEnumerable<NotificationsDTO>> GetAllNotificationsAsync()
        {
            var notifications = await _notificationsRepository.GetAllNotifications();

            return notifications.Select(i => new NotificationsDTO
            {
                NotificationsId = i.NotificationsId,
                SendDate = i.SendDate,
                ReceiverId = i.ReceiverId,
                IsReceived = i.IsReceived,
                Title = i.Title,
                BodyDesc = i.BodyDesc,
                IsRead = i.IsRead,
               

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
                BodyDesc = notification.BodyDesc,
              
            };


            return new NotificationsDTO
            {
                NotificationsId = notification.NotificationsId,
                SendDate = notification.SendDate,
                ReceiverId = notification.ReceiverId,
                IsReceived = notification.IsReceived,
                IsRead = notification.IsRead,
                Title = notification.Title,
                BodyDesc = notification.BodyDesc,

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


            var currentUser = await _userManager.GetUserAsync(_httpContextAccessor.HttpContext.User);
            var notification = new Notifications
            {
                SendDate = DateTime.Now,
                ReceiverId = entity.ReceiverId,
                IsReceived = entity.IsReceived,
                Title = entity.Title,
                BodyDesc = entity.BodyDesc,
                CreatedOn = DateTime.Now,
                CreatedBy=currentUser.Id,
            

            };
            await _notificationsRepository.Create(notification);

        }

        public async Task Update(int notificationId, NotificationsDTO entity)
        {

            var existingNotification = await _notificationsRepository.GetNotificationsById(notificationId);
            if (existingNotification == null)
                throw new Exception("Notifications not found");
            var currentUser = await _userManager.GetUserAsync(_httpContextAccessor.HttpContext.User);
            existingNotification.SendDate = entity.SendDate;
            existingNotification.ReceiverId = entity.ReceiverId;
            existingNotification.IsReceived = entity.IsReceived;
            existingNotification.Title = entity.Title;
            existingNotification.BodyDesc = entity.BodyDesc;
            existingNotification.IsRead = entity.IsRead;
            existingNotification.ModifiedOn = DateTime.Now;
            existingNotification.ModifiedBy=currentUser.Id;
        

            await _notificationsRepository.Update(existingNotification);

        }

        public async Task Delete(int notificationId)
        {
            var notification = await _notificationsRepository.GetNotificationsById(notificationId);
            if (notification != null)
                await _notificationsRepository.Delete(notification);
        }




        public async Task<List<NotificationsDTO>> GetNotificationsForUserAsync(string userId)
        {
            var HrId = "";

            var Hr = await _roleManager.FindByNameAsync("HR Manager");

            HrId = (await _userManager.GetUsersInRoleAsync(Hr.Name)).FirstOrDefault().Id;


            var notifications = await _notificationsRepository.GetSpacificNotificationsforHR();

            var notificationsDTOList = notifications
         .Where(notification => notification.ReceiverId == HrId) //hrId
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

            var Hr = await _roleManager.FindByNameAsync("HR Manager");

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

            var manager = await _roleManager.FindByNameAsync("General Manager");

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



        public async Task CreateNotificationForGeneralManagerAsync(int status, string notes, int CandidateId, int positionId)
        {
            var managerId = "";
            var HrId = "";

            var manager = await _roleManager.FindByNameAsync("General Manager");
            managerId = (await _userManager.GetUsersInRoleAsync(manager.Name)).FirstOrDefault().Id;

            var Hr = await _roleManager.FindByNameAsync("HR Manager");
            HrId = (await _userManager.GetUsersInRoleAsync(Hr.Name)).FirstOrDefault().Id;

            string userName = GetLoggedInUserName();
            var currentUser = await _userManager.GetUserAsync(_httpContextAccessor.HttpContext.User);

            var candidateName = await GetCandidateName(CandidateId);
            var positionName = await GetPositionName(positionId);

            // Create the notification for the manager.
            var notification = new Notifications
            {
                SendDate = DateTime.Now,
                
                IsReceived = true,
                IsRead = false,
                Title = "",
                BodyDesc = notes,
                CreatedBy = currentUser.Id,
                CreatedOn = DateTime.Now
            };

            if (status == 2)
            {
                notification.Title = $"You have a Second Interview with {candidateName} for the {positionName} position. Get ready to shine! 💼🚀";
                notification.ReceiverId = managerId;
            }
            else
            {
                notification.Title = $"{candidateName} Rejected by {userName} for position {positionName}";
                notification.ReceiverId = HrId;
            }

            await _notificationsRepository.Create(notification);

          
            if (status == 2)  
            {
                var hrNotification = new Notifications
                {
                    ReceiverId = HrId,
                    SendDate = DateTime.Now,
                    IsReceived = true,
                    IsRead = false,
                    Title = $"{candidateName} Approved by {userName} for position {positionName}",
                    BodyDesc = $"The candidate has been approved by the {userName} for the {positionName} position.",
                    CreatedBy = currentUser.Id,
                    CreatedOn = DateTime.Now
                };

                await _notificationsRepository.Create(hrNotification);
            }
        }





        public async Task CreateInterviewNotificationForInterviewerAsync(DateTime interviewDate , int CandidateId ,int positionId)
        {

            var interviewerId = "";

            var interviewer = await _roleManager.FindByNameAsync("Interviewer");

            interviewerId = (await _userManager.GetUsersInRoleAsync(interviewer.Name)).FirstOrDefault().Id;
            var currentUser = await _userManager.GetUserAsync(_httpContextAccessor.HttpContext.User);

            var candidateName = await GetCandidateName(CandidateId);
            var positionName = await GetPositionName(positionId);

            var notification = new Notifications
            {
                ReceiverId = interviewerId,
                SendDate = DateTime.Now,
                IsReceived = true,
                IsRead = false,
                Title = $"New interview invitation for {candidateName}",
                BodyDesc = $"You've been selected for a First Interview with {candidateName} for the {positionName} position on {interviewDate}. Get ready to shine! 💼🚀",
                CreatedBy=currentUser.Id,
                CreatedOn=DateTime.Now
            };

            await _notificationsRepository.Create(notification);
        }




        public async Task CreateInterviewNotificationForHRInterview(int status, string notes, int CandidateId , int positionId)
        {

            var HrId = "";

            var Hr = await _roleManager.FindByNameAsync("HR Manager");

            HrId = (await _userManager.GetUsersInRoleAsync(Hr.Name)).FirstOrDefault().Id;

            string userName = GetLoggedInUserName();
            var currentUser = await _userManager.GetUserAsync(_httpContextAccessor.HttpContext.User);

            var candidateName = await GetCandidateName(CandidateId);
            var positionName = await GetPositionName(positionId);

            var notification = new Notifications
            {
                ReceiverId = HrId,
                SendDate = DateTime.Now,
                IsReceived = true,
                IsRead = false,
                Title = "",
                BodyDesc = notes,
                CreatedOn = DateTime.Now,
                CreatedBy =currentUser.Id,
            };

            if (status == 2)
            {
                notification.Title = $"You have a Third Interview with {candidateName} for the {positionName} position. Get ready to shine! 💼🚀 ";
            }
            else
            {
                notification.Title = $"{candidateName} Rejected by {userName} for position {positionName}";
            }

            await _notificationsRepository.Create(notification);
        }



        public string GetLoggedInUserName()
        {
            return _httpContextAccessor.HttpContext.User.Identity.Name;
        }



        public async Task<string> GetCandidateName(int candidateId)
        {
            var candidate = await _candidateService.GetCandidateByIdAsync(candidateId);

            if (candidate != null)
            {
                return candidate.FullName;
            }

            return "Candidate Not Found";
        }



        public async Task<string> GetPositionName(int positionId)
        {
            var result = await _positionService.GetById(positionId);

            if (result.IsSuccess)
            {
                var position = result.Value;
                return position.Name;
            }

            return "Position Not Found";
        }






    }



}

