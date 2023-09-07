using CMS.Application.DTOs;
using CMS.Application.Extensions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CMS.Services.Interfaces
{
    public interface IAccountService
    {
        Task<List<Login>> GetAllUsersAsync();
        Task<bool> LoginAsync(Login collection);
        Task<bool> DeleteAccountAsync(string id);
        Task LogoutAsync();
    }
}
