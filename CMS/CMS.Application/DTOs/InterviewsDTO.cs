using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace CMS.Application.DTOs
{
    public class InterviewsDTO
    {
        
        public int InterviewsId { get; set; }

        public int Source { get; set; }
        
        public DateTime Date { get; set; }

        public int ParentId { get; set; }

        public string Status { get; set; }

        public string Notes { get; set; }

        public int InterviewerId { get; set; }

        public CandidateDTO candidateDTO { get; set; }
        public string FullName{ get; set; }

        public PositionDTO positionDTO { get; set; }
        public string Name { get; set; }
    }
}
