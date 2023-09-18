// CMS.Domain.Interfaces/IInterviewsService.cs
using CMS.Application.DTOs;
using CMS.Application.Extensions;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace CMS.Services.Interfaces
{
    public interface IInterviewsService
    {
        //Task<IEnumerable<InterviewsDTO>> GetAllInterviewsAsync();
        //Task<InterviewsDTO> GetInterviewByIdAsync(int interviewId);
        //Task Create(InterviewsDTO entity);
        //Task Update(int interviewId, InterviewsDTO entity);
        ////Task Update(InterviewsDTO entity);

        //Task Delete(int interviewId);


        Task<Result<InterviewsDTO>> Insert(InterviewsDTO data);
        Task<Result<List<InterviewsDTO>>> GetAll();
        Task<Result<InterviewsDTO>> Delete(int id);
        Task<Result<InterviewsDTO>> GetById(int id);
        Task<Result<InterviewsDTO>> Update(InterviewsDTO data);


        Task<List<UsersDTO>> GetInterviewers();

        Task<string> GetInterviewerName(string id);

        Task UpdateInterviewAttachmentAsync(int id, string fileName, long fileSize, Stream fileStream);

        Task ConductInterview(InterviewsDTO entity);

        Task<Result<List<InterviewsDTO>>> MyInterviews();




    }
}
