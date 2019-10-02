using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LavoroAPI.Models
{
    public class Conversation
    {
        public int ID { get; set; }
        public string InboundNo { get; set; }
        public string CallerNo { get; set; }
    }
}