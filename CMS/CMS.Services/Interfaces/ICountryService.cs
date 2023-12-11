using CMS.Application.DTOs;
using CMS.Application.Extensions;
using CMS.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CMS.Services.Interfaces
{
    public interface ICountryService
    {
        Task<Result<CountryDTO>> Insert(CountryDTO data, string ByUserId);
        Task<Result<List<CountryDTO>>> GetAll(string ByUserId);
        Result<CountryDTO> Delete(int id, string ByUserId);

        //Task<Result<CountryDTO>> Delete(int id);
        Task<Result<CountryDTO>> GetById(int id, string ByUserId);
        Task<Result<CountryDTO>> Update(CountryDTO data, string ByUserId);

        string GetUserId();
        bool DoesCountryNameExist(string name);
        Task<IEnumerable<Country>> GetAllCountriesAsync(string ByUserId);


        void LogException(string methodName, Exception ex, string createdByUserId = null, string additionalInfo = null);
    }
}
