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
        [Required]
        public DateTime Date { get; set; }
        public string StatusName { get; set; }
        public int? ParentId { get; set; }
        public int PositionId { get; set; }
        public string InterviewerName { get; set; }
        public int CandidateId { get; set; }
        public int? StatusId { get; set; }
        public string StatusName { get; set; }
        public string Notes { get; set; }

        public int InterviewerId { get; set; }

        public CandidateDTO candidateDTO { get; set; }
        public string FullName{ get; set; }

        public PositionDTO positionDTO { get; set; }
        public string Name { get; set; }
        #nullable enable
        public string? Notes { get; set; }
        public int? AttachmentId { set; get; }

        public string FileName { get; set; }
        public long? FileSize { get; set; }
        public Stream FileData { get; set; }

        public bool isUpdated { set; get; } = false;
    }
}
