using CMS.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace CMS.Domain.Entities
{
    public class Templates : BaseEntity
    {
       
        public int TemplatesId { get; set; }

        public TemplatesName Name { get; set; }

        public string Title { get; set; }

        public string BodyDesc { get; set; }

        public virtual ICollection<Notifications> Notifications { get; set; }
    }
}
