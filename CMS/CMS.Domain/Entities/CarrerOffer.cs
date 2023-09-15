﻿using Microsoft.AspNetCore.Identity;
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
        public int PositionId { get; set; }
        public virtual Position Position { get; set; }

        [Required(ErrorMessage = "YearsOfExperience is required.")]
        public int YearsOfExperience { get; set; }

        [Required(ErrorMessage = "LongDescription is required.")]
        public string LongDescription { get; set; }
        [Required(ErrorMessage = "CreatedBy is required.")]
        public int CreatedBy { get; set; }
        public virtual IdentityUser Creator { get; set; }
        public DateTime CreatedDateTime { get; set; } = DateTime.Now;
    }
}
