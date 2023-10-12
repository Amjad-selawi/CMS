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

        public async Task<IEnumerable<CandidateDTO>> GetAllCandidatesAsync()
        {
            var candidates = await _candidateRepository.GetAllCandidatesAsync();
            return candidates.Select(c => new CandidateDTO
            {
                Id = c.Id,
                FullName = c.FullName,
                Phone = c.Phone,
                PositionId = c.PositionId,
                Name = c.Position.Name,
                CompanyId = c.CompanyId,
                CompanyName=c.Company.Name,
             
                Experience = c.Experience,
                CVAttachmentId = c.CVAttachmentId,
                CountryId = c.CountryId,
                CountryName=c.Country.Name
            });
        }

        public async Task<CandidateDTO> GetCandidateByIdAsync(int id)
        {
            var candidate = await _candidateRepository.GetCandidateByIdAsync(id);


            if (candidate == null)
                return null;

            return new CandidateDTO
            {
                Id = candidate.Id,
                FullName = candidate.FullName,
                Phone = candidate.Phone,
                PositionId = candidate.PositionId,
                Name = candidate.Position.Name,
                CompanyId =candidate.CompanyId,
                CompanyName=candidate.Company.Name,
                Experience = candidate.Experience,
                CVAttachmentId = candidate.CVAttachmentId,
                CountryId = candidate.CountryId,
                CountryName = candidate.Country.Name

            };
        }

        public async Task CreateCandidateAsync(CandidateCreateDTO candidateDTO)
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
                CompanyId =candidateDTO.CompanyId,
                Experience = candidateDTO.Experience,
                CVAttachmentId = candidateDTO.CVAttachmentId,
                CountryId = candidateDTO.CountryId,
                CreatedBy =currentUser.Id,
                CreatedOn=DateTime.Now
            };
            await _candidateRepository.CreateCandidateAsync(candidate);
        }

        public async Task UpdateCandidateAsync(int id, CandidateDTO candidateDTO)
        {
            var existingCandidate = await _candidateRepository.GetCandidateByIdAsync(id);
            if (existingCandidate == null)
                throw new Exception("Candidate not found");
            var currentUser = await _userManager.GetUserAsync(_httpContextAccessor.HttpContext.User);

            existingCandidate.FullName = candidateDTO.FullName;
            existingCandidate.Phone = candidateDTO.Phone;
            existingCandidate.PositionId = candidateDTO.PositionId;
            existingCandidate.CompanyId= candidateDTO.CompanyId;
            existingCandidate.Experience = candidateDTO.Experience;
            existingCandidate.CVAttachmentId = candidateDTO.CVAttachmentId;
            existingCandidate.CountryId = candidateDTO.CountryId;
            existingCandidate.ModifiedOn = DateTime.Now;
            existingCandidate.ModifiedBy = currentUser.Id;

            await _candidateRepository.UpdateCandidateAsync(existingCandidate);
        }


        //public async Task<Result<CandidateDTO>> DeleteCandidateAsync(int id)
        //{

        //    try
        //    {
        //        await _candidateRepository.DeleteCandidateAsync(id);
        //        return Result<CandidateDTO>.Success(null);
        //    }

        //    catch (Exception ex)
        //    {
        //        return Result<CandidateDTO>.Failure(null, $"An error occurred while deleting the candidate{ex.InnerException.Message}");
        //    }
        //}


        public async Task DeleteCandidateAsync(int id)
        {
            var candidate = await _candidateRepository.GetCandidateByIdAsync(id);

            if (candidate != null)
            {
                int attachmentToRemove = (int)candidate.CVAttachmentId;
                await _candidateRepository.DeleteCandidateAsync(candidate);
                await _attachmentService.DeleteAttachmentAsync(attachmentToRemove);
            }

        }

        public async Task UpdateCandidateCVAsync(int id, string fileName, long fileSize, Stream fileStream)
        {
            var candidate = await _candidateRepository.GetCandidateByIdAsync(id);
            int attachmentId = await _attachmentService.CreateAttachmentAsync(fileName, fileSize, fileStream);
            
            int attachmentToRemove=0;
            if (candidate.CVAttachmentId != null)
            {
                attachmentToRemove = (int)candidate.CVAttachmentId;
                
                
            }
           
            candidate.CVAttachmentId = attachmentId;
            await _candidateRepository.UpdateCandidateAsync(candidate);
            if (attachmentToRemove!=0)
            {
                await _attachmentService.DeleteAttachmentAsync(attachmentToRemove);
            }
            
        }
    }
}
