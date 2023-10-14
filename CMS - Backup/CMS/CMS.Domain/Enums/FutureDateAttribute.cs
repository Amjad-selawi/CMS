using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;
namespace CMS.Domain.Enums
{
    public class FutureDateAttribute:ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            if(value is DateTime date)
            {
                return date >= DateTime.Now;
            }
            return false;
        }
    }
}
