using CMS.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CMS.Repository.Interfaces
{
    public interface INotificationsRepository
    {
        Task<IEnumerable<Notifications>> GetAllNotifications();
        Task<Notifications> GetNotificationsById(int notificationId,string ByUserId);
        Task Create(Notifications entity, string ByUserId);
        Task Update(Notifications entity, string ByUserId);
        Task Delete(Notifications entity, string ByUserId);




        Task<List<Notifications>> GetSpacificNotificationsforHR( string ByUserId);

        Task<List<Notifications>> GetSpacificNotificationsforGeneral(string ByUserId);
        Task<List<Notifications>> GetSpacificNotificationsforArchi(string ByUserId);

        Task<List<Notifications>> GetSpacificNotificationsforInterviewer(string interviewerId, string ByUserId);

        void LogException(string methodName, Exception ex, string createdByUserId, string additionalInfo);
    }
}
