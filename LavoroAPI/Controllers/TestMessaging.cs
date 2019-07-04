using System.Web.Mvc;
using Twilio.AspNet.Mvc;
using Twilio.TwiML;

namespace LavoroAPI.Controllers
{
    public class TestMessaging : TwilioController
    {
        [Route("Test")]
        [HttpPost]
        public ActionResult Message(string From, string Body)
        {
            var twiml = new MessagingResponse();
            var message = twiml.Message($"Hello {From}. You said {Body}");
            return TwiML(message);
        }
    }
}