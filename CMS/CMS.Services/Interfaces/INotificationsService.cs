﻿using CMS.Application.DTOs;
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
        


        Task<List<NotificationsDTO>> GetNotificationsForUserAsync(string userId);

        Task<IEnumerable<NotificationsDTO>> GetNotificationsForHRAsync();
        Task<IEnumerable<NotificationsDTO>> GetNotificationsForHRAsyncicon();
        Task<IEnumerable<NotificationsDTO>> GetNotificationsForInterviewers(string interviewerId);
        Task<IEnumerable<NotificationsDTO>> GetNotificationsForInterviewersicon(string interviewerId);

        Task<IEnumerable<NotificationsDTO>> GetNotificationsForGeneralManager();
        Task<IEnumerable<NotificationsDTO>> GetNotificationsForGeneralManagericon();

        Task<IEnumerable<NotificationsDTO>> GetNotificationsForArchitecture();
        Task<IEnumerable<NotificationsDTO>> GetNotificationsForArchitectureicon();


        Task CreateNotificationForGeneralManagerAsync(int status, string notes, int CandidateId, int positionId, string archiInterviewerId);
        Task CreateNotificationForArchiAsync(int status, string notes, int CandidateId, int positionId);

        Task CreateInterviewNotificationForInterviewerAsync(DateTime interviewDate, int CandidateId, int positionId, List<string> selectedInterviewerId, bool isCanceled);


        Task CreateInterviewNotificationForHRInterview(int status, string notes, int CandidateId, int positionId);
        Task CreateInterviewNotificationForFinalHRInterview(int status, string notes, int CandidateId, int positionId);
        Task CreateInterviewNotificationtoHrForOnHold(int status, string notes, int CandidateId, int positionId);
        Task CreateInterviewNotificationForHRInterviewfromGM(int status, string notes, int CandidateId, int positionId);

        Task CreateNotificationForInterviewer(int CandidateId, string selectedInterviewerId);
        Task<NotificationsDTO> GetNotificationByIdforDetails(int notificationsId);

        void LogException(string methodName, Exception ex = null, string additionalInfo = null);
        Task<int> GetUnreadNotificationCount();
        Task MarkAllAsReadForHRAsync();
    }
}
