using CMS.Application.DTOs;
using CMS.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;

namespace CMS.Web.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        SignInManager<IdentityUser> _signInManager;
        
        public HomeController(SignInManager<IdentityUser> _signInManager)
        {
            this._signInManager = _signInManager;
            
        }

  
        public IActionResult Index()
        {
            if(_signInManager.IsSignedIn(User))
            {
                return View();
            }else
            {
                return RedirectToAction("Login", "Account");
            }
            
        }
        public IActionResult Privacy()
        {
            return View();
        }

    }

   

   


}



