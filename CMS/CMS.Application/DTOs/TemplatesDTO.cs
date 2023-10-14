using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace CMS.Application.DTOs
{
    public class TemplatesDTO
    {
        public int TemplatesId { get; set; }

        public string Name { get; set; }
        public string Title { get; set; }

        public string BodyDesc { get; set; }
    }
}
