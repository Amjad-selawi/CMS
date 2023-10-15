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
using System.Text.Json;
using CMS.Domain.Entities;
using CMS.Services.Services;

namespace CMS.Web.Controllers
{
    public class DashboardController : Controller
    {
        private readonly IReportingService _reportingService;
        private readonly ApplicationDbContext _context;
        private ICountryService _countryService;

        public DashboardController(IReportingService reportingService, ApplicationDbContext context, ICountryService countryService)
        {
            _reportingService = reportingService;
            _context = context;
            _countryService = countryService;
        }
        public async Task<IActionResult> Index()
        {
            if (User.IsInRole("Admin") || User.IsInRole("General Manager") || User.IsInRole("HR Manager"))
            {
                var report = (await _reportingService.GetBusinessPerformanceReport()).Value;
                double percentageFloat = ((double)report.NumberOfAccepted / report.NumberOfCandidates) * 100;
                int acceptedPercentage = (int)percentageFloat;
                ViewBag.AcceptedPercentage = acceptedPercentage;
                double rejectedFloat = ((double)report.NumberOfRejected / report.NumberOfCandidates) * 100;
                int rejectedPercentage = (int)rejectedFloat;
                ViewBag.RejectedPercentage = rejectedPercentage;
                //ViewBag.CountriesList = ArrayToString(report.CandidatesPerCountry.Keys.ToArray());
                //ViewBag.CandidatesPerCompanyList = DictionaryToString(report.candidatesPerCompany);
                ViewBag.PendingCount = report.NumberOfPending; // Add the pending count to the ViewBag
                                                               //var countries = await _countryService.GetAllCountriesAsync();

                var countries = await _countryService.GetAllCountriesAsync(); // Assuming you have a countryService instance

                // Convert the list of countries to a JSON array for use in JavaScript
                var countriesJson = JsonSerializer.Serialize(countries.Select(c => c.Name).ToList());

                ViewBag.CountriesList = countriesJson; // Pass the JSON data to the view

                var treeData = GetDataFromDatabase();

                ViewBag.TreeData = treeData;



                return View(report);
            }
            else
            {
                // User is not in the Admin role, handle accordingly (redirect or show an error message)
                return View("AccessDenied");
            }
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
            //    var candidateData = (from candidate in _context.Candidates
            //                         join interview in _context.Interviews on candidate.Id equals interview.CandidateId
            //                         select new
            //                         {
            //                             PositionId = interview.Position.Id,
            //                             PositionName = interview.Position.Name,
            //                             CountryId = candidate.Company.Country.Id,
            //                             CountryName = candidate.Company.Country.Name,
            //                             CandidateName = candidate.FullName,
            //                             StatusName = interview.Status.Name,
            //                             Score = interview.Score
            //                         }).Distinct().ToList();


            //    var positionsGroups = candidateData.GroupBy(x => new
            //    {
            //        x.PositionId,
            //        x.PositionName,
            //        x.CountryId,
            //        x.CountryName

            //    }).Select(group => new PositionDTO
            //    {
            //        Id = group.Key.PositionId,
            //        Name = group.Key.PositionName,
            //        CountryId = group.Key.CountryId,
            //        CountryName = group.Key.CountryName,

            //        Candidates = group.Select(c => new CandidateDTO
            //        {
            //            Name = c.CandidateName,
            //            Status = c.StatusName,
            //            Score = c.Score
            //        }).ToList()

            //    }).ToList();

            //    var result = positionsGroups.GroupBy(g => new
            //    {
            //        g.CountryId,
            //        g.CountryName

            //    }).Select(g => new PerformanceReportDTO()
            //    {
            //        Name = g.Key.CountryName,
            //        Positions = g.ToList()

            //    }).ToList();

            //    return result;
            //}

            var candidateData = (from candidate in _context.Candidates
                                 join interview in _context.Interviews on candidate.Id equals interview.CandidateId
                                 select new
                                 {
                                     PositionId = interview.Position.Id,
                                     PositionName = interview.Position.Name,
                                     CountryId = candidate.Company.Country.Id,
                                     CountryName = candidate.Company.Country.Name,
                                     CandidateName = candidate.FullName,
                                     StatusName = interview.Status.Name,
                                     Score = interview.Score,
                                     InterviewDate = interview.Date  // Assuming you have an InterviewDate property
                                 }).ToList();  // Remove Distinct to keep all interviews

            var latestInterviews = candidateData
                .GroupBy(x => new
                {
                    x.PositionId,
                    x.PositionName,
                    x.CountryId,
                    x.CountryName,
                    x.CandidateName
                })
                .Select(group => group
                    .OrderByDescending(c => c.InterviewDate)
                    .FirstOrDefault())  // Select the most recent interview for each candidate
                .ToList();

            var positionsGroups = latestInterviews
                .GroupBy(x => new
                {
                    x.PositionId,
                    x.PositionName,
                    x.CountryId,
                    x.CountryName
                })
                .Select(group => new PositionDTO
                {
                    Id = group.Key.PositionId,
                    Name = group.Key.PositionName,
                    CountryId = group.Key.CountryId,
                    CountryName = group.Key.CountryName,
                    Candidates = group.Select(c => new CandidateDTO
                    {
                        Name = c.CandidateName,
                        Status = c.StatusName,
                        Score = c.Score,
                    }).ToList()
                })
                .ToList();

            var result = positionsGroups.GroupBy(g => new
            {
                g.CountryId,
                g.CountryName
            })
            .Select(g => new PerformanceReportDTO()
            {
                Name = g.Key.CountryName,
                Positions = g.ToList()
            })
            .ToList();

            return result;
        }

        public IActionResult AccessDenied()
        {
            return View();
        }



    }
}