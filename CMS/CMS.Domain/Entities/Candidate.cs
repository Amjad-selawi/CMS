using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace CMS.Domain.Entities
{
    public class Candidate : BaseEntity
    {

        public int CandidateId { get; set; }

        public string FullName { get; set; }

        public int Phone { get; set; }

        [Required(ErrorMessage = "DesiredPosition is required.")]
        public int PositionId { get; set; }
        public virtual Position Position { get; set; }

        public string Email { get; set; }

        public string Address { get; set; }

        public string Experience { get; set; }

        public int CVAttachmentId { get; set; } 
        public string LinkedInUrl { get; set; }
        public virtual Attachment CV { get; set; }

        public virtual ICollection<Interviews> Interviews { get; set; }
    }
}
