using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LavoroAPI.Models
{
    public class Contacts
    {
        public int ID { get; set; }
        public string ContactName { get; set; }
        public string Title { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Avatar { get; set; }
        public string Company { get; set; }
        public bool Favorite { get; set; }
        public int AccountID { get; set; }
        public int ProviderID { get; set; }
    }
}