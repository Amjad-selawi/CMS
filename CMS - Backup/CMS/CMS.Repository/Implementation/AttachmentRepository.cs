using CMS.Domain;
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

        public async Task<IEnumerable<CMS.Domain.Entities.Attachment>> GetAllAttachmentsAsync()
        {
            return await _dbContext.Attachments.ToListAsync();
        }

        public async Task<CMS.Domain.Entities.Attachment> GetAttachmentByIdAsync(int id)
        {
            return await _dbContext.Attachments.FindAsync(id);
        }

        public async Task<int> CreateAttachmentAsync(CMS.Domain.Entities.Attachment attachment)
        {
            _dbContext.Attachments.Add(attachment);
            await _dbContext.SaveChangesAsync();
            return attachment.Id;
        }

        public async Task DeleteAttachmentAsync(int id)
        {
            var attachment = await _dbContext.Attachments.FindAsync(id);
            if (attachment != null)
            {
                _dbContext.Attachments.Remove(attachment);
                await _dbContext.SaveChangesAsync();
            }
        }
    }
}
