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
        [Required(ErrorMessage = "Full Name is required.")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Phone is required.")]

        public int Phone { get; set; }

        public int PositionId { get; set; }

        [Required(ErrorMessage = "company is required.")]
        public int CompanyId { set; get; }

        public string CompanyName { set; get; }

        [Required(ErrorMessage = "Experience is required.")]
        [RegularExpression(@"^[0-9]+$", ErrorMessage = "Experience must contain only numeric values.")]
        public string Experience { get; set; }

        public int CountryId { get; set; }
        public string CountryName { get; set; }
        public int? CVAttachmentId { get; set; }
        public string FileName { get; set; }
        public long FileSize { get; set; }
        public Stream FileData { get; set; }
        public DateTime CreatedOn { get; set; }
        public List<InterviewsDTO> InterviewsDTO { get; set; }
    }
}
