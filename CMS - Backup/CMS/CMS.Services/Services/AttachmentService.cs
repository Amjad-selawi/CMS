using CMS.Application.DTOs;
using CMS.Repository.Interfaces;
using CMS.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace CMS.Services.Services
{
    public class AttachmentService : IAttachmentService
    {
        private readonly IAttachmentRepository _attachmentRepository;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public AttachmentService(IAttachmentRepository attachmentRepository, UserManager<IdentityUser> userManager, IHttpContextAccessor httpContextAccessor)
        {
            _attachmentRepository = attachmentRepository;
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<IEnumerable<AttachmentDTO>> GetAllAttachmentsAsync()
        {
            var attachments = await _attachmentRepository.GetAllAttachmentsAsync();
            return attachments.Select(a => new AttachmentDTO
            {
                Id = a.Id,
                FileName = a.FileName,
                FileSize = a.FileSize,
                FileData = a.FileData,
                CreatedOn = a.CreatedOn
            });
        }

        public async Task<AttachmentDTO> GetAttachmentByIdAsync(int id)
        {
            var attachment = await _attachmentRepository.GetAttachmentByIdAsync(id);
            if (attachment == null)
                return null;

            return new AttachmentDTO
            {
                Id = attachment.Id,
                FileName = attachment.FileName,
                FileSize = attachment.FileSize,
                FileData = attachment.FileData,
                CreatedOn = attachment.CreatedOn
            };
        }

        public async Task<int> CreateAttachmentAsync(string fileName, long fileSize, Stream fileStream)
        {
            var currentUser = await _userManager.GetUserAsync(_httpContextAccessor.HttpContext.User);
            var attachment = new CMS.Domain.Entities.Attachment
            {
                FileName = fileName,
                FileSize = fileSize,
                FileData = ReadStream(fileStream),
                CreatedOn = DateTime.Now,
                CreatedBy = currentUser.Id
                
            };

            return await _attachmentRepository.CreateAttachmentAsync(attachment);
        }

        public async Task DeleteAttachmentAsync(int id)
        {
            await _attachmentRepository.DeleteAttachmentAsync(id);
        }

        public async Task<byte[]> GetAttachmentFileDataAsync(int id)
        {
            var attachment = await _attachmentRepository.GetAttachmentByIdAsync(id);
            return attachment?.FileData;
        }

        private byte[] ReadStream(Stream stream)
        {
            using (var memoryStream = new MemoryStream())
            {
                stream.CopyTo(memoryStream);
                return memoryStream.ToArray();
            }
        }

        public Task UpdateAttachmentAsync(int id, AttachmentDTO attachmentDTO)
        {
            throw new NotImplementedException();
        }
    }
}
