// CMS.Domain.Interfaces/IInterviewsRepository.cs
using CMS.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CMS.Repository.Interfaces
{
    public interface IInterviewsRepository
    {
        //Task<IEnumerable<Interviews>> GetAllInterviews();
        //Task<Interviews> GetInterviewById(int interviewId);
        //Task Create(Interviews entity);
        //Task Update(Interviews entity);
        //Task Delete(Interviews entity);

        Task<int> Insert(Interviews entity);
        Task<int> Update(Interviews entity);
        Task<int> Delete(int id);
        Task<Interviews> GetById(int id);
        Task<List<Interviews>> GetAll();
    }
}
