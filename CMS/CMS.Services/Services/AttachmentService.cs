using CMS.Application.DTOs;
using CMS.Repository.Implementation;
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
using System.Security.Claims;
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

        public void LogException(string methodName, Exception ex = null, string additionalInfo = null)
        {
            _attachmentRepository.LogException(methodName, ex, additionalInfo);
        }


        public async Task<IEnumerable<AttachmentDTO>> GetAllAttachmentsAsync()
        {
            try
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
            catch (Exception ex)
            {
                LogException(nameof(GetAllAttachmentsAsync), ex, "GetAllAttachmentsAsync not working");
                throw ex;
            }
        }

        public async Task<AttachmentDTO> GetAttachmentByIdAsync(int id)
        {
            try
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
            catch (Exception ex)
            {
                LogException(nameof(GetAttachmentByIdAsync), ex, $"Attachment ID: {id}");
                throw ex;
            }
        }
        public async Task<int> CreateAttachmentAsync(string fileName, long fileSize, Stream fileStream)
        {
            try
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
            catch (Exception ex)
            {
                LogException(nameof(CreateAttachmentAsync), ex, "CreateAttachmentAsync not working");
                throw ex;
            }
        }

        public async Task DeleteAttachmentAsync(int id)
        {
            try
            {
                await _attachmentRepository.DeleteAttachmentAsync(id);
            }
            catch (Exception ex)
            {
                LogException(nameof(DeleteAttachmentAsync), ex, $"Faild to delete Attachment ID: {id}");
                throw ex;
            }
        }

        public async Task<byte[]> GetAttachmentFileDataAsync(int id)
        {
            try
            {
                var attachment = await _attachmentRepository.GetAttachmentByIdAsync(id);
                return attachment?.FileData;
            }
            catch (Exception ex)
            {
                LogException(nameof(GetAttachmentFileDataAsync), ex, $"Faild to get Attachment ID: {id}");
                throw ex;
            }
        }
        public Task UpdateAttachmentAsync(int id, AttachmentDTO attachmentDTO)
        {
            throw new NotImplementedException();
        }

        private byte[] ReadStream(Stream stream)
        {
            using (var memoryStream = new MemoryStream())
            {
                stream.CopyTo(memoryStream);
                return memoryStream.ToArray();
            }
        }
    }
}
