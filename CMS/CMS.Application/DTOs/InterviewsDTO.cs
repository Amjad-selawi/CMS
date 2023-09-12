using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace CMS.Application.DTOs
{
    public class InterviewsDTO
    {
        
        public int InterviewsId { get; set; }

        public int? Score { get; set; }
        [Required]
        public DateTime Date { get; set; }

        public int? ParentId { get; set; }
        [Required]
        public int StatusId { set; get; }
        public string StatusName { get; set; }

        public string Notes { get; set; }

        [Required]
        public int InterviewerId { get; set; }
        [Required]
        public int candidateId { get; set; }
        public string FullName{ get; set; }

        [Required]
        public int positionId { get; set; }
        public string Name { get; set; }
    }
}
