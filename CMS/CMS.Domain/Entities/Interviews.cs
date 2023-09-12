using CMS.Domain.Enums;
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
        [Required]
        public DateTime Date { get; set; }

        //public InterviewStatus Status { get; set; }
        [Required]
        public int StatusId { set; get; }
        public virtual Status Status { get; set; }

        [Required]
        public int CandidateId { get; set; }
        public virtual Candidate Candidate { get; set; }

        [Required]
        public int PositionId { get; set; }
        public virtual Position Position { get; set; }

        [Required]
        public int InterviewerId { get; set; }
        //public Interviewers Interviewer { get; set; }


        public int? ParentId { get; set; }

        public string Notes { get; set; }


    }
}
