using System;
using System.Collections.Generic;
using System.Text;

namespace CMS.Application.DTOs
{
    public class PerformanceReportDTO
    {//Updated class
        public int NumberOfCandidates { get; set; }
        public int NumberOfAccepted { get; set; }
        public int NumberOfRejected { get; set; }
        public int NumberOfPending { get; set; }
        public Dictionary<string, int> CandidatesPerCountry { get; set; }
        public Dictionary<string, int> candidatesPerCompany { get; set; }


        //Tree
        public int Id { get; set; }
        public string Name { get; set; }
        public List<PositionDTO> Positions { get; set; }
    }
}
