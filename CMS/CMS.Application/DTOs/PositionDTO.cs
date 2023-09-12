using CMS.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace CMS.Application.DTOs
{
    public class PositionDTO
    {
        public int PositionId { set; get; }

        [Required]
        public string Name { set; get; }

        public List<CarrerOfferDTO> CarrerOfferDTO { get; set; }

        public List<InterviewsDTO> InterviewsDTO { get; set; }
    }
}
