// CMS.Application/Services/InterviewsService.cs
using CMS.Application.DTOs;
using CMS.Application.Extensions;
using CMS.Domain.Entities;
using CMS.Domain.Enums;
using CMS.Repository.Implementation;
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

        public async Task<Result<InterviewsDTO>> Delete(int id)
        {
            try
            {
                await _interviewsRepository.Delete(id);
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
                    return Result<List<InterviewsDTO>>.Failure(null, "No career offers found");
                }

                var interviewsDTO = new List<InterviewsDTO>();
                foreach (var c in interviews)
                {

                    var com = new InterviewsDTO
                    {
                    
                       InterviewsId = c.InterviewsId,
                       Score = c.Score,
                       Status=c.Status.ToString(),
                       Date = c.Date,
                       PositionId = c.PositionId,
                       PositionName=c.Position.Name,
                       Notes=c.Notes,
                       ParentId=c.ParentId,
                       InterviewerId=c.InterviewerId,
                       CandidateId=c.CandidateId,
                       CandidateName=c.Candidate.FullName
                       

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
                var interview = await _interviewsRepository.GetById(id);
                var interviewDTO = new InterviewsDTO
                {
                   InterviewsId=interview.InterviewsId,
                    Score = interview.Score,
                    Status = interview.Status.ToString(),
                    Date = interview.Date,
                    PositionId = interview.PositionId,
                    PositionName = interview.Position.Name,
                    Notes = interview.Notes,
                    ParentId = interview.ParentId,
                    InterviewerId = interview.InterviewerId,
                    CandidateId=interview.CandidateId,
                    CandidateName=interview.Candidate.FullName


                };
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
            if (Enum.TryParse(data.Status, out InterviewStatus status)) { 
                var interview = new Interviews
            {
                PositionId = data.PositionId,
                CandidateId=data.CandidateId,
                Score=data.Score,
                Status= status,
                Date=data.Date,
                Notes=data.Notes,
                ParentId=data.ParentId,
                InterviewerId=data.InterviewerId,

                

            };
                await _interviewsRepository.Insert(interview);
            }

            
                

                return Result<InterviewsDTO>.Success(data);

           
                
        
            
        }

        public async Task<Result<InterviewsDTO>> Update(InterviewsDTO data)
        {

            if (data == null)
            {
                return Result<InterviewsDTO>.Failure(data, "can not update a null object");
            }
            if (Enum.TryParse(data.Status, out InterviewStatus status))
            {
                var interview = new Interviews
                {
                    InterviewsId = data.InterviewsId,
                    PositionId = data.PositionId,
                    CandidateId=data.CandidateId,
                    Score = data.Score,
                    ParentId = data.ParentId,
                    InterviewerId = data.InterviewerId,
                    Date = data.Date,
                    Notes = data.Notes,
                    Status = status




                };
                await _interviewsRepository.Update(interview);
            }
                return Result<InterviewsDTO>.Success(data);


          
        }










        //public async Task<IEnumerable<InterviewsDTO>> GetAllInterviewsAsync()
        //{
        //    var interviews = await _interviewsRepository.GetAllInterviews();
        //    return interviews.Select(i => new InterviewsDTO
        //    {
        //        InterviewsId = i.InterviewsId,
        //        Score = i.Score,
        //        Date = i.Date,
        //        ParentId = i.ParentId,
        //        InterviewerId=i.InterviewerId,
        //        Notes=i.Notes,
        //        Status =i.Status.ToString(),
        //        candidateDTO = new CandidateDTO
        //        {
        //            CandidateId = i.CandidateId,
        //            FullName = _candidateService.GetCandidateByIdAsync(i.CandidateId).Result?.FullName
        //        },
        //        //positionDTO = new PositionDTO
        //        //{
        //        //    PositionId = i.PositionId,
        //        //    Name = _positionService.GetById(i.PositionId).Result?.Name
        //        //}

        //    });
        //}

        //public async Task<InterviewsDTO> GetInterviewByIdAsync(int interviewId)
        //{
        //    var interview = await _interviewsRepository.GetInterviewById(interviewId);
        //    if (interview == null)
        //        return null;

        //    var candidateDTO = new CandidateDTO
        //    {
        //        CandidateId = interview.CandidateId,
        //        FullName = _candidateService.GetCandidateByIdAsync(interview.CandidateId).Result?.FullName,
        //    };

        //    //var positionDTO = new PositionDTO
        //    //{
        //    //    PositionId = interview.PositionId,
        //    //    Name = _positionService.GetById(interview.PositionId).Result?.Name,

        //    //};

        //    return new InterviewsDTO
        //    {
        //        InterviewsId = interview.InterviewsId,
        //        Score = interview.Score,
        //        Date = interview.Date,
        //        ParentId = interview.ParentId,
        //        InterviewerId = interview.InterviewerId,
        //        Notes = interview.Notes,

        //        //candidateDTO = candidateDTO,
        //        //positionDTO = positionDTO,

        //        FullName = candidateDTO.FullName,
        //       // Name=positionDTO.Name,

        //        Status = interview.Status.ToString(),
        //    };
        //}

        //public async Task Create(InterviewsDTO entity)
        //{
        //    if (Enum.TryParse(entity.Status, out InterviewStatus status))
        //    {
        //        var interview = new Interviews
        //        {
        //            Score = entity.Score,
        //            Date = entity.Date,
        //            ParentId = entity.ParentId,
        //            InterviewerId = entity.InterviewerId,
        //            Notes = entity.Notes,
        //            Status = status,

        //            CandidateId=entity.candidateDTO.CandidateId,
        //            //PositionId=entity.positionDTO.PositionId,
        //        };
        //        await _interviewsRepository.Create(interview);
        //    }
        //}

        //public async Task Update(int interviewId, InterviewsDTO entity)
        //{
        //    if (Enum.TryParse(entity.Status, out InterviewStatus status))
        //    {


        //        var existingInterview = await _interviewsRepository.GetInterviewById(interviewId);
        //    if (existingInterview == null)
        //        throw new Exception("Interview not found");


        //    existingInterview.Score = entity.Score;
        //    existingInterview.Date = entity.Date;
        //    existingInterview.ParentId = entity.ParentId;
        //    existingInterview.InterviewerId = entity.InterviewerId;
        //    existingInterview.Notes = entity.Notes;
        //    existingInterview.CandidateId = entity.candidateDTO.CandidateId;
        //    //existingInterview.PositionId = entity.positionDTO.PositionId;
        //    existingInterview.Status = status;

        //    await _interviewsRepository.Update(existingInterview);

        //    }


        //}

        //public async Task Delete(int interviewId)
        //{
        //    var interview = await _interviewsRepository.GetInterviewById(interviewId);
        //    if (interview != null)
        //        await _interviewsRepository.Delete(interview);
        //}








    }
}
