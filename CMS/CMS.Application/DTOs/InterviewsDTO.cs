using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Text;

namespace CMS.Application.DTOs
{
    public class InterviewsDTO
    {
        
        public int InterviewsId { get; set; }

        public int? Score { get; set; }
        [Required(ErrorMessage = "Please select a Date.")]
        public DateTime Date { get; set; }
        public int? ParentId { get; set; }
        [Required(ErrorMessage = "Please select a Position.")]
        public int PositionId { get; set; }
        public string PositionName { get; set; }
        [Required(ErrorMessage = "Please select a Candidate.")]
        public int CandidateId { get; set; }
        public string CandidateName { get; set; }
        public int? StatusId { get; set; }
        public string StatusName { get; set; }
        [Required(ErrorMessage = "Please select a Interviewer.")]
        public string InterviewerId { get; set; }
        public string InterviewerName { get; set; }
        public string FullName{ get; set; }
        public string Name { get; set; }
        public int? AttachmentId { set; get; }
        public string FileName { get; set; }
        public long? FileSize { get; set; }
        public Stream FileData { get; set; }
        public bool isUpdated { set; get; } = false;
        #nullable enable
        public string? Notes { get; set; }
    }
}
