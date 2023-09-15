using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Text;

namespace CMS.Application.DTOs
{
    public class CandidateCreateDTO
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "FullName is required.")]
        public string FullName { get; set; }
        [Required(ErrorMessage = "Phone is required.")]
        public int Phone { get; set; }
        [Required(ErrorMessage = "DesiredPosition is required.")]
        public int PositionId { get; set; }
        [Required(ErrorMessage = "Email is required.")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Address is required.")]
        public string Address { get; set; }
        [Required(ErrorMessage = "Experience is required.")]
        public string Experience { get; set; }
        public string LinkedInUrl { get; set; }
        public int CVAttachmentId { get; set; }
        public string FileName { get; set; }
        public long FileSize { get; set; }
        public Stream FileData { get; set; }
        public DateTime CreatedOn { get; set; }
        public int CountryId { get; set; }
    }
}
