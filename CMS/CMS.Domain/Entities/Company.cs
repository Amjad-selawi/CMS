using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace CMS.Domain.Entities
{
    public class Company:BaseEntity
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Email { get;set; }
        public string Address { get; set; } 
        public string PhoneNumber { set; get; }
        [Required]
        public int CountryId { get; set; }
        public virtual Country Country { get; set; }

        public virtual ICollection<Candidate> Candidates { get; set; }
    }
}
