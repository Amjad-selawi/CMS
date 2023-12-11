using CMS.Application.DTOs;
using CMS.Application.Extensions;
using CMS.Domain.Entities;
using CMS.Repository.Interfaces;
using CMS.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace CMS.Services.Services
{
    public class CandidateService : ICandidateService
    {
        private readonly ICandidateRepository _candidateRepository;
        private readonly IAttachmentService _attachmentService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserManager<IdentityUser> _userManager;

        public CandidateService(ICandidateRepository candidateRepository,
            IAttachmentService attachmentService,
            UserManager<IdentityUser> userManager, IHttpContextAccessor httpContextAccessor
            )
        {
            _candidateRepository = candidateRepository;
            _attachmentService = attachmentService;
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
        }



        public void LogException(string methodName, Exception ex, string createdByUserId = null, string additionalInfo = null)
        {
            _candidateRepository.LogException(methodName, ex, createdByUserId, additionalInfo);
        }

        public string GetUserId()
        {
            var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return userId;
        }


        public async Task<IEnumerable<CandidateDTO>> GetAllCandidatesAsync()
        {
            try
            {
                var candidates = await _candidateRepository.GetAllCandidatesAsync();
                var data = candidates.Select(c => new CandidateDTO
                {
                    Id = c.Id,
                    FullName = c.FullName,
                    Phone = c.Phone,
                    PositionId = c.PositionId,
                    PositionName = c.Position.Name,
                    Name = c.Position.Name,
                    CompanyId = c.CompanyId,
                    CompanyName = c.Company.Name,

                    Experience = c.Experience,
                    CVAttachmentId = c.CVAttachmentId,
                    //CountryId = c.CountryId,
                    //CountryName=c.Country.Name
                });
                return data;
            }
          catch (Exception ex)
            {
                LogException(nameof(GetAllCandidatesAsync), ex,null,null);
                throw ex;
            }
        }

        public async Task<CandidateDTO> GetCandidateByIdAsync(int id)
        {
            try
            {
                var candidate = await _candidateRepository.GetCandidateByIdAsync(id,GetUserId());


                if (candidate == null)
                    return null;

                return new CandidateDTO
                {
                    Id = candidate.Id,
                    FullName = candidate.FullName,
                    Phone = candidate.Phone,
                    PositionId = candidate.PositionId,
                    Name = candidate.Position.Name,
                    CompanyId = candidate.CompanyId,
                    CompanyName = candidate.Company.Name,
                    Experience = candidate.Experience,
                    CVAttachmentId = candidate.CVAttachmentId,
                    //CountryId = candidate.CountryId,
                    //CountryName = candidate.Country.Name

                };
            }
          
            catch (Exception ex)
            {
                LogException(nameof(GetCandidateByIdAsync), ex, null, null);
                throw ex;
            }
        }

        public async Task CreateCandidateAsync(CandidateCreateDTO candidateDTO)
        {
            try
            {
                if (candidateDTO.FileData != null)
                {
                    int attachmentId = await _attachmentService.CreateAttachmentAsync(candidateDTO.FileName, candidateDTO.FileSize, candidateDTO.FileData);
                    candidateDTO.CVAttachmentId = attachmentId;
                }
                var currentUser = await _userManager.GetUserAsync(_httpContextAccessor.HttpContext.User);

                var candidate = new Candidate
                {
                    FullName = candidateDTO.FullName,
                    Phone = candidateDTO.Phone,
                    PositionId = candidateDTO.PositionId,
                    CompanyId = candidateDTO.CompanyId,
                    Experience = candidateDTO.Experience,
                    CVAttachmentId = candidateDTO.CVAttachmentId,
                    //CountryId = candidateDTO.CountryId,
                    CreatedBy = currentUser.Id,
                    CreatedOn = DateTime.Now
                };
                await _candidateRepository.CreateCandidateAsync(candidate, GetUserId());

            }

            catch (Exception ex)
            {
                LogException(nameof(CreateCandidateAsync), ex, null, null);
                throw ex;
            }
        }

        public async Task UpdateCandidateAsync(int id, CandidateDTO candidateDTO)
        {
            try
            {
                var existingCandidate = await _candidateRepository.GetCandidateByIdAsync(id, GetUserId());
                if (existingCandidate == null)
                    throw new Exception("Candidate not found");
                var currentUser = await _userManager.GetUserAsync(_httpContextAccessor.HttpContext.User);

                existingCandidate.FullName = candidateDTO.FullName;
                existingCandidate.Phone = candidateDTO.Phone;
                existingCandidate.PositionId = candidateDTO.PositionId;
                existingCandidate.CompanyId = candidateDTO.CompanyId;
                existingCandidate.Experience = candidateDTO.Experience;
                existingCandidate.CVAttachmentId = candidateDTO.CVAttachmentId;
                //existingCandidate.CountryId = candidateDTO.CountryId;
                existingCandidate.ModifiedOn = DateTime.Now;
                existingCandidate.ModifiedBy = currentUser.Id;

                await _candidateRepository.UpdateCandidateAsync(existingCandidate, GetUserId());

            }

            catch (Exception ex)
            {
                LogException(nameof(UpdateCandidateAsync), ex, null, null);
                throw ex;
            }
        }


        public async Task DeleteCandidateAsync(int id)
        {
            try
            {
                var candidate = await _candidateRepository.GetCandidateByIdAsync(id, GetUserId());

                if (candidate != null)
                {
                    int? attachmentToRemove = (int?)candidate.CVAttachmentId;
                    await _candidateRepository.DeleteCandidateAsync(candidate, GetUserId());

                    if (attachmentToRemove.HasValue)
                    {
                        await _attachmentService.DeleteAttachmentAsync(attachmentToRemove.Value);
                    }
                }

            }

            catch (Exception ex)
            {
                LogException(nameof(DeleteCandidateAsync), ex, null, null);
                throw ex;
            }
        }


        public async Task UpdateCandidateCVAsync(int id, string fileName, long fileSize, Stream fileStream)
        {
            try
            {
                var candidate = await _candidateRepository.GetCandidateByIdAsync(id, GetUserId());
                int attachmentId = await _attachmentService.CreateAttachmentAsync(fileName, fileSize, fileStream);

                int attachmentToRemove = 0;
                if (candidate.CVAttachmentId != null)
                {
                    attachmentToRemove = (int)candidate.CVAttachmentId;


                }

                candidate.CVAttachmentId = attachmentId;
                await _candidateRepository.UpdateCandidateAsync(candidate, GetUserId());
                if (attachmentToRemove != 0)
                {
                    await _attachmentService.DeleteAttachmentAsync(attachmentToRemove);
                }
            }
            
              catch (Exception ex)
            {
                LogException(nameof(UpdateCandidateCVAsync), ex, null, null);
                throw ex;
            }

        }



        public async Task<int?> GetCVAttachmentIdByCandidateId(int candidateId)
        {
            try
            {
                return await _candidateRepository.GetCVAttachmentIdByCandidateId(candidateId);
            }
            catch (Exception ex)
            {
                LogException(nameof(GetCVAttachmentIdByCandidateId), ex, null, null);
                throw ex;
            }
        }


    }
}
