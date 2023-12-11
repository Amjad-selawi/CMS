﻿using CMS.Application.DTOs;
using CMS.Domain;
using CMS.Domain.Entities;
using CMS.Services.Interfaces;
using CMS.Services.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace CMS.Web.Controllers
{
    //Test
    public class AccountController : Controller
    {
        SignInManager<IdentityUser> _signInManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IAccountService _accountService;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ApplicationDbContext Db;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AccountController(IAccountService accountService, UserManager<IdentityUser> userManager, 
            ApplicationDbContext _db, RoleManager<IdentityRole> roleManager, 
            SignInManager<IdentityUser> signInManager,IHttpContextAccessor httpContextAccessor)
        {
            _accountService = accountService;
            _userManager = userManager;
            Db = _db;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _httpContextAccessor = httpContextAccessor;
        }

        public void LogException(string methodName, Exception ex, string additionalInfo = null)
        {
            var createdByUserId = GetUserId();
            _accountService.LogException(methodName, ex, createdByUserId, additionalInfo);
        }
        public string GetUserId()
        {
            try
            {
                var userId = _accountService.GetUserId();
                return userId;
            }
            catch (Exception ex)
            {
                LogException(nameof(GetUserId), ex, null);
                throw ex;
            }
        }


        //GET
        public async Task<ActionResult> Login()
        {
            try
            {

            
            if (_signInManager.IsSignedIn(User))
            {

                if (User.IsInRole("HR Manager") || User.IsInRole("Admin") || User.IsInRole("General Manager") )
                {
                    return RedirectToAction("Index", "Dashboard");
                }
                else if (User.IsInRole("Interviewer") || User.IsInRole("Solution Architecture"))
                {
                    return RedirectToAction("MyInterviews", "Interviews");
                }
                else
                {
                    return RedirectToAction("Index", "Home");
                }

            }
            else
            {
                return View();
            }
            }
            catch (Exception ex)
            {
                LogException(nameof(Login), ex, "Faild to went to the Login Page");
                throw ex;
            }
        }

        //POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(Login collection)
        {
            try
            {

                //if (!ModelState.IsValid)
                //{
                //    ModelState.AddModelError("", "Email or Password not correct..!");
                //    return View();
                //}
                if (ModelState.IsValid)
                {
                    var result = await _accountService.LoginAsync(collection);
                    if (result)
                    {
                        if (_signInManager.IsSignedIn(User))
                        {
                            if (User.IsInRole("HR Manager") || User.IsInRole("Admin") || User.IsInRole("General Manager"))
                            {
                                return RedirectToAction("Index", "Dashboard");
                            }
                            else if (User.IsInRole("Interviewer") || User.IsInRole("Solution Architecture"))
                            {
                                return RedirectToAction("MyInterviews", "Interviews");
                            }
                            else
                            {
                                return RedirectToAction("Index", "Home");
                            }
                        }
                        else
                        {
                            return RedirectToAction("Index", "Home");
                        }
                    }
                    else
                    {
                        var user = await _accountService.GetUserByEmailAsync(collection.UserEmail);
                        if (user == null)
                        {
                            ModelState.AddModelError("", "Invalid email address.");
                        }
                        else
                        {
                            ModelState.AddModelError("", $"Wrong password");
                        }
                        return View();
                    }
                }
                else
                {
                    return View();
                }
            }

            catch (Exception ex)
            {
                LogException(nameof(Login), ex, "Faild to Login ");
                throw ex;
            }
        }



        //POST

        public async Task<ActionResult> DeleteAccount(string id)
        {
            try
            {

            
            var result = await _accountService.DeleteAccountAsync(id, GetUserId());
            if (result)
            {
                return RedirectToAction(nameof(Index));
            }
            else
            {
                // Handle user not found or deletion failure
                return View("Index");
            }
}
            catch (Exception ex)
            {
                LogException(nameof(DeleteAccount), ex, $"Faild to delete User ID: {id}");
                throw ex;
            }
        }

        //GET
        public async Task<ActionResult> Logout()
        {
            try
            {

            
            await _accountService.LogoutAsync(GetUserId());
            return RedirectToAction("Login", "Account");
            }
            catch (Exception ex)
            {
                LogException(nameof(Logout), ex, "Faild to Logout");
                throw ex;
            }
        }

        public IActionResult Index()
        {
            try
            {

            
            if (User.IsInRole("Admin") ||  User.IsInRole("HR Manager") )
            {
                // User is in the Admin role
                var usersWithRoles = _accountService.GetAllUsersWithRoles();
                return View(usersWithRoles);
            }
            else
            {
                return View("AccessDenied");
            }
            }
            catch (Exception ex)
            {
                LogException(nameof(Index), ex, "Faild to Load the Index Page");
                throw ex;
            }

        }




        public async Task<IActionResult> Details(string id)
        {
            try
            {

            
            var userDetails =  _accountService.GetUsersById(id);

            return View(userDetails);
            }
            catch (Exception ex)
            {
                LogException(nameof(Details), ex, "Faild to Load the Details of the users");
                throw ex;
            }

        }



        // GET: Register/Create
        public IActionResult Create()
        {
            try
            {

            
            var roles = _roleManager.Roles.Select(r => r.Name).ToList();

            var model = new Register
            {
                SelectedRole = roles.ToString()
            };

            ViewBag.Roles = new SelectList(roles);

            return View(model);
            }
            catch (Exception ex)
            {
                LogException(nameof(Create), ex, "Faild to load the Create page");
                throw ex;
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Register collection)
        {
            try
            {

            
            if (ModelState.IsValid)
            {
                var existingUser = await _userManager.FindByEmailAsync(collection.Email);
                if (existingUser != null)
                {
                    ModelState.AddModelError(string.Empty, "Email is already in use.");
                    var roless = _roleManager.Roles.Select(r => r.Name).ToList();
                    ViewBag.Roles = new SelectList(roless);
                    return View(collection);
                }

                var user = new IdentityUser
                {
                    Email = collection.Email,
                    UserName = collection.UserName
                };

                var result = await _userManager.CreateAsync(user, collection.Password);

                if (result.Succeeded)
                {
                    if (!string.IsNullOrEmpty(collection.SelectedRole))
                    {
                        await _userManager.AddToRoleAsync(user, collection.SelectedRole);
                    }


                    //Send an Email to the user after creted it
                    await _accountService.SendRegistrationEmail(user, collection.Password);

                    // Your registration success logic here
                    return RedirectToAction("Index");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }

            var roles = _roleManager.Roles.Select(r => r.Name).ToList();
            ViewBag.Roles = new SelectList(roles);

            return View(collection);
            }
            catch (Exception ex)
            {
                LogException(nameof(Create), ex, "Faild to Create a new user");
                throw ex;
            }
        }

        public async Task<IActionResult> Edit(string id)
        {
            try
            {

            
            var user = await _userManager.FindByIdAsync(id);

            var roles = _roleManager.Roles.Select(r => r.Name).ToList();

            var userRole = await _userManager.GetRolesAsync(user);

            var model = new Register
            {
                RegisterrId=user.Id,
                Email = user.Email,
                UserName = user.UserName,
                SelectedRole = userRole.FirstOrDefault(),
            };

            ViewBag.Roles = new SelectList(roles);
            return View(model);
            }
            catch (Exception ex)
            {
                LogException(nameof(Edit), ex, $"faild to load the edit page for the users");
                throw ex;
            }
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Register collection)
        {
            try
            {

            
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByIdAsync(collection.RegisterrId);

                user.Email = collection.Email;
                user.UserName = collection.UserName;

                if (!string.IsNullOrEmpty(collection.Password))
                {

                    var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                    var passwordChangeResult = await _userManager.ResetPasswordAsync(user, token, collection.Password);

                    if (!passwordChangeResult.Succeeded)
                    {

                        foreach (var error in passwordChangeResult.Errors)
                        {
                            ModelState.AddModelError("", error.Description);
                        }
                        return View(collection);
                    }
                }

                var result = await _userManager.UpdateAsync(user);

                if (result.Succeeded)
                {
                    if (!string.IsNullOrEmpty(collection.SelectedRole))
                    {
                        var userRoles = await _userManager.GetRolesAsync(user);
                        await _userManager.RemoveFromRolesAsync(user, userRoles);
                        await _userManager.AddToRoleAsync(user, collection.SelectedRole);
                    }
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                }
            }

            var roles = _roleManager.Roles.Select(r => r.Name).ToList();
            ViewBag.Roles = new SelectList(roles);

            return View(collection);
            }
             catch (Exception ex)
            {
                LogException(nameof(Edit), ex, $"User ID: {collection.RegisterrId}");
                throw ex;
            }
        }



        public async Task<IActionResult> Delete(string id)
        {
            try
            {

            
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            var roles = _roleManager.Roles.Select(r => r.Name).ToList();

            var model = new Register
            {
                RegisterrId = user.Id,
                Email = user.Email,
                UserName = user.UserName,
                SelectedRole = (await _userManager.GetRolesAsync(user)).FirstOrDefault()
            };

            ViewBag.Roles = new SelectList(roles);

            return View(model);
            }
            catch (Exception ex)
            {
                LogException(nameof(Delete), ex, "faild to load the delete page for the users");
                throw ex;
            }
        }




        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            try
            {

            
            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            var result = await _userManager.DeleteAsync(user);

            if (result.Succeeded)
            {
                return RedirectToAction(nameof(Index));
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                return View(user);
            }
            }
            catch (Exception ex)
            {
                LogException(nameof(DeleteConfirmed), ex, $"Faild to delete User ID: {id}");
                throw ex;
            }
        }


        public IActionResult AccessDenied()
        {
            try
            {

            return View();
            }

            catch(Exception ex) {
                LogException(nameof(AccessDenied), ex, "Faild to load the AccessDenied page");
                throw ex;

            }
        }



    }
}
