﻿using CMS.Application.DTOs;
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
                //TemplateName = i.TemplateName.ToString(),
                templatesDTO = new TemplatesDTO 
                {
                    TemplatesId = i.TemplatesId,
                    Title = _templatesService.GetTemplateByIdAsync(i.TemplatesId).Result?.Title,
                    //Name = _templatesService.GetTemplateByIdAsync(i.TemplatesId).Result?.Name.ToString()
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
                //Name = _templatesService.GetTemplateByIdAsync(notification.TemplatesId).Result?.Name.ToString() 

            };


            return new NotificationsDTO
            {
                NotificationsId = notification.NotificationsId,
                SendDate = notification.SendDate,
                ReceiverId = notification.ReceiverId,
                IsReceived = notification.IsReceived,
                templatesDTO = templatesDTO,
                Title = templatesDTO.Title,
                //TemplateName = templatesDTO.Name

            };
        
            }

        public async Task Create(NotificationsDTO entity)
        {
          
             
            
                var notification = new Notifications
                {
                    SendDate = DateTime.Now,
                    ReceiverId = entity.ReceiverId,
                    IsReceived = entity.IsReceived,
                    TemplatesId = entity.templatesDTO.TemplatesId,
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
                existingNotification.TemplatesId = entity.templatesDTO.TemplatesId;
                //existingNotification.TemplateName = entity.templatesDTO.Name;

                await _notificationsRepository.Update(existingNotification);
           
        }

        public async Task Delete(int notificationId)
        {
            var notification = await _notificationsRepository.GetNotificationsById(notificationId);
            if (notification != null)
                await _notificationsRepository.Delete(notification);
        }
    }
}
