using CMS.Application.Extensions;
using CMS.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMS.Repository.Interfaces
{
    public interface ICompanyRepository
    {
        Task<int> Insert(Company entity);
        Task<int> Update(Company entity);
        Task <int>Delete(int id);
        Task<Company> GetById(int id);
        Task<List<Company>> GetAll();

        bool DoesCompanyNameExist(string name);

    }
}
