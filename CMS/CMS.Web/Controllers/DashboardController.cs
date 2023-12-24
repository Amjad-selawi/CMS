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
using System.Net.Mail;
using System.Net;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using CMS.Repository.Interfaces;
using CMS.Domain.Enums;

namespace CMS.Web.Controllers
{
    public class DashboardController : Controller
    {
        private readonly IReportingService _reportingService;
        private readonly ApplicationDbContext _context;
        private ICountryService _countryService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IStatusRepository _statusRepository;

        public DashboardController(IReportingService reportingService, 
            ApplicationDbContext context, ICountryService countryService,IHttpContextAccessor httpContextAccessor,
            IStatusRepository statusRepository)
        {
            _reportingService = reportingService;
            _context = context;
            _countryService = countryService;
            _httpContextAccessor = httpContextAccessor;
            _statusRepository = statusRepository;
        }

          public void LogException(string methodName, Exception ex, string additionalInfo = null)
        {
            
            _countryService.LogException(methodName, ex, additionalInfo);
        }
    
        public async Task<IActionResult> Index()
        {
            try
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


                    double onHoldPercentageFloat = ((double)report.NumberOfOnHold / report.NumberOfCandidates) * 100;
                    int onHoldPercentage = (int)onHoldPercentageFloat;
                    ViewBag.OnHoldPercentage = onHoldPercentage;


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
            catch (Exception ex)
            {
                LogException(nameof(Index), ex, "Index page for Dashboard not working");
                throw ex;
            }
        }
        private static string ArrayToString(string[] array)
        {
            try
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
            catch (Exception ex)
            {
                throw ex;
            }
        }



        public IActionResult IndexForTree()
        {
            try
            {

            
            var treeData = GetDataFromDatabase(); // Retrieve data from the database

            return View(treeData);
            }
            catch(Exception ex)
            {
                LogException(nameof(IndexForTree), ex, "IndexForTree not working");
                throw ex;
            }
        }



        private List<PerformanceReportDTO> GetDataFromDatabase()
        {
            try
            {
                var candidateData = (from candidate in _context.Candidates
                                     join interview in _context.Interviews on candidate.Id equals interview.CandidateId
                                     orderby interview.ModifiedOn descending
                                     select new
                                     {
                                         PositionId = interview.Position.Id,
                                         PositionName = interview.Position.Name,
                                         CountryId = candidate.Company.Country.Id,
                                         CountryName = candidate.Company.Country.Name,
                                         CandidateName = candidate.FullName,
                                         StatusName = interview.Status.Name,
                                         Score = interview.Score,
                                         InterviewDate = interview.Date,
                                         ModifiedOn = interview.ModifiedOn,
                                         InterviewerName = interview.Interviewer.UserName // Include InterviewerName
                                     }).ToList();


                var latestInterviews = candidateData
                    .GroupBy(x => new
                    {
                        x.PositionId,
                        x.PositionName,
                        x.CountryId,
                        x.CountryName,
                        x.CandidateName
                    })
                    .Select(group => group.FirstOrDefault()) // Select the first interview for each candidate
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
                            InterviewerName = c.InterviewerName,
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
            catch (Exception ex)
            {
                LogException(nameof(GetDataFromDatabase), ex, "GetDataFromDatabase not working");
                throw ex;
            }
        }

        public IActionResult AccessDenied()
        {
            try
            {

            return View();
            }
            catch (Exception ex)
            {
                LogException(nameof(AccessDenied), ex, "Faild to load AccessDenied page");
                throw ex;
            }
        }



        //public async Task<IActionResult> AcceptedCandidates()
        //{
        //    var acceptedCandidates = await _statusRepository.GetCandidatesByCode(CMS.Domain.Enums.StatusCode.Approved);

        //    return View(acceptedCandidates);
        //}


    }
}