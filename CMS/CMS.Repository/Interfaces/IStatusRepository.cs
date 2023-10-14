using CMS.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CMS.Repository.Interfaces
{
    public interface IStatusRepository
    {
        Task<int> Insert(Status entity);
        Task<List<Status>> GetAll();

        Task<Status> GetByCode(string co);
        Task<Status> GetById(int id);

        Task<Status> GetStatusByNameAsync(string statusName);

    }
}
