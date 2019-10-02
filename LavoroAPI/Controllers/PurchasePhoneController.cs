using Newtonsoft.Json;
using System.Net.Http;
using System.Web.Http;
using Twilio;
using Twilio.Rest.Api.V2010.Account;

namespace LavoroAPI.Controllers
{
    public class PurchasePhoneController : ApiController
    {
        const string accountSid = "AC618dc060cc8ca10b4101a3ecfbaf1553";
        const string authToken = "5bd534fd7ba46a0f79104ac5d0de7bb4";

        // POST: api/PurchasePhone
        public HttpResponseMessage Post([FromBody]string phone)
        {
            // Need to add the okta token and do a query to see if the token is valid
            // that means we will need to create the use before we can get the phone number

            TwilioClient.Init(accountSid, authToken);

            var incomingPhoneNumber = IncomingPhoneNumberResource.Create(phoneNumber: new Twilio.Types.PhoneNumber(phone));

            var response = JsonConvert.SerializeObject(incomingPhoneNumber);
            return new HttpResponseMessage() { Headers = { }, Content = new StringContent(response) };
            
        }
    }
}
