using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace CMS.Application.DTOs
{
    public class CandidateDTO
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "FullName is required.")]
        public string FullName { get; set; }
        [Required(ErrorMessage = "Phone is required.")]
        public int Phone { get; set; }
        [Required(ErrorMessage = "DesiredPosition is required.")]
        public int PositionId { get; set; }

        public string Name { get; set; }


        [Required(ErrorMessage = "company is required.")]
        public int CompanyId { set; get; }
        public string CompanyName { set; get; }
        [Required(ErrorMessage = "Email is required.")]


        public string Email { get; set; }
        [Required(ErrorMessage = "Address is required.")]
        public string Address { get; set; }

        [Required(ErrorMessage = "Experience is required.")]
        [RegularExpression(@"^[0-9]+$", ErrorMessage = "Experience must contain only numeric values.")]
        public string Experience { get; set; }

        [Required(ErrorMessage = "CVAttachment is required.")]
        public int CVAttachmentId { get; set; }
        [Required]
        [Url(ErrorMessage = "Invalid LinkedIn URL.")]
        public string LinkedInUrl { get; set; }
        public int CountryId { get; set; }
        public string CountryName { get; set; }
        public List<InterviewsDTO> InterviewsDTO { get; set; }



    }
}
