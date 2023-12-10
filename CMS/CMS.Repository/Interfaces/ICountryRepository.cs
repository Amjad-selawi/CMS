using CMS.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CMS.Repository.Interfaces
{
    public interface ICountryRepository
    {
        Task<int> Insert(Country entity, string UserId);
        Task<int> Update(Country entity, string UserId);
        int Delete(int id, string UserId);
        //Task<int> Delete(int id);
        Task<Country> GetById(int id, string UserId);
        Task<List<Country>> GetAll();

        bool DoesCountryNameExist(string name);
        Task<IEnumerable<Country>> GetAllCountriesAsync();

        void LogException(string methodName, Exception ex, string createdByUserId, string additionalInfo);
    }
}
