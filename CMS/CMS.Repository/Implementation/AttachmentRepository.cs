using CMS.Domain;
using CMS.Domain.Entities;
using CMS.Repository.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace CMS.Repository.Implementation
{
    public class AttachmentRepository : IAttachmentRepository
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AttachmentRepository(ApplicationDbContext dbContext, UserManager<IdentityUser> userManager, IHttpContextAccessor httpContextAccessor)
        {
            _dbContext = dbContext;
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
        }

        public async void LogException(string methodName, Exception ex, string additionalInfo)
        {
            var currentUser = await _userManager.GetUserAsync(_httpContextAccessor.HttpContext.User);
            var userId = currentUser?.Id;
            _dbContext.Logs.Add(new Log
            {
                MethodName = methodName,
                ExceptionMessage = ex.Message,
                StackTrace = ex.StackTrace,CreatedByUserId = userId,
                LogTime = DateTime.Now,
                
                AdditionalInfo = additionalInfo
            });
            _dbContext.SaveChanges();
        }

        public async Task<IEnumerable<CMS.Domain.Entities.Attachment>> GetAllAttachmentsAsync()
        {
            try
            {
                return await _dbContext.Attachments.ToListAsync();
            }
            catch (Exception ex)
            {
                LogException(nameof(GetAllAttachmentsAsync), ex,"Enable to get all attachments");
                throw ex;
            }
        }
        public async Task<CMS.Domain.Entities.Attachment> GetAttachmentByIdAsync(int id)
        {
            try
            {
                return await _dbContext.Attachments.FindAsync(id);
            }
            catch (Exception ex)
            {
                LogException(nameof(GetAttachmentByIdAsync), ex, $"Attachment ID: {id}");
                throw ex;
            }
        }

        public async Task<int> CreateAttachmentAsync(CMS.Domain.Entities.Attachment attachment)
        {
            try
            {
                _dbContext.Attachments.Add(attachment);
                await _dbContext.SaveChangesAsync();
                return attachment.Id;
            }
            catch (Exception ex)
            {
                LogException(nameof(CreateAttachmentAsync), ex,"Enable to create attachment");
                throw ex;
            }
        }
        public async Task DeleteAttachmentAsync(int id)
        {
            try
            {
                var attachment = await _dbContext.Attachments.FindAsync(id);
                if (attachment != null)
                {
                    _dbContext.Attachments.Remove(attachment);
                    await _dbContext.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                LogException(nameof(DeleteAttachmentAsync), ex, $"Attachment ID: {id}");
                throw ex;
            }
        }
    }
}
