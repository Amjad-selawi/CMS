﻿using CMS.Application.DTOs;
using CMS.Application.Extensions;
using CMS.Domain;
using CMS.Domain.Entities;
using CMS.Domain.Enums;
using CMS.Repository.Implementation;
using CMS.Repository.Interfaces;
using CMS.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace CMS.Services.Services
{
    public class SearchInterviewsService : ISearchInterviewsService
    {
        private readonly IInterviewsRepository _interviewsRepository;
        private readonly ICandidateService _candidateService;
        private readonly IPositionService _positionService;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IStatusRepository _statusRepository;
        private readonly ApplicationDbContext _db;
        private readonly IHttpContextAccessor _httpContextAccessor;

        private readonly IAttachmentService _attachmentService;

        public SearchInterviewsService(IInterviewsRepository interviewsRepository,
            ICandidateService candidateService,
            IPositionService positionService,
            IAttachmentService attachmentService,
            UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IHttpContextAccessor httpContextAccessor,
            IStatusRepository statusRepository,
            ApplicationDbContext Db
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
            _db = Db;
        }

        public async void LogException(string methodName, Exception ex, string additionalInfo)
        {
            var currentUser = await _userManager.GetUserAsync(_httpContextAccessor.HttpContext.User);
            var userId = currentUser?.Id;
            _db.Logs.Add(new Log
            {
                MethodName = methodName,
                ExceptionMessage = ex.Message,
                StackTrace = ex.StackTrace,CreatedByUserId = userId,
                LogTime = DateTime.Now,
                
                AdditionalInfo = additionalInfo
            });
            _db.SaveChanges();
        }

        public async Task<List<UsersDTO>> GetAllInterviewers()
        {
            try
            {
                // Get all roles
                var allRoles = await _roleManager.Roles.ToListAsync();

                // Initialize a list to store all users
                var allUsers = new List<UsersDTO>();

                // Iterate through each role
                foreach (var role in allRoles)
                {
                    // Check if the current role is not the "Admin" role
                    if (!string.Equals(role.Name, "Admin", StringComparison.OrdinalIgnoreCase))
                    {
                        // Get users for the current role
                        var usersInRole = await _userManager.GetUsersInRoleAsync(role.Name);

                        // Convert users to UsersDTO and add to the list
                        var usersDtoList = usersInRole.Select(user => new UsersDTO
                        {
                            Id = user.Id,
                            Name = user.UserName,
                            Email = user.Email,
                        }).ToList();

                        allUsers.AddRange(usersDtoList);
                    }
                }

                return allUsers;
            }
            catch (Exception ex)
            {
                LogException(nameof(GetAllInterviewers), ex, "Error while getting all users by role");
                throw ex;
            }
        }



        public async Task<List<UsersDTO>> GetInterviewers()
        {
            try
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
            catch (Exception ex)
            {
                LogException(nameof(GetInterviewers), ex,"Error while getting all Interviewers");
                throw ex;
            }

        }

        public async Task<string> GetInterviewerName(string id)
        {
            try
            {
                


                var user = await _userManager.FindByIdAsync(id);
            if (user != null)
            {
                return user.UserName;
            }

            return "User not found";
            }
            catch (Exception ex)
            {
                LogException(nameof(GetInterviewerName), ex, "Error while getting Interviewer Name");
                throw ex;
            }
        }

        public async Task<Result<InterviewsDTO>> Delete(int id)
        {
            try
            {
                

                var interview = await _interviewsRepository.GetById(id);
                if (interview != null && interview.AttachmentId != null)
                {
                    var attachmentToRemove = (int)interview.AttachmentId;
                    await _interviewsRepository.Delete(id);
                    await _attachmentService.DeleteAttachmentAsync(attachmentToRemove);

                }
                else
                {
                    await _interviewsRepository.Delete(id);
                }

                return Result<InterviewsDTO>.Success(null);
            }

            catch (Exception ex)
            {
                LogException(nameof(Delete), ex, $"Error while deleteing an interview with Id : {id}");
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
                    string userName = await GetInterviewerName(c.InterviewerId);
                    var com = new InterviewsDTO
                    {

                        InterviewsId = c.InterviewsId,
                        Score = c.Score,
                        StatusId = c.StatusId,
                        StatusName = c.Status.Name,
                        Date = c.Date,
                        PositionId = c.PositionId ,
                        Name = c.Position.Name,
                        Notes = c.Notes,
                        ParentId = c.ParentId,
                        InterviewerId = c.InterviewerId,
                        InterviewerName = userName,
                        CandidateId = c.CandidateId,
                        FullName = c.Candidate.FullName,
                        AttachmentId = c.AttachmentId,

                    };
                    interviewsDTO.Add(com);

                }
                return Result<List<InterviewsDTO>>.Success(interviewsDTO);


            }
            catch (Exception ex)
            {
                LogException(nameof(GetAll), ex,null);
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
                    InterviewsId = interview.InterviewsId,
                    Score = interview.Score,
                    StatusId = interview.StatusId,
                    StatusName = interview.Status.Name,
                    Date = interview.Date,
                    PositionId = interview.PositionId,
                    Name = interview.Position.Name,
                    Notes = interview.Notes,
                    ParentId = interview.ParentId,
                    InterviewerId = interview.InterviewerId,
                    CandidateId = interview.CandidateId,
                    FullName = interview.Candidate.FullName,
                    AttachmentId = interview.AttachmentId,
                };
                return Result<InterviewsDTO>.Success(interviewDTO);
            }
            catch (Exception ex)
            {
                LogException(nameof(GetById), ex, $"Error while Getting details with Id: {id}");
                return Result<InterviewsDTO>.Failure(null, $"unable to retrieve the Interview from the repository{ex.InnerException.Message}");
            }
        }

        public async Task<Result<InterviewsDTO>> Insert(InterviewsDTO data)
        {
            try
            {

                

                if (data == null)
            {
                return Result<InterviewsDTO>.Failure(data, "the interview  DTO is null");
            }

            //int attachmentId = await _attachmentService.CreateAttachmentAsync(data.FileName, (long)data.FileSize, data.FileData);
            //data.AttachmentId = attachmentId;
            var status = await _statusRepository.GetByCode(StatusCode.Pending);
            var interview = new Interviews
            {
                PositionId = data.PositionId,
                CandidateId = data.CandidateId,
                Score = data.Score,
                StatusId = status.Id,//data.StatusId,
                Date = data.Date,
                Notes = data.Notes,
                ParentId = data.ParentId,
                InterviewerId = data.InterviewerId,
                AttachmentId = data.AttachmentId,

            };
            await _interviewsRepository.Insert(interview);


            return Result<InterviewsDTO>.Success(data);
            }
            catch (Exception ex)
            {
                LogException(nameof(Insert), ex, $"Error while creating an interview with Id: {data.InterviewsId}");
                throw ex;
            }

        }

        public async Task<Result<InterviewsDTO>> Update(InterviewsDTO data)
        {
            try
            {
                



                if (data == null)
            {
                return Result<InterviewsDTO>.Failure(data, "can not update a null object");
            }
            //if (Enum.TryParse(data.Status, out InterviewStatus status))
            // {
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
                StatusId = (int)data.StatusId,
                AttachmentId = data.AttachmentId,

            };
            await _interviewsRepository.Update(interview);
            // }
            return Result<InterviewsDTO>.Success(data);
            }
            catch (Exception ex)
            {
                LogException(nameof(Update), ex,  $"Error while updating an interview with Id: {data.InterviewsId}");
                throw ex;
            }
        }

        public async Task UpdateInterviewAttachmentAsync(int id, string fileName, long fileSize, Stream fileStream)
        {
            try
            {

                

                var interview = await _interviewsRepository.GetById(id);
            int attachmentId = await _attachmentService.CreateAttachmentAsync(fileName, fileSize, fileStream);
            int attachmentToRemove = (int)interview.AttachmentId;
            interview.AttachmentId = attachmentId;
            await _interviewsRepository.Update(interview);
            await _attachmentService.DeleteAttachmentAsync(attachmentToRemove);
            }
            catch (Exception ex)
            {
                LogException(nameof(UpdateInterviewAttachmentAsync), ex,  $"Error while updating Interview Attachment with ID : {id}");
                throw ex;
            }
        }


        public async Task ConductInterview(InterviewsDTO entity)
        {
            try
            {
                

                var interview = await _interviewsRepository.GetById(entity.InterviewsId);
                int attachmentId = await _attachmentService.CreateAttachmentAsync(entity.FileName, (long)entity.FileSize, entity.FileData);
                entity.AttachmentId = attachmentId;

                if (interview == null)
                {
                    throw new Exception();
                }
                var managerId = "";

                var status = await _statusRepository.GetById((int)entity.StatusId);

                var manager = await _roleManager.FindByNameAsync("General Manger");
                if (manager != null)
                {
                    managerId = (await _userManager.GetUsersInRoleAsync(manager.Name)).FirstOrDefault().Id;

                }
                var HRId = "";
                var HR = await _roleManager.FindByNameAsync("HR");
                if (HR != null)
                {
                    HRId = (await _userManager.GetUsersInRoleAsync(HR.Name)).FirstOrDefault().Id;

                }
                bool ff = false;
                if (entity.InterviewerId == HRId)
                {
                    ff = true;
                }

                var nextInterviewer = "";

                if (interview.ParentId == null)
                {
                    nextInterviewer = managerId;

                }
                else
                {
                    nextInterviewer = HRId;

                }
                bool f = false;
                if (status.Code == StatusCode.Rejected)
                {
                    f = true;
                }
                var statID = await _statusRepository.GetByCode(StatusCode.Pending);
                if (!f && !ff)
                {
                    var newInterview = new Interviews
                    {
                        //Score = entity.Score,
                        StatusId = statID.Id,
                        Date = interview.Date,
                        CandidateId = interview.CandidateId,
                        PositionId = interview.PositionId,

                        InterviewerId = nextInterviewer,
                        //  AttachmentId = entity.AttachmentId,
                        // Notes = entity.Notes,
                        ParentId = entity.InterviewsId

                    };
                    await _interviewsRepository.Insert(newInterview);
                }
                var updatedInterview = new Interviews
                {
                    InterviewsId = entity.InterviewsId,
                    Score = entity.Score,
                    Date = interview.Date,
                    StatusId = (int)entity.StatusId,
                    CandidateId = interview.CandidateId,
                    PositionId = interview.PositionId,
                    InterviewerId = interview.InterviewerId,
                    AttachmentId = entity.AttachmentId,
                    Notes = entity.Notes,
                    ParentId = interview.ParentId
                };


                await _interviewsRepository.Update(updatedInterview);


            }
            catch (Exception ex)
            {
                LogException(nameof(ConductInterview), ex, "Error while interviwing");
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
                    interviewsDTOs.Add(new InterviewsDTO
                    {
                        InterviewsId = i.InterviewsId,
                        InterviewerId = i.InterviewerId,
                        InterviewerName = userName,
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
                        AttachmentId = i.AttachmentId

                    });


                }
                return Result<List<InterviewsDTO>>.Success(interviewsDTOs);


            }
            catch (Exception ex)
            {
                LogException(nameof(MyInterviews), ex,"Error while getting my interviews");
                return Result<List<InterviewsDTO>>.Failure(null, $"Unable to get interviews: {ex.InnerException.Message}");
            }
        }








    }
}
