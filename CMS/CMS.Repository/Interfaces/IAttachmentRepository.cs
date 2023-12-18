using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace CMS.Repository.Interfaces
{
    public interface IAttachmentRepository
    {
        Task<IEnumerable<CMS.Domain.Entities.Attachment>> GetAllAttachmentsAsync();
        Task<CMS.Domain.Entities.Attachment> GetAttachmentByIdAsync(int id);
        Task<int> CreateAttachmentAsync(CMS.Domain.Entities.Attachment attachment);
        Task DeleteAttachmentAsync(int id);
        void LogException(string methodName, Exception ex,  string additionalInfo);
    }
}
