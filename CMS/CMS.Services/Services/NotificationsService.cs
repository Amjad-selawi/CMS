using CMS.Application.DTOs;
using CMS.Application.Extensions;
using CMS.Domain;
using CMS.Domain.Entities;
using CMS.Domain.Enums;
using CMS.Repository.Implementation;
using CMS.Repository.Interfaces;
using CMS.Repository.Repositories;
using CMS.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
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
        private readonly IStatusService _statusService;

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
            IPositionService positionService,
            IStatusService statusService
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
            _statusService = statusService;
        }

        public void LogException(string methodName, Exception ex = null, string additionalInfo = null)
        {
            _notificationsRepository.LogException(methodName, ex,  additionalInfo);
        }


        public async Task<IEnumerable<NotificationsDTO>> GetAllNotificationsAsync()
        {
            try
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
            catch (Exception ex) {
                LogException(nameof(GetAllNotificationsAsync), ex,"Error while getting all Notifications");
                throw ex;
            }

        }

        public async Task<NotificationsDTO> GetNotificationByIdAsync(int notificationsId)
        {
            try
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
            catch (Exception ex)
            {

                LogException(nameof(GetNotificationByIdAsync),ex,null);
                throw ex;
            }
        }

        public async Task<NotificationsDTO> GetNotificationByIdforDetails(int notificationsId)
        {
            try
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

            catch (Exception ex)
            {

                LogException(nameof(GetNotificationByIdforDetails), ex, "GetNotificationByIdforDetails not working");
                throw ex;
            }
        }

        public async Task Create(NotificationsDTO entity)
        {

            try
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
                CreatedBy = currentUser.Id,
                IsRead=entity.IsRead,

            };
            await _notificationsRepository.Create(notification);
            }
            catch (Exception ex)
            {

                LogException(nameof(Create), ex, "Can not create Notification");
                throw ex;
            }
        }

        public async Task Update(int notificationId, NotificationsDTO entity)
        {
            try
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
            existingNotification.ModifiedBy = currentUser.Id;


            await _notificationsRepository.Update(existingNotification);
            }
            catch (Exception ex)
            {

                LogException(nameof(Update), ex, "Update Notifications not working");
                throw ex;
            }
        }

        public async Task Delete(int notificationId)
        {
            try
            {

                

                var notification = await _notificationsRepository.GetNotificationsById(notificationId);
            if (notification != null)
                await _notificationsRepository.Delete(notification);
            }
            catch (Exception ex)
            {

                LogException(nameof(Delete), ex, "Delete for Notification is not working");
                throw ex;
            }
        }

        public async Task<List<NotificationsDTO>> GetNotificationsForUserAsync(string userId)
        {
            try
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
            catch (Exception ex)
            {

                LogException(nameof(GetNotificationsForUserAsync), ex, "GetNotificationsForUserAsync not working");
                throw ex;
            }

        }

        public async Task<IEnumerable<NotificationsDTO>> GetNotificationsForHRAsync()
        {
            try
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
            catch (Exception ex)
            {

                LogException(nameof(GetNotificationsForHRAsync), ex, "GetNotificationsForHRAsync not working");
                throw ex;
            }
        }

        public async Task<IEnumerable<NotificationsDTO>> GetNotificationsForHRAsyncicon()
        {
            try
            {
                


                var HrId = "";

                var Hr = await _roleManager.FindByNameAsync("HR Manager");

                HrId = (await _userManager.GetUsersInRoleAsync(Hr.Name)).FirstOrDefault().Id;


                var notifications = await _notificationsRepository.GetSpacificNotificationsforHR();
                var notificationsDTOList = notifications
                    .Where(notification => notification.ReceiverId == HrId && notification.IsRead == false)//hrId
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
            catch (Exception ex)
            {

                LogException(nameof(GetNotificationsForHRAsyncicon), ex, "GetNotificationsForHRAsyncicon not working");
                throw ex;
            }
        }

        public async Task<IEnumerable<NotificationsDTO>> GetNotificationsForInterviewers(string interviewerId)
        {
            try
            {

                

                var notifications = await _notificationsRepository.GetSpacificNotificationsforInterviewer(interviewerId);
            var notificationsDTOList = notifications
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
            catch (Exception ex)
            {

                LogException(nameof(GetNotificationsForInterviewers), ex, "GetNotificationsForInterviewers not working");
                throw ex;
            }
        }

        public async Task<IEnumerable<NotificationsDTO>> GetNotificationsForInterviewersicon(string interviewerId)
        {
            try
            {
                


                var notifications = await _notificationsRepository.GetSpacificNotificationsforInterviewer(interviewerId);
                var notificationsDTOList = notifications.Where(notification => notification.IsRead == false)
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
            catch (Exception ex)
            {

                LogException(nameof(GetNotificationsForInterviewersicon), ex, "GetNotificationsForInterviewersicon not working");
                throw ex;
            }
        }
        public async Task<IEnumerable<NotificationsDTO>> GetNotificationsForGeneralManager()
        {
            try
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
            catch (Exception ex)
            {

                LogException(nameof(GetNotificationsForGeneralManager), ex, "GetNotificationsForGeneralManager not working");
                throw ex;
            }
        }

        public async Task<IEnumerable<NotificationsDTO>> GetNotificationsForGeneralManagericon()
        {
            try
            {
                


                var managerId = "";

                var manager = await _roleManager.FindByNameAsync("General Manager");

                managerId = (await _userManager.GetUsersInRoleAsync(manager.Name)).FirstOrDefault().Id;



                var notifications = await _notificationsRepository.GetSpacificNotificationsforGeneral();
                var notificationsDTOList = notifications
                    .Where(notification => notification.ReceiverId == managerId && notification.IsRead == false)//GMId
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
            catch (Exception ex)
            {

                LogException(nameof(GetNotificationsForGeneralManagericon), ex, "GetNotificationsForGeneralManagericon not working");
                throw ex;
            }
        }

        public async Task<IEnumerable<NotificationsDTO>> GetNotificationsForArchitecture()
        {
            try
            {
                


                var archiId = "";

                var archi = await _roleManager.FindByNameAsync("Solution Architecture");

                archiId = (await _userManager.GetUsersInRoleAsync(archi.Name)).FirstOrDefault().Id;



                var notifications = await _notificationsRepository.GetSpacificNotificationsforArchi();
                var notificationsDTOList = notifications
                    .Where(notification => notification.ReceiverId == archiId)//ArchiId
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
            catch (Exception ex)
            {

                LogException(nameof(GetNotificationsForArchitecture), ex, "GetNotificationsForArchitecture not working");
                throw ex;
            }
        }

        public async Task<IEnumerable<NotificationsDTO>> GetNotificationsForArchitectureicon()
        {
            try
            {
                


                var archiId = "";

                var archi = await _roleManager.FindByNameAsync("Solution Architecture");

                archiId = (await _userManager.GetUsersInRoleAsync(archi.Name)).FirstOrDefault().Id;



                var notifications = await _notificationsRepository.GetSpacificNotificationsforArchi();
                var notificationsDTOList = notifications
                    .Where(notification => notification.ReceiverId == archiId && notification.IsRead == false)//ArchiId
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
            catch (Exception ex)
            {

                LogException(nameof(GetNotificationsForArchitectureicon), ex, "GetNotificationsForArchitectureicon not working");
                throw ex;
            }
        }


        public async Task CreateNotificationForGeneralManagerAsync(int status, string notes, int CandidateId, int positionId)
        {
            try
            {
                


                var managerId = "";
                var HrId = "";

                var statusResult = await _statusService.GetById(status);
                var statusstatus = statusResult.Value;

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

                if (statusstatus.Code == Domain.Enums.StatusCode.Approved)
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


                if (statusstatus.Code == Domain.Enums.StatusCode.Approved)
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
            catch (Exception ex)
            {

                LogException(nameof(CreateNotificationForGeneralManagerAsync), ex,  null);
                throw ex;
            }
        }


        public async Task CreateNotificationForArchiAsync(int status, string notes, int CandidateId, int positionId)
        {
            try
            {

                

                var archiId = "";
                var HrId = "";

                var statusResult = await _statusService.GetById(status);
                var statusstatus = statusResult.Value;

                var archi = await _roleManager.FindByNameAsync("Solution Architecture");
                archiId = (await _userManager.GetUsersInRoleAsync(archi.Name)).FirstOrDefault().Id;

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

                if (statusstatus.Code == Domain.Enums.StatusCode.Approved)
                {
                    notification.Title = $"You and the GM have a Second Interview with {candidateName} for the {positionName} position. Get ready to shine! 💼🚀";
                    notification.ReceiverId = archiId;
                }
                else
                {
                    notification.Title = $"{candidateName} Rejected by {userName} for position {positionName}";
                    notification.ReceiverId = HrId;
                }

                await _notificationsRepository.Create(notification);


                if (statusstatus.Code == Domain.Enums.StatusCode.Approved)
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
            catch (Exception ex)
            {

                LogException(nameof(CreateNotificationForGeneralManagerAsync), ex, "CreateNotificationForGeneralManagerAsync not working");
                throw ex;
            }
        }

        public async Task CreateInterviewNotificationForInterviewerAsync(DateTime interviewDate, int candidateId, int positionId, string selectedInterviewerId, bool isCanceled)
        {
            try
            {
                


                var currentUser = await _userManager.GetUserAsync(_httpContextAccessor.HttpContext.User);
                var candidateName = await GetCandidateName(candidateId);
                var positionName = await GetPositionName(positionId);

                var notification = new Notifications
                {
                    ReceiverId = selectedInterviewerId,
                    SendDate = DateTime.Now,
                    IsReceived = true,
                    IsRead = false,
                    CreatedBy = currentUser.Id,
                    CreatedOn = DateTime.Now
                };

                if (isCanceled)
                {
                    notification.Title = $"Interview Cancellation for {candidateName}";
                    notification.BodyDesc = $"The interview with {candidateName} for the {positionName} position, scheduled on {interviewDate}, has been canceled by HR.";
                }
                else
                {
                    notification.Title = $"New interview invitation for {candidateName}";
                    notification.BodyDesc = $"You've been selected for a First Interview with {candidateName} for the {positionName} position on {interviewDate}. Get ready to shine! 💼🚀";
                }

                await _notificationsRepository.Create(notification);
            }
            catch (Exception ex)
            {

                LogException(nameof(CreateInterviewNotificationForInterviewerAsync), ex, "CreateInterviewNotificationForInterviewerAsync not working");
                throw ex;
            }
        }

        public async Task CreateInterviewNotificationForHRInterview(int status, string notes, int CandidateId, int positionId)
        {
            try
            {

                

                var statusResult = await _statusService.GetById(status);
                var statusstatus = statusResult.Value;


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
                    CreatedBy = currentUser.Id,
                };

                if (statusstatus.Code == Domain.Enums.StatusCode.Approved)
                {
                    notification.Title = $"You have a Third Interview with {candidateName} for the {positionName} position. Get ready to shine! 💼🚀 ";
                }
                else
                {
                    notification.Title = $"{candidateName} Rejected by {userName} for position {positionName}";
                }

                await _notificationsRepository.Create(notification);
            }
            catch (Exception ex)
            {

                LogException(nameof(CreateInterviewNotificationForHRInterview), ex, "CreateInterviewNotificationForHRInterview not working");
                throw ex;
            }
        }
       
        public async Task CreateNotificationForInterviewer(int CandidateId, string selectedInterviewerId)
        {
            try
            {
                


                var currentUser = await _userManager.GetUserAsync(_httpContextAccessor.HttpContext.User);
                var candidateName = await GetCandidateName(CandidateId);

                var notification = new Notifications
                {
                    ReceiverId = selectedInterviewerId,
                    SendDate = DateTime.Now,
                    IsReceived = true,
                    IsRead = false,
                    Title = $"Your interview with {candidateName} cancelled",
                    BodyDesc = $"The HR has rejected this candidate for some reasons, so you don't have an interview for {candidateName}.",
                    CreatedBy = currentUser.Id,
                    CreatedOn = DateTime.Now
                };

                await _notificationsRepository.Create(notification);
            }
            catch (Exception ex)
            {

                LogException(nameof(CreateNotificationForInterviewer), ex, "CreateNotificationForInterviewer not working");
                throw ex;
            }
        }

        public string GetLoggedInUserName()
        {
            try
            {
                return _httpContextAccessor.HttpContext.User.Identity.Name;
            }
            catch (Exception ex)
            {

                LogException(nameof(GetLoggedInUserName), ex, "GetLoggedInUserName not working");
                throw ex;
            }
        }

        public async Task<string> GetCandidateName(int candidateId)
        {
            try
            {

            
            var candidate = await _candidateService.GetCandidateByIdAsync(candidateId);

            if (candidate != null)
            {
                return candidate.FullName;
            }

            return "Candidate Not Found";
            }
            catch (Exception ex)
            {

                LogException(nameof(GetCandidateName), ex, "GetCandidateName not working");
                throw ex;
            }
        }

        public async Task<string> GetPositionName(int positionId)
        {
            try
            {


                var result = await _positionService.GetById(positionId);

                if (result.IsSuccess)
                {
                    var position = result.Value;
                    return position.Name;
                }

                return "Position Not Found";
            }
            catch (Exception ex)
            {

                LogException(nameof(GetPositionName), ex, "GetPositionName not working");
                throw ex;
            }
        }

        public async Task<int> GetUnreadNotificationCount()
        {
            try
            {
                var currentUser = await _userManager.GetUserAsync(_httpContextAccessor.HttpContext.User);


                var userId = currentUser.Id;

                var notifications = await _notificationsRepository.GetAllNotifications(); 

                var unreadCount = notifications
                    .Where(notification => !notification.IsRead && notification.ReceiverId == userId)
                    .Count();

                return unreadCount;
            }
            catch (Exception ex)
            {
                LogException(nameof(GetUnreadNotificationCount), ex);
                throw ex;
            }
        }


    }
}