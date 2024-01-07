using CMS.Application.Extensions;
using CMS.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CMS.Repository.Interfaces
{
    public interface IUserRepository
    {
        List<IdentityUser> GetAllUsersWithRoles();
        List<string> GetUserRoles(IdentityUser user);
        Task<bool> Delete(string id);

        IdentityUser GetUserById(string userId);
    }
}
