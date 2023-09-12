// CMS.Domain.Interfaces/IInterviewsService.cs
using CMS.Application.DTOs;
using CMS.Application.Extensions;
using System.Collections.Generic;
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


    }
}
