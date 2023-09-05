using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace CMS.Domain.Entities
{
    public class BaseEntity
    {
        [StringLength(20)]
        public string CreatId { get; set; }
        public DateTime CreatDate { get; set; }

        [StringLength(20)]
        public string EditId { get; set; }
        public DateTime EditDate { get; set; }

        public bool IsActive { get; set; }
        public bool IsDelete { get; set; }
    }
}
