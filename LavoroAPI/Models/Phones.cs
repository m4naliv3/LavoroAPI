using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LavoroAPI.Models
{
    public class Phones
    {
        public int ID { get; set; }
        public string PhoneNumber { get; set; }
        public string RingTo { get; set; }
        public string TwilioSid { get; set; }
    }
}