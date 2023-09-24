using System;
using System.Collections.Generic;
using System.Text;

namespace CMS.Application.DTOs
{
    public class PerformanceReportDTO
    {
        public int NumberOfCandidates { get; set; }
        public int NumberOfAccepted { get; set; }
        public int NumberOfRejected { get; set; }
        public Dictionary<string, int> CandidatesPerCountry { get; set; }
    }
}
