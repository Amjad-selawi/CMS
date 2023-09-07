using CMS.Application.DTOs;
using CMS.Application.Extensions;
using CMS.Domain.Entities;
using CMS.Domain.Enums;
using CMS.Repository.Implementation;
using CMS.Repository.Interfaces;
using CMS.Services.Interfaces;
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

        public NotificationsService(INotificationsRepository notificationsRepository, ITemplatesService templatesService)
        {
            _notificationsRepository = notificationsRepository;
            _templatesService = templatesService;
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
                Type=i.Type.ToString(),
                templatesDTO = new TemplatesDTO 
                {
                    TemplatesId = i.TemplatesId,
                    Title = _templatesService.GetTemplateByIdAsync(i.TemplatesId).Result?.Title 
                },
                
            });

          
            }

        public async Task<NotificationsDTO> GetNotificationByIdAsync(int notificationsId)
        {
            var notification = await _notificationsRepository.GetNotificationsById(notificationsId);
            if (notification == null)
                return null;

            var templatesDTO = new TemplatesDTO
            {
                TemplatesId = notification.TemplatesId,
                Title = _templatesService.GetTemplateByIdAsync(notification.TemplatesId).Result?.Title,
            };


            return new NotificationsDTO
            {
                NotificationsId = notification.NotificationsId,
                SendDate = notification.SendDate,
                ReceiverId = notification.ReceiverId,
                IsReceived = notification.IsReceived,
                Type = notification.Type.ToString(),
                templatesDTO = templatesDTO,
                Title = templatesDTO.Title

            };
        
            }

        public async Task Create(NotificationsDTO entity)
        {
          
             
            if (Enum.TryParse(entity.Type, out NotificationsType type))
            {
                var notification = new Notifications
                {
                    SendDate = DateTime.Now,
                    ReceiverId = entity.ReceiverId,
                    IsReceived = entity.IsReceived,
                    TemplatesId = entity.templatesDTO.TemplatesId,
                    Type=type,
                };
                await _notificationsRepository.Create(notification);
            }
        }

        public async Task Update(int notificationId, NotificationsDTO entity)
        {
            if (Enum.TryParse(entity.Type, out NotificationsType type))
            {
                var existingNotification = await _notificationsRepository.GetNotificationsById(notificationId);
                if (existingNotification == null)
                    throw new Exception("Notifications not found");

                existingNotification.SendDate = DateTime.Now;
                existingNotification.ReceiverId = entity.ReceiverId;
                existingNotification.IsReceived = entity.IsReceived;
                existingNotification.TemplatesId = entity.templatesDTO.TemplatesId;
                existingNotification.Type = type;

                await _notificationsRepository.Update(existingNotification);
            }
        }

        public async Task Delete(int notificationId)
        {
            var notification = await _notificationsRepository.GetNotificationsById(notificationId);
            if (notification != null)
                await _notificationsRepository.Delete(notification);
        }
    }
}
