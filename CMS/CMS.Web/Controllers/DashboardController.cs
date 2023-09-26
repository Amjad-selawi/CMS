using CMS.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient.Server;
using System.Text;
using System;
using System.Threading.Tasks;
using System.Linq;
using CMS.Application.DTOs;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using CMS.Domain;

namespace CMS.Web.Controllers
{
    public class DashboardController : Controller
    {
        private readonly IReportingService _reportingService;
        private readonly ApplicationDbContext _context;

        public DashboardController(IReportingService reportingService, ApplicationDbContext context)
        {
            _reportingService = reportingService;
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            var report = (await _reportingService.GetBusinessPerformanceReport()).Value;
            double percentageFloat = ((double)report.NumberOfAccepted / report.NumberOfCandidates) * 100;
            int acceptedPercentage = (int) percentageFloat;
            ViewBag.AcceptedPercentage = acceptedPercentage;
            double rejectedFloat = ((double)report.NumberOfRejected / report.NumberOfCandidates) * 100;
            int rejectedPercentage = (int)rejectedFloat;
            ViewBag.RejectedPercentage = rejectedPercentage;
            ViewBag.CountriesList = ArrayToString(report.CandidatesPerCountry.Keys.ToArray());



            var treeData = GetDataFromDatabase();

            ViewBag.TreeData = treeData;



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




        public IActionResult IndexForTree()
        {
            var treeData = GetDataFromDatabase(); // Retrieve data from the database

            return View(treeData);
        }



        private List<PerformanceReportDTO> GetDataFromDatabase()
        {
            var countries = _context.Countries
                .Include(c => c.Candidates)
                .ThenInclude(candidate => candidate.Position)
                .ToList();

            var result = new List<PerformanceReportDTO>();

            foreach (var country in countries)
            {
                var groupedCandidates = country.Candidates
                    .GroupBy(candidate => candidate.Position)
                    .Select(group => new PositionDTO
                    {
                        Id = group.Key.Id,
                        Name = group.Key.Name,
                        Candidates = group.Select(c => new CandidateDTO()
                        {
                            FullName = c.FullName,
                        }).ToList()
                    })
                    .ToList();

                var countryDto = new PerformanceReportDTO()
                {
                    Id = country.Id,
                    Name = country.Name,
                    Positions = groupedCandidates.Select(group => group).ToList()
                };

                result.Add(countryDto);
            }

            return result;
        }



    }
}
