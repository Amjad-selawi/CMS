using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace CMS.Domain.Entities
{
    public class Position
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public virtual ICollection<Interviews> Interviews { get; set; }
        public virtual ICollection<Candidate> Candidates { get; set; }
    }
}
