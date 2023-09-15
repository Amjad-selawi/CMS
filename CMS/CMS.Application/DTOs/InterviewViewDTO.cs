using System;
using System.Collections.Generic;
using System.Text;

namespace CMS.Application.DTOs
{
    public class InterviewViewDTO
    {

        public int InterviewsId { get; set; }
        public int? Score { get; set; }
        public DateTime Date { get; set; }
        public int? Status { get; set; }
        public string? Notes { get; set; }
        public string InterviewerName { get; set; }
        public string PositionName { get; set; }
        public string CandidateName { get; set; }
    }
}
