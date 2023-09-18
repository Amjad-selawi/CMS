using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace CMS.Domain.Entities
{
    public class Status
    {
        public int Id { set; get; }
        [Required]
        public string Name { set; get; }

        [Required]
        public string Code { set; get; }

        public virtual ICollection<Interviews> Interviews { get; set; }

    }
}
