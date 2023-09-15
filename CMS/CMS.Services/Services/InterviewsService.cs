// CMS.Application/Services/InterviewsService.cs
using CMS.Application.DTOs;
using CMS.Domain.Entities;
using CMS.Domain.Enums;
using CMS.Repository.Interfaces;
using CMS.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CMS.Services.Services
{
    public class InterviewsService : IInterviewsService
    {
       private readonly  IInterviewsRepository _interviewsRepository;
        private readonly ICandidateService _candidateService;
        private readonly IPositionService _positionService;

        public InterviewsService(IInterviewsRepository interviewsRepository,ICandidateService candidateService,IPositionService positionService)
        {
            _interviewsRepository = interviewsRepository;
            _candidateService = candidateService;
            _positionService = positionService;
        }

        public async Task<IEnumerable<InterviewsDTO>> GetAllInterviewsAsync()
        {
            var interviews = await _interviewsRepository.GetAllInterviews();
            return interviews.Select(i => new InterviewsDTO
            {
                InterviewsId = i.InterviewsId,
                Source = i.Source,
                Date = i.Date,
                ParentId = i.ParentId,
                InterviewerId=i.InterviewerId,
                Notes=i.Notes,
                Status =i.Status.ToString(),
                candidateDTO = new CandidateDTO
                {
                    Id = i.CandidateId,
                    FullName = _candidateService.GetCandidateByIdAsync(i.CandidateId).Result?.FullName
                },
                positionDTO = new PositionDTO
                {
                    Id = i.PositionId,
                    Name = _positionService.GetById(i.PositionId).Result?.Name
                }

            });
        }

        public async Task<InterviewsDTO> GetInterviewByIdAsync(int interviewId)
        {
            var interview = await _interviewsRepository.GetInterviewById(interviewId);
            if (interview == null)
                return null;

            var candidateDTO = new CandidateDTO
            {
                Id = interview.CandidateId,
                FullName = _candidateService.GetCandidateByIdAsync(interview.InterviewsId).Result?.FullName,
            };

            var positionDTO = new PositionDTO
            {
                Id = interview.PositionId,
                Name = _positionService.GetById(interview.InterviewsId).Result?.Name,

            };

            return new InterviewsDTO
            {
                InterviewsId = interview.InterviewsId,
                Source = interview.Source,
                Date = interview.Date,
                ParentId = interview.ParentId,
                InterviewerId = interview.InterviewerId,
                Notes = interview.Notes,

                candidateDTO = candidateDTO,
                positionDTO = positionDTO,

                FullName = candidateDTO.FullName,
                Name=positionDTO.Name,

                Status = interview.Status.ToString(),
            };
        }

        public async Task Create(InterviewsDTO entity)
        {
            if (Enum.TryParse(entity.Status, out InterviewStatus status))
            {
                var interview = new Interviews
                {
                    Source = entity.Source,
                    Date = entity.Date,
                    ParentId = entity.ParentId,
                    InterviewerId = entity.InterviewerId,
                    Notes = entity.Notes,
                    Status = status,

                    CandidateId=entity.candidateDTO.Id,
                    PositionId=entity.positionDTO.Id,
                };
                await _interviewsRepository.Create(interview);
            }
        }

        public async Task Update(int interviewId, InterviewsDTO entity)
        {
            if (Enum.TryParse(entity.Status, out InterviewStatus status))
            {

            
                var existingInterview = await _interviewsRepository.GetInterviewById(interviewId);
            if (existingInterview == null)
                throw new Exception("Interview not found");


            existingInterview.Source = entity.Source;
            existingInterview.Date = entity.Date;
            existingInterview.ParentId = entity.ParentId;
            existingInterview.InterviewerId = entity.InterviewerId;
            existingInterview.Notes = entity.Notes;
            existingInterview.CandidateId = entity.candidateDTO.Id;
            existingInterview.PositionId = entity.positionDTO.Id;
            existingInterview.Status = status;

            await _interviewsRepository.Update(existingInterview);

            }


        }

        public async Task Delete(int interviewId)
        {
            var interview = await _interviewsRepository.GetInterviewById(interviewId);
            if (interview != null)
                await _interviewsRepository.Delete(interview);
        }








    }
}
