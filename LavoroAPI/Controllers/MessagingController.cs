using System.Collections.Generic;
using System.Web.Http;
using Twilio.TwiML.Messaging;

namespace LavoroAPI.Controllers
{
    public class MessagingController : ApiController
    {
        // GET: api/Messaging
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/Messaging/5
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/Messaging
        public string Post([FromBody]string value)
        {
            Message message = new Message();
            message.From = "6613494046";
            message.To = "6615930958";
            message.BodyAttribute = "Hello World!";
            Twilio.TwiML.MessagingResponse response = new Twilio.TwiML.MessagingResponse();
            response.Append(message);
            return response.ToString();
        }

        // PUT: api/Messaging/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/Messaging/5
        public void Delete(int id)
        {
        }
    }
}
