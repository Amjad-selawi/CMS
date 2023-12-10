// CMS.Domain.Interfaces/IInterviewsService.cs
using CMS.Application.DTOs;
using CMS.Application.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace CMS.Services.Interfaces
{
    public interface IInterviewsService
    {
        Task UpdateInterviewAttachmentAsync(int id, string fileName, long fileSize, Stream fileStream, string ByUserId);
        Task ConductInterview(InterviewsDTO interviewsDTO, string ByUserId);
        Task<List<UsersDTO>> GetInterviewers();
        Task<Result<List<InterviewsDTO>>> MyInterviews();

        Task<Result<InterviewsDTO>> Insert(InterviewsDTO data,string ByUserId);
        Task<Result<List<InterviewsDTO>>> GetAll();
        Task<Result<InterviewsDTO>> Delete(int id, string ByUserId);
        Task<Result<InterviewsDTO>> GetById(int id, string ByUserId);
        Task<Result<InterviewsDTO>> Update(InterviewsDTO data, string ByUserId);

        Task<string> GetInterviewerName(string id);
        Task<string> GetArchitectureName(string id);
        Task<string> GetInterviewerRole(string id);

        Task <Result<List<InterviewsDTO>>> ShowHistory(int id);

        void LogException(string methodName, Exception ex, string createdByUserId = null, string additionalInfo = null);

    }
}
