using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace CMS.Domain.Entities
{
    public class Candidate
    {

        public int Id { get; set; }

        [Required(ErrorMessage = "FullName is required.")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Phone is required.")]
        public int Phone { get; set; }

        [Required(ErrorMessage = "DesiredPosition is required.")]
        public string DesiredPosition { get; set; }
        
        //public virtual Position Position { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Address is required.")]
        public string Address { get; set; }

        [Required(ErrorMessage = "Experience is required.")]
        public string Experience { get; set; }

        [Required(ErrorMessage = "CVAttachmentId is required.")]
        public int CVAttachmentId { get; set; }
        public virtual Attachment CV { get; set; }
        public string LinkedInUrl { get; set; }
        public virtual ICollection<Interviews> Interviews { get; set; }
    }
}
