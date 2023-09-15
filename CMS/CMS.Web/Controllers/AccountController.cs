using CMS.Application.DTOs;
using CMS.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Threading.Tasks;

namespace CMS.Web.Controllers
{
    public class AccountController : Controller
    {
        SignInManager<IdentityUser> _signInManager;
        private readonly IAccountService _accountService;

        public AccountController(IAccountService accountService, SignInManager<IdentityUser> signInManager)
        {
            _accountService = accountService;
            _signInManager = signInManager;
        }

        //GET
        public async Task<ActionResult> ListUsers()
        {
            var listuser = await _accountService.GetAllUsersAsync();
            return View(listuser);
        }



        //GET
        public async Task<ActionResult> Login()
        {
            if (_signInManager.IsSignedIn(User))
            {
                return RedirectToAction("Index", "Home");
                
            }
            else
            {
                return View();
            }
        }

        //POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(Login collection)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    ModelState.AddModelError("", "Email or Password not correct..!");
                    return View();
                }

                var result = await _accountService.LoginAsync(collection);
                if (result)
                {
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError("", "Email or Password not correct..!");
                    return View();
                }
            }

            catch
            {
                return View();
            }
        }



        //POST

        public async Task<ActionResult> DeleteAccount(string id)
        {

            var result = await _accountService.DeleteAccountAsync(id);
            if (result)
            {
                return RedirectToAction(nameof(ListUsers));
            }
            else
            {
                // Handle user not found or deletion failure
                return View("ListUsers");
            }

        }

        //GET
        public async Task<ActionResult> Logout()
        {
            await _accountService.LogoutAsync();
            return RedirectToAction("Login", "Account");
        }

        public async Task<IActionResult> Register()
        {
            return View();
        }
    }
}
