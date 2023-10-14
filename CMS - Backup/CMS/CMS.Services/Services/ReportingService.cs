using CMS.Application.DTOs;
using CMS.Application.Extensions;
using CMS.Repository.Interfaces;
using CMS.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CMS.Services.Services
{
    public class ReportingService : IReportingService
    {
        ICarrerOfferRepository _carrerOfferRepository;
        ICandidateRepository _candidateRepository;
        private ICountryService _countryService;


        public ReportingService(ICarrerOfferRepository carrerOfferRepository, ICandidateRepository candidateRepository, ICountryService countryService)
        {
            _carrerOfferRepository = carrerOfferRepository;
            _candidateRepository = candidateRepository;
            _countryService = countryService;
        }
        public async Task<Result<PerformanceReportDTO>> GetBusinessPerformanceReport()
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
    }
}
