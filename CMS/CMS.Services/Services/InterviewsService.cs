// CMS.Application/Services/InterviewsService.cs
using CMS.Application.DTOs;
using CMS.Application.Extensions;
using CMS.Domain.Entities;
using CMS.Domain.Enums;
using CMS.Repository.Implementation;
using CMS.Repository.Interfaces;
using CMS.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Formatters.Xml;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;

namespace CMS.Services.Services
{
    public class InterviewsService : IInterviewsService
    {
        private readonly IInterviewsRepository _interviewsRepository;
        private readonly ICandidateService _candidateService;
        private readonly IPositionService _positionService;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IStatusRepository _statusRepository;

        private readonly IHttpContextAccessor _httpContextAccessor;

        private readonly IAttachmentService _attachmentService;

        public InterviewsService(IInterviewsRepository interviewsRepository,
            ICandidateService candidateService,
            IPositionService positionService,
            IAttachmentService attachmentService,
            UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IHttpContextAccessor httpContextAccessor,
            IStatusRepository statusRepository
            )
        {
            _interviewsRepository = interviewsRepository;
            _candidateService = candidateService;
            _positionService = positionService;
            _attachmentService = attachmentService;
            _userManager = userManager;
            _roleManager = roleManager;
            _httpContextAccessor = httpContextAccessor;
            _statusRepository = statusRepository;
        }


        public async Task<List<UsersDTO>> GetInterviewers()
        {
            var interviewerRole = await _roleManager.FindByNameAsync("Interviewer");
            if (interviewerRole == null)
            {
                return new List<UsersDTO>();
            }
            var usersInRole = await _userManager.GetUsersInRoleAsync(interviewerRole.Name);

            var interviewers = usersInRole.Select(user => new UsersDTO
            {
                Id = user.Id,
                Name = user.UserName,
                Email = user.Email,
            }).ToList();
            return interviewers;

        }

        public async Task<string> GetInterviewerName(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user != null)
            {
                return user.UserName;
            }

            return "User not found";
        }
        public async Task<string> GetInterviewerRole(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user != null)
            {
                var roles = await _userManager.GetRolesAsync(user);
                if (roles.Any())
                {
                    return roles[0];
                }
            }

            return "User not found";
        }

        public async Task<Result<InterviewsDTO>> Delete(int id)
        {
            try
            {
                var interview = await _interviewsRepository.GetById(id);
                if (interview != null && interview.AttachmentId != null)
                {
                    var attachmentToRemove = interview.AttachmentId;
                    await _interviewsRepository.Delete(id);
                    if (attachmentToRemove != null)
                    {
                        await _attachmentService.DeleteAttachmentAsync((int)attachmentToRemove);
                    }

                }
                else
                {
                    await _interviewsRepository.Delete(id);
                }

                return Result<InterviewsDTO>.Success(null);
            }

            catch (Exception ex)
            {
                return Result<InterviewsDTO>.Failure(null, $"An error occurred while deleting the Interview{ex.InnerException.Message}");
            }
        }

       public async Task<Result<List<InterviewsDTO>>>ShowHistory(int id){

            

            List<InterviewsDTO> interviewsDTOs = new List<InterviewsDTO>();
            try
            {
                var Result = await GetById(id);
                var interview = Result.Value;
                while (interview.ParentId != null)
                {
                    var result = await GetById((int)interview.ParentId);
                    interview= result.Value;
                    interviewsDTOs.Add(result.Value);
                }
                return Result<List<InterviewsDTO>>.Success(interviewsDTOs);

            }
            catch (Exception ex)
            {
                return Result<List<InterviewsDTO>>.Failure(null, $"Unable to get interview History: {ex.Message}");
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
                    string userName = await GetInterviewerName(c.InterviewerId);
                    string SeconduserName = await GetInterviewerName(c.SecondInterviewerId);

                    string interviewerRole = await GetInterviewerRole(c.InterviewerId);
                    var com = new InterviewsDTO
                    {

                        InterviewsId = c.InterviewsId,
                        Score = c.Score,
                        StatusId = c.StatusId,
                        StatusName = c.Status.Name,
                        Date = c.Date,
                        PositionId = c.PositionId,
                        Name = c.Position.Name,
                        EvalutaionFormId=c.Position.EvaluationId,
                        Notes = c.Notes,
                        ParentId = c.ParentId,
                        InterviewerId = c.InterviewerId,
                        InterviewerName = userName,
                        CandidateId = c.CandidateId,
                        FullName = c.Candidate.FullName,
                        CandidateCVAttachmentId=c.Candidate.CVAttachmentId,
                        AttachmentId = c.AttachmentId,
                        InterviewerRole = interviewerRole,
                        ActualExperience= c.ActualExperience,
                        SecondInterviewerId = c.SecondInterviewerId,
                        SecondInterviewerName = SeconduserName,

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
                string userName = await GetInterviewerName(interview.InterviewerId);
                string SeconduserName = await GetInterviewerName(interview.SecondInterviewerId);

                string interviewerRole = await GetInterviewerRole(interview.InterviewerId);
                var interviewDTO = new InterviewsDTO
                {
                    InterviewsId = interview.InterviewsId,
                    Score = interview.Score,
                    StatusId = interview.StatusId,
                    StatusName = interview.Status.Name,
                    Date = interview.Date,
                    PositionId = interview.PositionId,
                    Name = interview.Position.Name,
                    EvalutaionFormId=interview.Position.EvaluationId,
                    Notes = interview.Notes,
                    ParentId = interview.ParentId,
                    InterviewerId = interview.InterviewerId,
                    InterviewerName = userName,
                    CandidateId = interview.CandidateId,
                    FullName = interview.Candidate.FullName,
                    CandidateCVAttachmentId=interview.Candidate.CVAttachmentId,
                    AttachmentId = interview.AttachmentId,
                    InterviewerRole = interviewerRole,
                    ActualExperience = interview.ActualExperience,
                    SecondInterviewerId = interview.SecondInterviewerId,
                    SecondInterviewerName = SeconduserName,
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
            var status = await _statusRepository.GetByCode(StatusCode.Pending);
            var currentUser = await _userManager.GetUserAsync(_httpContextAccessor.HttpContext.User);
            var interview = new Interviews
            {
                PositionId = data.PositionId,
                CandidateId = data.CandidateId,
                Score = data.Score,
                StatusId = status.Id,
                Date = data.Date,
                Notes = data.Notes,
                ParentId = data.ParentId,
                InterviewerId = data.InterviewerId,
                AttachmentId = data.AttachmentId,
                CreatedBy = currentUser.Id,
                CreatedOn = DateTime.Now,
                SecondInterviewerId = data.SecondInterviewerId,


            };
            await _interviewsRepository.Insert(interview);


            return Result<InterviewsDTO>.Success(data);


        }

        public async Task<Result<InterviewsDTO>> Update(InterviewsDTO data)
        {

            if (data == null)
            {
                return Result<InterviewsDTO>.Failure(data, "can not update a null object");
            }
            var currentUser = await _userManager.GetUserAsync(_httpContextAccessor.HttpContext.User);
            //if (Enum.TryParse(data.Status, out InterviewStatus status))
            // {
            var previouseInterview = await _interviewsRepository.GetByIdForEdit(data.InterviewsId);
            var interview = new Interviews
            {
                InterviewsId = data.InterviewsId,
                PositionId = data.PositionId,
                CandidateId = data.CandidateId,
                Score = data.Score,
                ParentId = data.ParentId,
                InterviewerId = data.InterviewerId,
                SecondInterviewerId = data.SecondInterviewerId,

                Date = data.Date,
                Notes = data.Notes,
                StatusId = (int)data.StatusId,
                AttachmentId = data.AttachmentId,
                ModifiedOn = DateTime.Now,
                ModifiedBy = currentUser.Id,
                CreatedBy = previouseInterview.CreatedBy,
                CreatedOn = previouseInterview.CreatedOn,

            };
            await _interviewsRepository.Update(interview);
            // }
            return Result<InterviewsDTO>.Success(data);

        }

        public async Task UpdateInterviewAttachmentAsync(int id, string fileName, long fileSize, Stream fileStream)
        {
            var interview = await _interviewsRepository.GetById(id);
            int attachmentId = await _attachmentService.CreateAttachmentAsync(fileName, fileSize, fileStream);
            int attachmentToRemove = (int)interview.AttachmentId;
            interview.AttachmentId = attachmentId;
            await _interviewsRepository.Update(interview);
            await _attachmentService.DeleteAttachmentAsync(attachmentToRemove);
        }


        public async Task ConductInterview(InterviewsDTO completedDTO)
        {
            try
            {
                var currentUser = await _userManager.GetUserAsync(_httpContextAccessor.HttpContext.User);

                var interview = await _interviewsRepository.GetById(completedDTO.InterviewsId);
                if (completedDTO.FileData != null)
                {
                    int attachmentId = await _attachmentService.CreateAttachmentAsync(completedDTO.FileName, (long)completedDTO.FileSize, completedDTO.FileData);
                    completedDTO.AttachmentId = attachmentId;
                }
                
                Debug.Assert(interview != null, "No Interview Provided for Conduct Interview Method");
                // Step 1: Update Completed Interview
                interview.StatusId = (int)completedDTO.StatusId;
                interview.Score = completedDTO.Score;
                interview.Notes = completedDTO.Notes;
                interview.ActualExperience = completedDTO.ActualExperience;
                interview.AttachmentId = completedDTO.AttachmentId;
                interview.ModifiedBy = currentUser.Id;
                interview.ModifiedOn = DateTime.Now;
                interview.IsUpdated = true;
                await _interviewsRepository.Update(interview);
                // Step 2: Create Next Interview if Needed.
                var Completedstatus = await _statusRepository.GetById((int)completedDTO.StatusId);
                bool isApproved = Completedstatus.Code == StatusCode.Approved;
                bool isLastInterviewerAnHR = await _userManager.IsInRoleAsync(interview.Interviewer, "HR Manager");
                if (isApproved && !isLastInterviewerAnHR) // There is a next interview
                {
                    bool isFirstMeeting = interview.ParentId == null;
                    var PendeingStatus = await _statusRepository.GetByCode(StatusCode.Pending);
                    var newInterview = new Interviews
                    {
                        StatusId = PendeingStatus.Id,
                        Date =interview.Date,
                        CandidateId = interview.CandidateId,
                        PositionId = interview.PositionId,
                        ParentId = completedDTO.InterviewsId,
                        CreatedOn = DateTime.Now,
                        CreatedBy = currentUser.Id,
                    };
                    if (isFirstMeeting) // Second Interview Needed which done by General Manager
                    {
                        var manager = (await _userManager.GetUsersInRoleAsync("General Manager")).FirstOrDefault();
                        Debug.Assert(manager != null, "There is No Valid General Manager in The System");
                        newInterview.InterviewerId = manager.Id;
                    }
                    else // Third Interview Needed which done by HR Manager
                    {
                        var hr = (await _userManager.GetUsersInRoleAsync("HR Manager")).FirstOrDefault();
                        Debug.Assert(hr != null, "There is No Valid HR Manager in The System");
                        newInterview.InterviewerId = hr.Id;
                    }
                    await _interviewsRepository.Insert(newInterview);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<Result<List<InterviewsDTO>>> MyInterviews()
        {
            try
            {
                var user = await _userManager.GetUserAsync(_httpContextAccessor.HttpContext.User);

                if (user == null)
                {
                    return Result<List<InterviewsDTO>>.Failure(null, "User not found.");
                }

                var interviews = await _interviewsRepository.GetCurrentInterviews(user.Id);
                if (interviews == null)
                {
                    return Result<List<InterviewsDTO>>.Failure(null, "no available interviews.");
                }
                var interviewsDTOs = new List<InterviewsDTO>();
                foreach (var i in interviews)
                {
                    string userName = await GetInterviewerName(i.InterviewerId);
                    string SeconduserName = await GetInterviewerName(i.SecondInterviewerId);

                    interviewsDTOs.Add(new InterviewsDTO
                    {
                        InterviewsId = i.InterviewsId,
                        InterviewerId = i.InterviewerId,
                        InterviewerName = userName,
                        SecondInterviewerId = i.SecondInterviewerId,
                        SecondInterviewerName = SeconduserName,
                        Score = i.Score,
                        StatusId = i.StatusId,
                        StatusName = i.Status.Name,
                        Date = i.Date,
                        PositionId = i.PositionId,
                        Name = i.Position.Name,
                        Notes = i.Notes,
                        ParentId = i.ParentId,
                        CandidateId = i.CandidateId,
                        FullName = i.Candidate.FullName,
                        CandidateCVAttachmentId=i.Candidate.CVAttachmentId,
                        AttachmentId = i.AttachmentId,
                        modifiedBy = i.ModifiedBy,
                        isUpdated = i.IsUpdated,
                        ActualExperience=i.ActualExperience

                    });


                }
                return Result<List<InterviewsDTO>>.Success(interviewsDTOs);


            }
            catch (Exception ex)
            {
                return Result<List<InterviewsDTO>>.Failure(null, $"Unable to get interviews: {ex.InnerException.Message}");
            }
        }



    }
}
