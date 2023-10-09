using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace CMS.Application.DTOs
{
    public class CompanyDTO
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        public string Address { get; set; }
        public string PhoneNumber { set; get; }
        [Required]
        public int CountryId { set; get; }
        public string CountryName { set; get; }
    }
}
