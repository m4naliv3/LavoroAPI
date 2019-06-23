using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LavoroAPI.Models
{
    public class Accounts
    {
        public int ID { get; set; }
        public string BusinessName { get; set; }
        public string RingTo { get; set; }
        public int PhoneNumberID { get; set; }
        public string UserName { get; set; }
        public string Avatar { get; set; }
    }
}