using CMS.Application.DTOs;
using CMS.Application.Extensions;
using CMS.Domain;
using CMS.Repository.Interfaces;
using CMS.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMS.Services.Services
{
    public class AccountService : IAccountService
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IUserRepository _userRepository;

        public AccountService(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, ApplicationDbContext dbContext,
            IUserRepository userRepository)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _userRepository = userRepository;
        }


        public async Task<List<Login>> GetAllUsersAsync()
        {
            var users = _userManager.Users.ToList();
            List<Login> listuser = new List<Login>();
            for (int i = 0; i < users.Count; i++)
            {
                Login list = new Login();
                list.Id = users[i].Id;
                list.UserEmail = users[i].Email;
                list.UserName = users[i].UserName;
                list.Password = users[i].PasswordHash;
                listuser.Add(list);
            }
            return listuser;
        }

        public async Task<bool> LoginAsync(Login collection)
        {
            var result = await _signInManager.PasswordSignInAsync(collection.UserEmail, collection.Password, collection.RememberMe, false);
            return result.Succeeded;
        }

        public async Task<bool> DeleteAccountAsync(string id)
        {
            return await _userRepository.Delete(id);
        }

        public async Task LogoutAsync()
        {
            await _signInManager.SignOutAsync();
        }
    }
}
