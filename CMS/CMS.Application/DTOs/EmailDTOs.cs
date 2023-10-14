using System;
using System.Collections.Generic;
using System.Text;

namespace CMS.Application.DTOs
{
    public class EmailDTOs
    {//Updated class
        public List<string> EmailTo { get; set; }
        public string EmailBody { get; set; }
        public string Subject { get; set; }
    }
}
