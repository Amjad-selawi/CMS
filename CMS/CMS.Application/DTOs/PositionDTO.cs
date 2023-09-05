using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace CMS.Application.DTOs
{
    public class PositionDTO
    {
        public int Id { set; get; }
        [Required]
        public string Name { set; get; }

    }
}
