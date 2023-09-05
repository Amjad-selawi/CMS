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

        public InterviewsService(IInterviewsRepository interviewsRepository)
        {
            _interviewsRepository = interviewsRepository;
            
        }

        public async Task<IEnumerable<InterviewsDTO>> GetAllInterviewsAsync()
        {
            var interviews = await _interviewsRepository.GetAllInterviews();
            return interviews.Select(i => new InterviewsDTO
            {
                InterviewsId = i.InterviewsId,
                Score = i.Score,
                Date = i.Date,
                ParentId = i.ParentId,
                CandidateId = i.CandidateId,
                Notes=i.Notes,
                Status =i.Status.ToString(),
            });
        }

        public async Task<InterviewsDTO> GetInterviewByIdAsync(int interviewId)
        {
            var interview = await _interviewsRepository.GetInterviewById(interviewId);
            if (interview == null)
                return null;

            return new InterviewsDTO
            {
                InterviewsId = interview.InterviewsId,
                Score = interview.Score,
                Date = interview.Date,
                ParentId = interview.ParentId,
                CandidateId = interview.CandidateId,
                Notes = interview.Notes,
                Status = interview.Status.ToString(),
            };
        }

        public async Task Create(InterviewsDTO entity)
        {
            if (Enum.TryParse(entity.Status, out InterviewStatus status))
            {
                var interview = new Interviews
                {
                    Score = entity.Score,
                    Date = entity.Date,
                    ParentId = entity.ParentId,
                    CandidateId = entity.CandidateId,
                    Notes = entity.Notes,
                    Status = status
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


            existingInterview.Score = entity.Score;
            existingInterview.Date = entity.Date;
            existingInterview.ParentId = entity.ParentId;
            existingInterview.CandidateId = entity.CandidateId;
            existingInterview.Notes = entity.Notes;
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
