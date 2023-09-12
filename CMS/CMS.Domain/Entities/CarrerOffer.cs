using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace CMS.Domain.Entities
{
    public class CarrerOffer : BaseEntity
    {
        public int Id { get; set; }


        [Required(ErrorMessage = "Position is required.")]
        public int PositionId { get; set; }
        public Position Positions { get; set; }



        [Required(ErrorMessage = "YearsOfExperience is required.")]
        public int YearsOfExperience { get; set; }

        [Required(ErrorMessage = "LongDescription is required.")]
        public string LongDescription { get; set; }
     
    }
}
