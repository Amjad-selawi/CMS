using System;
using System.Collections.Generic;
using System.Text;

namespace CMS.Domain.Entities
{
    public class Interviewer
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public int Phone { get; set; }
        public string Email { get; set; }
    }
}
