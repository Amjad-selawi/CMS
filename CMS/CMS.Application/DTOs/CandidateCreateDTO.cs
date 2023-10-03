using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Text;

namespace CMS.Application.DTOs
{
    public class CandidateCreateDTO
    {
        public int CandidateId { get; set; }
        [Required(ErrorMessage = "FullName is required.")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Phone is required.")]
        //[RegularExpression(@"\d.*", ErrorMessage = "Phone must contain at least one number.")]

        public int Phone { get; set; }

        [Required(ErrorMessage = "DesiredPosition is required.")]
        public int PositionId { get; set; }

        [Required(ErrorMessage = "company is required.")]
        public int CompanyId { set; get; }

        public string CompanyName { set; get; }

        [Required(ErrorMessage = "Email is required.")]
        [DataType(DataType.EmailAddress, ErrorMessage = "E-mail is not valid")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Address is required.")]
        public string Address { get; set; }

        [Required(ErrorMessage = "Experience is required.")]
        [RegularExpression(@"^[0-9]+$", ErrorMessage = "Experience must contain only numeric values.")]
        public string Experience { get; set; }

        [Url(ErrorMessage = "Invalid LinkedIn URL.")]
      
        public string LinkedInUrl { get; set; }
        public int CountryId { get; set; }
        public string CountryName { get; set; }
        public int? CVAttachmentId { get; set; }
        public string FileName { get; set; }
        public long FileSize { get; set; }
        //[Required(ErrorMessage = "CVAttachment is required.")]
        public Stream FileData { get; set; }
        public DateTime CreatedOn { get; set; }
        public List<InterviewsDTO> InterviewsDTO { get; set; }
    }
}
