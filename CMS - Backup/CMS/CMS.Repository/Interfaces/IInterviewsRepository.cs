// CMS.Domain.Interfaces/IInterviewsRepository.cs
using CMS.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CMS.Repository.Interfaces
{
    public interface IInterviewsRepository
    {
        Task<int> Insert(Interviews entity);
        Task<int> Update(Interviews entity);
        Task<int> Delete(int id);
        Task<Interviews> GetById(int id);
        Task<Interviews> GetByIdForEdit(int id);
        Task<List<Interviews>> GetAll();
        Task<List<Interviews>> GetCurrentInterviews(string id);

        Task<string> GetInterviewerEmail(string interviewerId);
        Task<string> GetGeneralManagerEmail();

        Task<string> GetHREmail();


    }
}
