using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace CMS.Application.DTOs
{
    public class CountryDTO
    {//Updated class
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public List<CompanyDTO> companyDTOs { get; set; }
    }
}
