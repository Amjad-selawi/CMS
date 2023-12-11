using CMS.Application.DTOs;
using CMS.Application.Extensions;
using Microsoft.AspNetCore.Identity;
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
        Task<Result<IList<IdentityUser>>> GetAllInterviewers();
        Task<Result<IList<IdentityUser>>> GetAllArchitectureInterviewers();
        Task<string> GetUserRoleAsync(IdentityUser user);
        Task<IdentityUser> GetUserByEmailAsync(string email);
        List<Register> GetAllUsersWithRoles();
        Register GetUsersById(string userId);

        void LogException(string methodName, Exception ex, string additionalInfo);

        Task SendRegistrationEmail(IdentityUser user, string password);
    }
}
