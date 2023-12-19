using System;
using System.Collections.Generic;
using System.Text;

namespace CMS.Domain.Enums
{
    public static class StatusCode
    {
        public static string Pending = "PND";
        public static string Approved = "APR";
        public static string Rejected = "REJ";
        public static string OnHold = "HOLD";


        public static string GetName(string code)
        {
            switch (code)
            {
                case "PND":
                    return "Pending";
                case "APR":
                    return "Approved";
                case "REJ":
                    return "Rejected";
                case "HOLD":
                    return "On hold";

                default:
                    return "Unknown"; 
            }
        }
    }

  
}
