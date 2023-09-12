using CMS.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace CMS.Domain.Entities
{
    public class Notifications : BaseEntity
    {
        public int NotificationsId { get; set; }


        public int ReceiverId { get; set; }

        public int TemplatesId { get; set; }
        public Templates Templates { get; set; }

        public DateTime SendDate { get; set; }

        public bool IsReceived { get; set; }


        //public string TemplateName { get; set; }



    }
}
