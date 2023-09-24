using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace CMS.Application.DTOs
{
    public class NotificationsDTO
    {
        
        public int NotificationsId { get; set; }

        public string ReceiverId { get; set; }

        public DateTime SendDate { get; set; }

        public bool IsReceived { get; set; }

        public TemplatesDTO templatesDTO { get; set; }

        public string Title { get; set; }
        public string BodyDesc { get; set; }
        public bool IsRead { get; set; }

        //public string TemplateName { get; set; }



    }
}
