using CMS.Application.DTOs;
using CMS.Application.Extensions;
using CMS.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CMS.Services.Interfaces
{
    public interface INotificationsService
    {

        Task<IEnumerable<NotificationsDTO>> GetAllNotificationsAsync();
        Task<NotificationsDTO> GetNotificationByIdAsync(int notificationsId);
        Task Create(NotificationsDTO entity);
        Task Update(int notificationsId, NotificationsDTO entity);
        Task Delete(int notificationsId);
        string GetUserId();


        Task<List<NotificationsDTO>> GetNotificationsForUserAsync(string userId);

        Task<IEnumerable<NotificationsDTO>> GetNotificationsForHRAsync();
        Task<IEnumerable<NotificationsDTO>> GetNotificationsForHRAsyncicon();
        Task<IEnumerable<NotificationsDTO>> GetNotificationsForInterviewers(string interviewerId);
        Task<IEnumerable<NotificationsDTO>> GetNotificationsForInterviewersicon(string interviewerId);

        Task<IEnumerable<NotificationsDTO>> GetNotificationsForGeneralManager();
        Task<IEnumerable<NotificationsDTO>> GetNotificationsForGeneralManagericon();

        Task<IEnumerable<NotificationsDTO>> GetNotificationsForArchitecture();
        Task<IEnumerable<NotificationsDTO>> GetNotificationsForArchitectureicon();


        Task CreateNotificationForGeneralManagerAsync(int status, string notes, int CandidateId, int positionId, string ByUserId);
        Task CreateNotificationForArchiAsync(int status, string notes, int CandidateId, int positionId, string ByUserId);

        Task CreateInterviewNotificationForInterviewerAsync(DateTime interviewDate, int CandidateId, int positionId, string selectedInterviewerId, bool isCanceled, string ByUserId);


        Task CreateInterviewNotificationForHRInterview(int status, string notes, int CandidateId, int positionId, string ByUserId);

        Task CreateNotificationForInterviewer(int CandidateId, string selectedInterviewerId, string ByUserId);
        Task<NotificationsDTO> GetNotificationByIdforDetails(int notificationsId);

        void LogException(string methodName, Exception ex, string createdByUserId = null, string additionalInfo = null);
        Task<int> GetUnreadNotificationCount();
    }
}
