using CMS.Application.DTOs;
using CMS.Application.Extensions;
using CMS.Domain;
using CMS.Domain.Entities;
using CMS.Repository.Interfaces;
using CMS.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CMS.Services.Services
{
    public class ReportingService : IReportingService
    {
        private readonly ApplicationDbContext _dbContext;
        ICarrerOfferRepository _carrerOfferRepository;
        ICandidateRepository _candidateRepository;
        private ICountryService _countryService;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ReportingService(ApplicationDbContext dbContext,
            ICarrerOfferRepository carrerOfferRepository,
            ICandidateRepository candidateRepository, ICountryService countryService
            , UserManager<IdentityUser> userManager, IHttpContextAccessor httpContextAccessor)
        {
            _dbContext = dbContext;
            _carrerOfferRepository = carrerOfferRepository;
            _candidateRepository = candidateRepository;
            _countryService = countryService;
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
        }
        public async void LogException(string methodName, Exception ex)
        {
            var currentUser = await _userManager.GetUserAsync(_httpContextAccessor.HttpContext.User);
            var userId = currentUser?.Id;
            _dbContext.Logs.Add(new Log
            {
                MethodName = methodName,
                ExceptionMessage = ex.Message,
                StackTrace = ex.StackTrace,CreatedByUserId = userId,
                LogTime = DateTime.Now
            });
            _dbContext.SaveChanges();
        }

        public async Task<Result<PerformanceReportDTO>> GetBusinessPerformanceReport()
        {
            try
            {

            
            int candidatesCount = await _candidateRepository.CountAllAsync();
            int acceptedCount = await _candidateRepository.CountAcceptedAsync();
            //Dictionary<string,int> candidatesPerCountry = await _candidateRepository.CountCandidatesPerCountry();
            int rejectedCount = await _candidateRepository.CountRejectedAsync();
            int pendingCount = await _candidateRepository.CountPendingAsync(); // New line to count pending candidates
            //Dictionary<string, int> candidatesPerCompany = await _candidateRepository.CountCandidatesPerCompanyAsync();

            PerformanceReportDTO report = new PerformanceReportDTO
            {
                //CandidatesPerCountry = candidatesPerCountry,
                NumberOfAccepted = acceptedCount,
                NumberOfCandidates = candidatesCount,
                NumberOfRejected = rejectedCount,
                NumberOfPending = pendingCount, // Add the pending count to the report
            };
            return Result<PerformanceReportDTO>.Success(report);
            }
            catch (Exception ex)
            {
                LogException(nameof(GetBusinessPerformanceReport),ex);
                throw ex;
            }
        }
    }
}