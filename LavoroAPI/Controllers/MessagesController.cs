using LavoroAPI.Models;
using LavoroAPI.SqlRepository;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using System.Web.Http.Cors;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.TwiML;

namespace LavoroAPI.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    [JWTAuthentication]
    public class MessagesController : LavoroApiController
    {
        MessagesRepository _messagesRepository = new MessagesRepository();

        [Route("Messages/{id}")]
        [HttpGet]
        public HttpResponseMessage GetMessagesByConversationId(int id)
        {
            List<Models.Message> messages = _messagesRepository.GetConversationMessages(id);
            return JsonResponse(messages);
        }

        [Route("Messages/Outgoing")]
        [HttpPost]
        public void SendMessage([FromBody]OutgoingSmsMessage value)
        {
            string target = _messagesRepository.SendMessage(value);

            const string accountSid = "AC618dc060cc8ca10b4101a3ecfbaf1553";
            const string authToken = "5bd534fd7ba46a0f79104ac5d0de7bb4";
            TwilioClient.Init(accountSid, authToken);

            var message = MessageResource.Create(
                body: value.MessageText,
                from: new Twilio.Types.PhoneNumber(value.Author),
                to: new Twilio.Types.PhoneNumber(target)
            );
        }

        [Route("Messages/Incoming")]
        [HttpPost]
        public HttpResponseMessage InsertMessage([FromBody]TwilioIncomingSmsMessage incomingMessage)
        {
            Phone phone =_messagesRepository.CreateIncomingMessage(incomingMessage);

            // Using the phone number found in the db pass the message along to the subscribed Lavoro user
            Twilio.TwiML.Messaging.Message message = new Twilio.TwiML.Messaging.Message();
            message.From = incomingMessage.To;
            message.To = phone.RingTo;
            message.BodyAttribute = incomingMessage.Body;
            MessagingResponse response = new MessagingResponse();
            response.Append(message);

            return new HttpResponseMessage() { Content = new StringContent(response.ToString(), Encoding.UTF8, "application/xml") };
        }
    }
}
