using CMS.Application.DTOs;
using CMS.Application.Extensions;
using CMS.Domain;
using CMS.Domain.Entities;
using CMS.Repository.Interfaces;
using CMS.Services.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.UI.Services;
using System.Net;
using System.Net.Mail;

namespace CMS.Services.Services
{
    public class AccountService : IAccountService
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly ApplicationDbContext _dbContext;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUserRepository _userRepository;

        public AccountService(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, ApplicationDbContext dbContext,
            IUserRepository userRepository, RoleManager<IdentityRole> roleManager,
            IHttpContextAccessor httpContextAccessor

            )
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _dbContext = dbContext;
            _userRepository = userRepository;
            _roleManager = roleManager;
            _httpContextAccessor = httpContextAccessor;
        }

        public void LogException(string methodName, Exception ex, string createdByUserId, string additionalInfo)
        {
            _dbContext.Logs.Add(new Log
            {
                MethodName = methodName,
                ExceptionMessage = ex.Message,
                StackTrace = ex.StackTrace,
                LogTime = DateTime.Now,
                CreatedByUserId = createdByUserId,
                AdditionalInfo = additionalInfo
            });
            _dbContext.SaveChanges();
        }

        public string GetUserId()
        {
            var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return userId;
        }


        public async Task<List<Login>> GetAllUsersAsync()
        {
            try
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
           catch (Exception ex)
            {
                LogException(nameof(GetAllUsersAsync), ex,null,null);
                throw ex;
            }
        }

        public async Task<bool> LoginAsync(Login collection)
        
        {
            try
            { 
                var signedUser = await _userManager.FindByEmailAsync(collection.UserEmail);
            if (signedUser == null)
            {
                return false;
            }

            var result = await _signInManager.PasswordSignInAsync(signedUser.UserName, collection.Password, collection.RememberMe, false);
            
            return result.Succeeded;


            }
          
             catch (Exception ex)
            {
                LogException(nameof(LoginAsync), ex, null, null);
                throw ex;
            }
        }

        public async Task<bool> DeleteAccountAsync(string id, string ByUserId)
        {
            try
            {
                return await _userRepository.Delete(id);

            }
            catch (Exception ex)
            {
                LogException(nameof(DeleteAccountAsync), ex,$"Deleted By :{ByUserId}",$"Error while deleting Account with id :{id}");
                throw ex;
            }
        }

        public async Task LogoutAsync(string ByUserId)
        {
            try
            {
                await _signInManager.SignOutAsync();

            }
            catch (Exception ex)
            {
                LogException(nameof(LogoutAsync), ex, ByUserId, $"Error while Logout");
                throw ex;
            }
        }
        public async Task<Result<IList<IdentityUser>>> GetAllInterviewers()
        {
            try
            {
                var interviewerRole = await _roleManager.FindByNameAsync("Interviewer");
                if (interviewerRole == null)
                {
                    return Result<IList<IdentityUser>>.Failure(null, "Requested Role Not Found");
                }
                var interviewers = await _userManager.GetUsersInRoleAsync(interviewerRole.Name);
                if (interviewers == null)
                {
                    return Result<IList<IdentityUser>>.Failure(null, "No Interviewers found");
                }
                return Result<IList<IdentityUser>>.Success(interviewers);
            }
           

              catch (Exception ex)
            {
                LogException(nameof(GetAllInterviewers), ex,null, "Error while Getting All Interviewers");
                throw ex;
            }
        }
        public async Task<Result<IList<IdentityUser>>> GetAllArchitectureInterviewers()
        {
            try
            {
                var ArchiRole = await _roleManager.FindByNameAsync("Solution Architecture");
                if (ArchiRole == null)
                {
                    return Result<IList<IdentityUser>>.Failure(null, "Requested Role Not Found");
                }
                var architectures = await _userManager.GetUsersInRoleAsync(ArchiRole.Name);
                if (architectures == null)
                {
                    return Result<IList<IdentityUser>>.Failure(null, "No Architectures found");
                }
                return Result<IList<IdentityUser>>.Success(architectures);
            }


            catch (Exception ex)
            {
                LogException(nameof(GetAllArchitectureInterviewers), ex,null, "Error while Getting All Architecture");
                throw ex;
            }
        }
        public async Task<string> GetUserRoleAsync(IdentityUser user)
        {
            try
            {
                var roles = await _userManager.GetRolesAsync(user);
                return roles.FirstOrDefault();

            }

            catch (Exception ex)
            {
                LogException(nameof(GetUserRoleAsync), ex, null, "Error while Getting user role");
                throw ex;
            }
        }

        public async Task<IdentityUser> GetUserByEmailAsync(string email)
        {
            try
            {
                return await _userManager.FindByEmailAsync(email);
            }
            catch (Exception ex)
            {
                LogException(nameof(GetUserByEmailAsync), ex, null, "Error while Getting user by email");
                throw ex;
            }
        }
        public List<Register> GetAllUsersWithRoles()
        {
            try
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
           

              catch (Exception ex)
            {
                LogException(nameof(GetAllUsersWithRoles), ex, null, "Error while Getting all users with roles");
                throw ex;
            }
        }
        public Register GetUsersById(string userId)
        {
            try
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

            catch (Exception ex)
            {
                LogException(nameof(GetUsersById), ex, null, null);
                throw ex;
            }
        }


        //public async Task SendRegistrationEmail(IdentityUser user, string password)
        //{
        //    try
        //    {
        //        var receiver = $"{user.Email}";
        //        var subject = $"New User created in CMS system for you";
        //        var message = $"Dear {user.UserName},\n\n"
        //                   + $"Your account details:\n"
        //                   + $"Username: {user.UserName}\n"
        //                   + $"Email: {user.Email}\n"
        //                   + $"Password: {password}\n\n"
        //                   + $"Login to your account: [https://apps.sssprocess.com:6134/]";

        //         _emailSender.SendEmailAsync(receiver, subject, message);

        //    }
        //    catch (Exception ex)
        //    {
        //        LogException(nameof(SendRegistrationEmail), ex);
        //        throw ex;
        //    }
        //}

        public async Task SendRegistrationEmail(IdentityUser user, string password)
        {
            try
            {
                var smtp = new SmtpClient();
                smtp.Host = "mail.sssprocess.com";
                smtp.Port = 587;
                smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtp.EnableSsl = true;
                smtp.UseDefaultCredentials = true;
                string UserName = "notifications@sss-process.org";
                string Password = "P@ssw0rd";
                smtp.Credentials = new NetworkCredential(UserName, Password);




                var subject = "Welcome to CMS System ";
                var body = $"Dear {user.UserName},\n\n"
                   + $"Your account details:\n"
                   + $"Username: {user.UserName}\n"
                   + $"Email: {user.Email}\n"
                   + $"Password: {password}\n\n"
                   + $"Login to your account: [https://apps.sssprocess.com:6134/]";

        // Sender and recipient email addresses
        var fromAddress = new MailAddress(UserName);
        var toAddress = new MailAddress(user.Email, user.UserName);

        var message = new MailMessage(fromAddress, toAddress)
        {
            Subject = subject,
            Body = body,
            IsBodyHtml = true,
        };

        smtp.Send(message);

            }
            catch (Exception ex)
            {
                LogException(nameof(SendRegistrationEmail), ex, null, "Error while Registration");
                throw ex;
            }
        }


    }
}
