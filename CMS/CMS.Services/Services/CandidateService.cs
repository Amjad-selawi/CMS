using CMS.Application.DTOs;
using CMS.Application.Extensions;
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

        //public async Task<Result<CandidateCreateDTO>> Delete(int id)
        //{
        //    try
        //    {
        //        await _candidateRepository.Delete(id);
        //        return Result<CandidateCreateDTO>.Success(null);
        //    }
        //    catch (Exception ex)
        //    {
        //        return Result<CandidateCreateDTO>.Failure(null, $"An error occurred while deleting the position{ex.InnerException.Message}");
        //    }
        //}

        //public async Task<Result<List<CandidateCreateDTO>>> GetAll()
        //{

        //    var candidate = await _candidateRepository.GetAll();
        //    if (candidate == null)
        //    {
        //        return Result<List<CandidateCreateDTO>>.Failure(null, "no candidates found");
        //    }
        //    try
        //    {
        //        var candidateDTOs = new List<CandidateCreateDTO>();
        //        foreach (var co in candidate)
        //        {
        //            candidateDTOs.Add(new CandidateCreateDTO
        //            {
        //                CandidateId = co.CandidateId,
        //                FullName = co.FullName,
        //                InterviewsDTO = co.Interviews.Select(com => new InterviewsDTO
        //                {
        //                   InterviewsId=com.InterviewsId,
        //                    CandidateId = com.CandidateId,
        //                    Date= com.Date,
        //                    Notes=com.Notes,
        //                    InterviewerId = com.InterviewerId,
        //                    ParentId=com.ParentId,
        //                    Score=com.Score,
        //                    Status = com.Status.ToString(),



        //                }).ToList()

        //            });

        //        }
        //        return Result<List<CandidateCreateDTO>>.Success(candidateDTOs);
        //    }
        //    catch (Exception ex)
        //    {
        //        return Result<List<CandidateCreateDTO>>.Failure(null, $"unable to get countries{ex.InnerException.Message}");
        //    }

        //}

        //public async Task<Result<CandidateCreateDTO>> GetById(int id)
        //{
        //    if (id <= 0)
        //    {
        //        return Result<CandidateCreateDTO>.Failure(null, "Invalid candidate id");
        //    }
        //    try
        //    {
        //        var candidate = await _candidateRepository.GetById(id);
        //        var candidateDTOs = new CandidateCreateDTO
        //        {
        //            CandidateId = candidate.CandidateId,
        //            FullName = candidate.FullName,
        //            InterviewsDTO = candidate.Interviews.Select(com => new InterviewsDTO
        //            {
        //                InterviewsId = com.InterviewsId,
        //                CandidateId = com.CandidateId,
        //                Date = com.Date,
        //                Notes = com.Notes,
        //                InterviewerId = com.InterviewerId,
        //                ParentId = com.ParentId,
        //                Score = com.Score,
        //                Status = com.Status.ToString(),



        //            }).ToList()

        //        };
        //        return Result<CandidateCreateDTO>.Success(candidateDTOs);
        //    }
        //    catch (Exception ex)
        //    {
        //        return Result<CandidateCreateDTO>.Failure(null, $"unable to retrieve the Candidate from the repository{ex.InnerException.Message}");
        //    }
        //}

        //public async Task<Result<CandidateCreateDTO>> Insert(CandidateCreateDTO data)
        //{

        //    if (data == null)
        //    {
        //        return Result<CandidateCreateDTO>.Failure(data, "the candidate dto is null");
        //    }
        //    var candidate = new Candidate
        //    {
        //        FullName = data.FullName,
        //        Address = data.Address,
        //        Email=data.Email,
        //        Experience = data.Experience,
        //        Phone = data.Phone,
        //        LinkedInUrl = data.LinkedInUrl,
        //        CVAttachmentId = data.CVAttachmentId,
        //        DesiredPosition = data.DesiredPosition,

        //    };
        //    try
        //    {
        //        await _candidateRepository.Insert(candidate);
        //        return Result<CandidateCreateDTO>.Success(data);

        //    }
        //    catch (Exception ex)
        //    {
        //        return Result<CandidateCreateDTO>.Failure(data, $"unable to insert a position: {ex.InnerException.Message}");
        //    }
        //}

        //public async Task<Result<CandidateCreateDTO>> Update(CandidateCreateDTO data)
        //{
        //    if (data == null)
        //    {
        //        return Result<CandidateCreateDTO>.Failure(null, "can not update a null object");
        //    }
        //    var candidate = new Candidate
        //    {
        //        FullName = data.FullName,
        //        CandidateId = data.CandidateId,
        //        Address = data.Address,
        //        Email = data.Email,
        //        Experience = data.Experience,
        //        Phone = data.Phone,
        //        LinkedInUrl = data.LinkedInUrl,
        //        CVAttachmentId = data.CVAttachmentId,
        //        DesiredPosition = data.DesiredPosition,


        //    };
        //    try
        //    {
        //        await _candidateRepository.Update(candidate);
        //        return Result<CandidateCreateDTO>.Success(data);
        //    }
        //    catch (Exception ex)
        //    {
        //        return Result<CandidateCreateDTO>.Failure(data, $"unable to update the Candidate: {ex.InnerException.Message}");
        //    }
        //}





        public async Task<IEnumerable<CandidateDTO>> GetAllCandidatesAsync()
        {
            var candidates = await _candidateRepository.GetAllCandidatesAsync();
            return candidates.Select(c => new CandidateDTO
            {
                CandidateId = c.CandidateId,
                FullName = c.FullName,
                Phone = c.Phone,
                PositionId = c.PositionId,
                DesiredPosition = c.Position.Name,
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
                CandidateId = candidate.CandidateId,
                FullName = candidate.FullName,
                Phone = candidate.Phone,
                PositionId = candidate.PositionId,
                DesiredPosition = candidate.Position.Name,
                Email = candidate.Email,
                Address = candidate.Address,
                Experience = candidate.Experience,
                CVAttachmentId = candidate.CVAttachmentId,
                LinkedInUrl = candidate.LinkedInUrl,

            };
        }

        public async Task CreateCandidateAsync(CandidateCreateDTO candidateDTO)
        {
            int attachmentId = await _attachmentService.CreateAttachmentAsync(candidateDTO.FileName, candidateDTO.FileSize, candidateDTO.FileData);
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
                LinkedInUrl = candidateDTO.LinkedInUrl
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
