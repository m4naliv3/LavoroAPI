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
        public string FriendlyName { get; set; }
        public string City { get; set; }
        public string Region { get; set; }
        public string Country { get; set; }
        public string Zip { get; set; }
    }
}