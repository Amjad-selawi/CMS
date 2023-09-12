using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace CMS.Application.DTOs
{
    public class CarrerOfferDTO
    {
        public int Id { get; set; }
      
        [Required(ErrorMessage = "YearsOfExperience is required.")]
        public int YearsOfExperience { get; set; }
        [Required(ErrorMessage = "LongDescription is required.")]
        public string LongDescription { get; set; }
     
        //public PositionDTO positionDTO { get; set; }


        public int PositionId { set; get; }
        public string PositionName { set; get; }

    }
}
