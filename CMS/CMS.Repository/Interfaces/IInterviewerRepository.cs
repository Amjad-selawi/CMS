using CMS.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CMS.Repository.Interfaces
{
    public interface IInterviewerRepository
    {
        Task<IEnumerable<Interviewer>> GetAllInterviewersAsync();
        Task<Interviewer> GetInterviewerByIdAsync(int id);
        Task CreateInterviewersAsync(Interviewer interviewer);
        Task UpdateInterviewerAsync(Interviewer interviewer);
        Task DeleteInterviewerAsync(Interviewer Interviewer);
    }
}
