using CMS.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient.Server;
using System.Text;
using System;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using System.Linq;

namespace CMS.Web.Controllers
{
    public class DashboardController : Controller
    {
        private readonly IReportingService _reportingService;
        public DashboardController(IReportingService reportingService)
        {
            _reportingService = reportingService;

        }
        public async Task<IActionResult> Index()
        {
            var report = (await _reportingService.GetBusinessPerformanceReport()).Value;
            double percentageFloat = ((double)report.NumberOfAccepted / report.NumberOfCandidates) * 100;
            int acceptedPercentage = (int) percentageFloat;
            ViewBag.Percentage = acceptedPercentage;
            ViewBag.CountriesList = ArrayToString(report.CandidatesPerCountry.Keys.ToArray());
            return View(report);
        }
        private static string ArrayToString(string[] array)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("[");
            for (int i = 0; i < array.Length; i++)
            {
                sb.Append("'");
                sb.Append(array[i]);
                sb.Append("'");

                if (i < array.Length - 1)
                {
                    sb.Append(",");
                }
            }
            sb.Append("]");
            return sb.ToString();
        }
  
    
    }
}
