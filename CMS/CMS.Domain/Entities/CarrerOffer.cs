using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace CMS.Domain.Entities
{
    public class CarrerOffer
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Position is required.")]
        public string Position { get; set; }

        [Required(ErrorMessage = "YearsOfExperience is required.")]
        public int YearsOfExperience { get; set; }

        [Required(ErrorMessage = "LongDescription is required.")]
        public string LongDescription { get; set; }
        [Required(ErrorMessage = "CreatedBy is required.")]
        public string CreatedBy { get; set; }
        public DateTime CreatedDateTime { get; set; } = DateTime.Now;
    }
}
