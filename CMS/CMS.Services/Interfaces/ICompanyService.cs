using CMS.Application.DTOs;
using CMS.Application.Extensions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CMS.Services.Interfaces
{
    public interface ICompanyService
    {
        Task<Result<CompanyDTO>> Insert(CompanyDTO data,string ByUserId);
        Task<Result<List<CompanyDTO>>> GetAll(string ByUserId);
        Task<Result<CompanyDTO>> Delete(int id, string ByUserId);
        Task<Result<CompanyDTO>> GetById(int id, string ByUserId);
        Task<Result<CompanyDTO>> Update(CompanyDTO data, string ByUserId);

        bool DoesCompanyNameExist(string name, int countryId);
        string GetUserId();
        void LogException(string methodName, Exception ex, string createdByUserId = null, string additionalInfo = null);
    }
}
