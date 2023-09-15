// CMS.Application/Services/InterviewsService.cs
using CMS.Application.DTOs;
using CMS.Application.Extensions;
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
        private readonly IInterviewsRepository _interviewsRepository;
        private readonly ICandidateService _candidateService;
        private readonly IPositionService _positionService;

        public InterviewsService(IInterviewsRepository interviewsRepository, ICandidateService candidateService, IPositionService positionService)
        {
            _interviewsRepository = interviewsRepository;
            _candidateService = candidateService;
            _positionService = positionService;
        }

        public async Task<Result<InterviewsDTO>> Delete(int id)
        {
            try
            {
                await _interviewsRepository.Delete(id)
;
                return Result<InterviewsDTO>.Success(null);
            }

            catch (Exception ex)
            {
                return Result<InterviewsDTO>.Failure(null, $"An error occurred while deleting the Interview{ex.InnerException.Message}");
            }
        }

        public async Task<Result<List<InterviewsDTO>>> GetAll()
        {
            try
            {
                var interviews = await _interviewsRepository.GetAll();
                if (interviews == null)
                {
                    return Result<List<InterviewsDTO>>.Failure(null, "No Interviews found");
                }

                var interviewsDTO = new List<InterviewsDTO>();
                foreach (var c in interviews)
                {

                    var com = new InterviewsDTO
                    {

                        InterviewsId = c.InterviewsId,
                        Score = c.Score,
                        Status = (int)c.Status,
                        Date = c.Date,
                        PositionId = c.PositionId,
                        Notes = c.Notes,
                        ParentId = c.ParentId,
                        InterviewerId = c.InterviewerId,
                        CandidateId = c.CandidateId,
                    };
                    interviewsDTO.Add(com);

                }
                return Result<List<InterviewsDTO>>.Success(interviewsDTO);


            }
            catch (Exception ex)
            {
                return Result<List<InterviewsDTO>>.Failure(null, $"Unable to get Interview: {ex.InnerException.Message}");
            }
        }

        public async Task<Result<InterviewsDTO>> GetById(int id)
        {
            if (id <= 0)
            {
                return Result<InterviewsDTO>.Failure(null, "Invalid interview id");
            }
            try
            {
                var interview = await _interviewsRepository.GetById(id)
;
                var interviewDTO = new InterviewsDTO
                {
                    InterviewsId = interview.InterviewsId,
                    Score = interview.Score,
                    Date = interview.Date,
                    PositionId = interview.PositionId,
                    Notes = interview.Notes,
                    ParentId = interview.ParentId,
                    InterviewerId = interview.InterviewerId,
                    CandidateId = interview.CandidateId,
                };
                if (interview.Status != null)
                {
                    interviewDTO.Status = (int)interview.Status;
                }
                return Result<InterviewsDTO>.Success(interviewDTO);
            }
            catch (Exception ex)
            {
                return Result<InterviewsDTO>.Failure(null, $"unable to retrieve the Interview from the repository{ex.InnerException.Message}");
            }
        }

        public async Task<Result<InterviewsDTO>> Insert(InterviewsDTO data)
        {
            if (data == null)
            {
                return Result<InterviewsDTO>.Failure(data, "the interview  DTO is null");
            }
            var interview = new Interviews
            {
                InterviewsId = data.InterviewsId,
                PositionId = data.PositionId,
                CandidateId = data.CandidateId,
                Score = data.Score,
                ParentId = data.ParentId,
                InterviewerId = data.InterviewerId,
                Date = data.Date,
                Notes = data.Notes,
                
            };
            if(data.Status != null)
            {
                interview.Status = (InterviewStatus)data.Status;
            }
            await _interviewsRepository.Insert(interview);
            
            return Result<InterviewsDTO>.Success(data);
        }

        public async Task<Result<InterviewsDTO>> Update(InterviewsDTO data)
        {

            if (data == null)
            {
                return Result<InterviewsDTO>.Failure(data, "can not update a null object");
            }
            var interview = new Interviews
            {
                InterviewsId = data.InterviewsId,
                PositionId = data.PositionId,
                CandidateId = data.CandidateId,
                Score = data.Score,
                ParentId = data.ParentId,
                InterviewerId = data.InterviewerId,
                Date = data.Date,
                Notes = data.Notes,
                Status = (InterviewStatus)data.Status
            };
            await _interviewsRepository.Update(interview);
            return Result<InterviewsDTO>.Success(data);
        }
        public async Task<Result<List<InterviewViewDTO>>> GetAllWithLabels()
        {
            try
            {
                var interviews = await _interviewsRepository.GetAll();
                if (interviews == null)
                {
                    return Result<List<InterviewViewDTO>>.Failure(null, "No Interviews found");
                }

                var interviewsDTO = new List<InterviewViewDTO>();
                foreach (var c in interviews)
                {

                    var com = new InterviewViewDTO
                    {

                        InterviewsId = c.InterviewsId,
                        Score = c.Score,
                        Date = c.Date,
                        Notes = c.Notes,
                        PositionName = c.Position.Name,
                        InterviewerName = c.Interviewer.UserName,
                        CandidateName= c.Candidate.FullName,
                    };
                    if(c.Status != null)
                    {
                        com.Status = (int)c.Status;
                    }
                    interviewsDTO.Add(com);

                }
                return Result<List<InterviewViewDTO>>.Success(interviewsDTO);


            }
            catch (Exception ex)
            {
                return Result<List<InterviewViewDTO>>.Failure(null, $"Unable to get Interview: {ex.InnerException.Message}");
            }
        }
        public async Task<Result<InterviewViewDTO>> GetByIdWithLabels(int id)
        {
            if (id <= 0)
            {
                return Result<InterviewViewDTO>.Failure(null, "Invalid interview id");
            }
            try
            {
                var interview = await _interviewsRepository.GetById(id)
;
                var interviewDTO = new InterviewViewDTO
                {
                    InterviewsId = interview.InterviewsId,
                    Score = interview.Score,
                    Date = interview.Date,
                    Notes = interview.Notes,
                    PositionName = interview.Position.Name,
                    InterviewerName = interview.Interviewer.UserName,
                    CandidateName = interview.Candidate.FullName,
                };
                if(interview.Status != null)
                {
                    interviewDTO.Status = (int)interview.Status;
                }
                return Result<InterviewViewDTO>.Success(interviewDTO);
            }
            catch (Exception ex)
            {
                return Result<InterviewViewDTO>.Failure(null, $"unable to retrieve the Interview from the repository{ex.InnerException.Message}");
            }
        }
    }
}
