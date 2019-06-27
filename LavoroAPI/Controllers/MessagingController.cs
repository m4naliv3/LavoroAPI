using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Web.Http;
using Twilio.TwiML;
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
        public HttpResponseMessage Post([FromBody]string value)
        {
            Message message = new Message();
            message.From = "6613494046";
            message.To = "6615930958";
            message.BodyAttribute = "Hello World!";
            MessagingResponse response = new MessagingResponse();
            response.Append(message);

            return this.Request.CreateResponse(
                HttpStatusCode.OK, response.ToString(), new XmlMediaTypeFormatter()
            );
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
