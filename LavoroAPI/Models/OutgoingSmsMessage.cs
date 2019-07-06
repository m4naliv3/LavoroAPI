using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LavoroAPI.Models
{
    public class OutgoingSmsMessage
    {
        public string Body { get; set; }
        public string To { get; set; }
        public string From { get; set; }
        public int ConversationID { get; set; }
    }
}