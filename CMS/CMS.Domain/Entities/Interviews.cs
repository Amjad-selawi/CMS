using CMS.Domain.Enums;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace CMS.Domain.Entities
{
    public class Interviews : BaseEntity
    {

        public int InterviewsId { get; set; }
        public int? Score { get; set; }
        public DateTime Date { get; set; }

        public InterviewStatus? Status { get; set; }

        public int CandidateId { get; set; }
        public virtual Candidate Candidate { get; set; }

        public int PositionId { get; set; }
        public virtual Position Position { get; set; }

        public string InterviewerId { get; set; }
        public virtual IdentityUser Interviewer { get; set; }

        public int? ParentId { get; set; }
        #nullable enable
        public string? Notes { get; set; }

    }
}
