using CMS.Application.DTOs;
using CMS.Domain.Entities;
using CMS.Repository.Interfaces;
using CMS.Services.Interfaces;
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

        public CandidateService(ICandidateRepository candidateRepository, IAttachmentService attachmentService)
        {
            _candidateRepository = candidateRepository;
            _attachmentService = attachmentService;
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
                Email = c.Email,
                Address = c.Address,
                Experience = c.Experience,
                CVAttachmentId = c.CVAttachmentId,
                LinkedInUrl = c.LinkedInUrl
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
                Email = candidate.Email,
                Address = candidate.Address,
                Experience = candidate.Experience,
                CVAttachmentId = candidate.CVAttachmentId,
                LinkedInUrl = candidate.LinkedInUrl,
                CountryId = candidate.CountryId,
            };
        }

        public async Task CreateCandidateAsync(CandidateCreateDTO candidateDTO)
        {
            int attachmentId = await _attachmentService.CreateAttachmentAsync(candidateDTO.FileName,candidateDTO.FileSize,candidateDTO.FileData);
            candidateDTO.CVAttachmentId = attachmentId;
            var candidate = new Candidate
            {
                FullName = candidateDTO.FullName,
                Phone = candidateDTO.Phone,
                PositionId = candidateDTO.PositionId,
                Email = candidateDTO.Email,
                Address = candidateDTO.Address,
                Experience = candidateDTO.Experience,
                CVAttachmentId = candidateDTO.CVAttachmentId,
                LinkedInUrl = candidateDTO.LinkedInUrl,
                CountryId = candidateDTO.CountryId
            };
            await _candidateRepository.CreateCandidateAsync(candidate);
        }

        public async Task UpdateCandidateAsync(int id, CandidateDTO candidateDTO)
        {
            var existingCandidate = await _candidateRepository.GetCandidateByIdAsync(id);
            if (existingCandidate == null)
                throw new Exception("Candidate not found");

            existingCandidate.FullName = candidateDTO.FullName;
            existingCandidate.Phone = candidateDTO.Phone;
            existingCandidate.PositionId = candidateDTO.PositionId;
            existingCandidate.Email = candidateDTO.Email;
            existingCandidate.Address = candidateDTO.Address;
            existingCandidate.Experience = candidateDTO.Experience;
            existingCandidate.CVAttachmentId = candidateDTO.CVAttachmentId;
            existingCandidate.LinkedInUrl = candidateDTO.LinkedInUrl;
            existingCandidate.CountryId = candidateDTO.CountryId;

            await _candidateRepository.UpdateCandidateAsync(existingCandidate);
        }

        public async Task DeleteCandidateAsync(int id)
        {
            var candidate = await _candidateRepository.GetCandidateByIdAsync(id);
          
            if (candidate != null)
            {
                int attachmentToRemove = candidate.CVAttachmentId;
                await _candidateRepository.DeleteCandidateAsync(candidate);
                await _attachmentService.DeleteAttachmentAsync(attachmentToRemove);
            }

        }

        public async Task UpdateCandidateCVAsync(int id, string fileName, long fileSize, Stream fileStream)
        {
            var candidate = await _candidateRepository.GetCandidateByIdAsync(id);
            int attachmentId = await _attachmentService.CreateAttachmentAsync(fileName, fileSize, fileStream);
            int attachmentToRemove = candidate.CVAttachmentId;
            candidate.CVAttachmentId = attachmentId;
            await _candidateRepository.UpdateCandidateAsync(candidate);
            await _attachmentService.DeleteAttachmentAsync(attachmentToRemove);
        }
    }
}
