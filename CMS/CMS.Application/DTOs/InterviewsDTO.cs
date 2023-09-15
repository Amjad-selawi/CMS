﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace CMS.Application.DTOs
{
    public class InterviewsDTO
    {
        
        public int InterviewsId { get; set; }
        public int? Score { get; set; }
        public DateTime Date { get; set; }
        public int? ParentId { get; set; }
        public int? Status { get; set; }
        
        public string InterviewerId { get; set; }
        public int PositionId { get; set; }
        public int CandidateId { get; set; }
        #nullable enable
        public string? Notes { get; set; }
    }
}
