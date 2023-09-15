using CMS.Application.DTOs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace CMS.Services.Interfaces
{
    public interface IInterviewerService
    {
        Task<IEnumerable<InterviewerDTOs>> GetAllInterviewersAsync();
        Task<InterviewerDTOs> GetInterviewerByIdAsync(int id);
        Task CreateInterviewerAsync(InterviewerDTOs interviewerDTOs);
        Task UpdateInterviewerAsync(int id, InterviewerDTOs interviewerDTOs);
       
        Task DeleteInterviewerAsync(int id);
    }
}
