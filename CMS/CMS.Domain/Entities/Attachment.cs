using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace CMS.Domain.Entities
{
    public class Attachment
    {
        public int Id { get; set; }

        [Required]
        public string FileName { get; set; }

        public long FileSize { get; set; }

        public byte[] FileData { get; set; }

        public DateTime CreatedOn { get; set; }
    }
}
