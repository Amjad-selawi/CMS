using Microsoft.AspNetCore.Mvc;

namespace CMS.Web.Controllers
{
    public class DashboardController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
