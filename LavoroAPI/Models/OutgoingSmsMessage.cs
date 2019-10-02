using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LavoroAPI.Models
{
    public class OutgoingSmsMessage
    {
        public string MessageText { get; set; }
        public int ConversationID { get; set; }
        public string Author { get; set; }
    }
}