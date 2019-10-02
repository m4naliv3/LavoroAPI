using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LavoroAPI.Models
{
    public class Messages
    {
        public int ID { get; set; }
        public int ConversationID { get; set; }
        public string MessageText { get; set; }
        public bool Direction { get; set; }
        public DateTime SentDate { get; set; }
    }
}