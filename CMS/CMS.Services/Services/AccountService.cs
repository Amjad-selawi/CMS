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
        private readonly RoleManager<IdentityRole> _roleManager;

        public AccountService(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, ApplicationDbContext dbContext,
            IUserRepository userRepository, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _userRepository = userRepository;
            _roleManager = roleManager;
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



        public async Task<string> GetUserRoleAsync(IdentityUser user)
        {
            var roles = await _userManager.GetRolesAsync(user);
            return roles.FirstOrDefault(); 
        }

        public async Task<IdentityUser> GetUserByEmailAsync(string email)
        {
            return await _userManager.FindByEmailAsync(email);
        }



        public List<Register> GetAllUsersWithRoles()
        {
            var users = _userRepository.GetAllUsersWithRoles();
            var usersWithRoles = new List<Register>();

            foreach (var user in users)
            {
                var roles = _userRepository.GetUserRoles(user);

                usersWithRoles.Add(new Register
                {
                    RegisterrId = user.Id,
                    Email = user.Email,
                    UserName = user.UserName,
                    SelectedRole = roles.FirstOrDefault()
                });
            }

            return usersWithRoles;
        }


        public Register GetUsersById(string userId)
        {
            var user = _userRepository.GetUserById(userId);

            if (user == null)
            {
                return null; 
            }
            var roles = _userRepository.GetUserRoles(user);

            var userDetails = new Register
            {
                RegisterrId = user.Id,
                Email = user.Email,
                UserName = user.UserName,
                SelectedRole = roles.FirstOrDefault()
            };

            return userDetails;
        }




        public async Task<List<IdentityUser>> GetUsersInRole(string roleName)
        {
            var role = await _roleManager.FindByNameAsync(roleName);

            if (role == null)
            {
                throw new ApplicationException($"Role '{roleName}' not found.");
            }

            var usersInRole = await _userManager.GetUsersInRoleAsync(roleName);

            return usersInRole.ToList();
        }




    }
}
