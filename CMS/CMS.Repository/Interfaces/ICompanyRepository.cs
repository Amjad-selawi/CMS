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
        Task<int> Insert(Company entity,string userId);
        Task<int> Update(Company entity, string userId);
        Task <int>Delete(int id, string userId);
        Task<Company> GetById(int id, string userId);
        Task<List<Company>> GetAll();

        bool DoesCompanyNameExist(string name, int countryId);
        void LogException(string methodName, Exception ex, string createdByUserId, string additionalInfo);


    }
}
