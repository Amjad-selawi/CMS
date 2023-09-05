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


    }
}
