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

        public ReportingService(ICarrerOfferRepository carrerOfferRepository, ICandidateRepository candidateRepository)
        {
            _carrerOfferRepository = carrerOfferRepository;
            _candidateRepository = candidateRepository;
        }
        public async Task<Result<PerformanceReportDTO>> GetBusinessPerformanceReport()
        {
            int candidatesCount = await _candidateRepository.CountAllAsync();
            int acceptedCount = await _candidateRepository.CountAcceptedAsync();
            Dictionary<string,int> candidatesPerCountry = await _candidateRepository.CountCandidatesPerCountry();
            int carreroffersCount = await _carrerOfferRepository.CountAllAsync();
            PerformanceReportDTO report = new PerformanceReportDTO
            {
                CarrerOffers = carreroffersCount,
                CandidatesPerCountry = candidatesPerCountry,
                NumberOfAccepted = acceptedCount,
                NumberOfCandidates = candidatesCount,
            };
            return Result<PerformanceReportDTO>.Success(report);
        }
    }
}
