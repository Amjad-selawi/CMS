using CMS.Application.DTOs;
using CMS.Domain;
using CMS.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ApplicationDbContext Db;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AccountController(IAccountService accountService, UserManager<IdentityUser> userManager, ApplicationDbContext _db, RoleManager<IdentityRole> roleManager, SignInManager<IdentityUser> signInManager)
        {
            _accountService = accountService;
            _userManager = userManager;
            Db = _db;
            _roleManager = roleManager;
            _signInManager = signInManager;
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
                    ModelState.AddModelError("", $"error logging in..!");
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
                return RedirectToAction(nameof(Index));
            }
            else
            {
                // Handle user not found or deletion failure
                return View("Index");
            }

        }

        //GET
        public async Task<ActionResult> Logout()
        {
            await _accountService.LogoutAsync();
            return RedirectToAction("Login", "Account");
        }

        // GET: Register
        public IActionResult Index()
        {
            var usersWithRoles = _accountService.GetAllUsersWithRoles();
            return View(usersWithRoles);

        }



        public async Task<IActionResult> Details(string id)
        {
            var userDetails =  _accountService.GetUsersById(id);

            return View(userDetails);
        }



        // GET: Register/Create
        public IActionResult Create()
        {
            var roles = _roleManager.Roles.Select(r => r.Name).ToList();

            var model = new Register
            {
                SelectedRole = roles.ToString()
            };

            ViewBag.Roles = new SelectList(roles);

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Register collection)
        {
            if (ModelState.IsValid)
            {
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

        public async Task<IActionResult> Edit(string id)
        {

            var user = await _userManager.FindByIdAsync(id);

            var roles = _roleManager.Roles.Select(r => r.Name).ToList();

            var userRole = await _userManager.GetRolesAsync(user);

            var model = new Register
            {
                Email = user.Email,
                UserName = user.UserName,
                SelectedRole = userRole.FirstOrDefault(),
            };

            ViewBag.Roles = new SelectList(roles);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Register collection, string id)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByIdAsync(id);

                user.Email = collection.Email;
                user.UserName = collection.UserName;


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



        public async Task<IActionResult> Delete(string id)
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




        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
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



















    }
}
