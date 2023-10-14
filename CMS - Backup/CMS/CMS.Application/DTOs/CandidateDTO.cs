using CMS.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace CMS.Application.DTOs
{
    public class CandidateDTO
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Full Name is required.")]
        public string FullName { get; set; }
        [Required(ErrorMessage = "Phone is required.")]
        public int Phone { get; set; }
        public int? PositionId { get; set; }
        public int? PositionName { get; set; }

        public string Name { get; set; }

        public InterviewsDTO LastInterview { get; set; }

        [Required(ErrorMessage = "company is required.")]
        public int CompanyId { set; get; }
        public string CompanyName { set; get; }

        [Required(ErrorMessage = "Experience is required.")]
        [RegularExpression(@"^[0-9]+$", ErrorMessage = "Experience must contain only numeric values.")]
        public string Experience { get; set; }

        public int? CVAttachmentId { get; set; }
       
        [Url(ErrorMessage = "Invalid LinkedIn URL.")]
        public string LinkedInUrl { get; set; }
        public int? CountryId { get; set; }
        public string CountryName { get; set; }
        public string Status { get; set; }
        public double? Score { get; set; }
        public List<InterviewsDTO> InterviewsDTO { get; set; }



    }
}
