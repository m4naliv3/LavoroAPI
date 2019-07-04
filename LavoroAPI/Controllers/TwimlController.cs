using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Mvc;
using Twilio.AspNet.Mvc;
using Twilio.TwiML;

namespace LavoroAPI.Controllers
{
    public class TwimlController : TwilioController
    {
        // GET: api/Twiml
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/Twiml/5
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/Twiml
        public ActionResult Post([FromBody]string value)
        {
            var twiml = new MessagingResponse();
            var message = twiml.Message($"Hello  You said ");
            return TwiML(message);
        }
        
    

        // PUT: api/Twiml/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/Twiml/5
        public void Delete(int id)
        {
        }
    }
}
