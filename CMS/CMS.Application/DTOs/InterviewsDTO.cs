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

        public int? ParentId { get; set; }
       // [Required]
        public int? StatusId { set; get; }
        public string StatusName { get; set; }

        public string Notes { get; set; }

        [Required]
        public string InterviewerId { get; set; }
        public string InterviewerName { get; set;}
        [Required]
        public int candidateId { get; set; }
        public string FullName{ get; set; }

        [Required]
        public int positionId { get; set; }
        public string Name { get; set; }

        public int? AttachmentId { set; get; }

        public string FileName { get; set; }
        public long? FileSize { get; set; }
        public Stream FileData { get; set; }

        public  bool isUpdated { set; get; } = false;

    }
}
