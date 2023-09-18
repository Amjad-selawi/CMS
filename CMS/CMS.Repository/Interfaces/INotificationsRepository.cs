using CMS.Application.DTOs;
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
        Task<Notifications> GetNotificationsById(int notificationId);
        Task Create(Notifications entity);
        Task Update(Notifications entity);
        Task Delete(Notifications entity);


        Task<List<Notifications>> GetSpacificNotificationsforHR();

        Task<List<Notifications>> GetSpacificNotificationsforGeneral();

        Task<List<Notifications>> GetSpacificNotificationsforInterviewer();
    }
}
