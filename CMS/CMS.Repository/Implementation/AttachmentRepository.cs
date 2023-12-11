using CMS.Domain;
using CMS.Domain.Entities;
using CMS.Repository.Interfaces;
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

        public AttachmentRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void LogException(string methodName, Exception ex, string createdByUserId, string additionalInfo)
        {

            _dbContext.Logs.Add(new Log
            {
                MethodName = methodName,
                ExceptionMessage = ex.Message,
                StackTrace = ex.StackTrace,
                LogTime = DateTime.Now,
                CreatedByUserId = createdByUserId,
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
                LogException(nameof(GetAllAttachmentsAsync), ex, null, null);
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
                LogException(nameof(GetAttachmentByIdAsync), ex, null, $"Attachment ID: {id}");
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
                LogException(nameof(CreateAttachmentAsync), ex, null, null);
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
                LogException(nameof(DeleteAttachmentAsync), ex, null, $"Attachment ID: {id}");
                throw ex;
            }
        }
    }
}
