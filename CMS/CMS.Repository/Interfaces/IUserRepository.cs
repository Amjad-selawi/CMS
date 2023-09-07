using CMS.Application.Extensions;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CMS.Repository.Interfaces
{
    public interface IUserRepository
    {
        Task<bool> Delete(string id);
        Task<string> Register(IdentityUser user, string password);
    }
}
