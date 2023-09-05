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

        public int Score { get; set; }
       
        public DateTime Date { get; set; }

        public InterviewStatus Status { get; set; }
 
        public int CandidateId { get; set; }
        //public Candidates Candidate { get; set; }


        public int ParentId { get; set; }

        public string Notes { get; set; }


    }
}
