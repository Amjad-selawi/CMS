using CMS.Application.DTOs;
using CMS.Application.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace CMS.Services.Interfaces
{
    public interface ISearchInterviewsService
    {
        Task<Result<InterviewsDTO>> Insert(InterviewsDTO data);
        Task<Result<List<InterviewsDTO>>> GetAll();
        Task<Result<InterviewsDTO>> Delete(int id);
        Task<Result<InterviewsDTO>> GetById(int id);
        Task<Result<InterviewsDTO>> Update(InterviewsDTO data);
        Task UpdateInterviewAttachmentAsync(int id, string fileName, long fileSize, Stream fileStream);
        Task ConductInterview(InterviewsDTO interviewsDTO);
        Task<List<UsersDTO>> GetInterviewers();
        Task<Result<List<InterviewsDTO>>> MyInterviews();

        Task<string> GetInterviewerName(string id);




    }
}
