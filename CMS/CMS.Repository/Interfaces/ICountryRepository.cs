using CMS.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CMS.Repository.Interfaces
{
    public interface ICountryRepository
    {
        Task<int> Insert(Country entity);
        Task<int> Update(Country entity);
        Task<int> Delete(int id);
        Task<Country> GetById(int id);
        Task<List<Country>> GetAll();
    }
}
