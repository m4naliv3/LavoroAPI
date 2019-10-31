using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;
using System.Web.Http;
using Twilio;
using Twilio.Rest.Api.V2010.Account.AvailablePhoneNumberCountry;

namespace LavoroAPI.Controllers
{
    public class PhoneResourceController : ApiController
    {
        const string accountSid = "AC618dc060cc8ca10b4101a3ecfbaf1553";
        const string authToken = "5bd534fd7ba46a0f79104ac5d0de7bb4";

        // POST: api/PhoneResource
        [Route("Phones")]
        [HttpPost]
        public HttpResponseMessage Post([FromBody]int request)
        {
            TwilioClient.Init(accountSid, authToken);
            var local = LocalResource.Read(areaCode: request, pathCountryCode: "US", limit: 5);

            List<PhoneNumberResource> localList = new List<PhoneNumberResource>();
            foreach (var record in local)
            {
                PhoneNumberResource phone = new PhoneNumberResource()
                {
                    FriendlyName = record.FriendlyName.ToString(),
                    PhoneNumber = record.PhoneNumber.ToString()
                };
                localList.Add(phone);
            }

            var response = JsonConvert.SerializeObject(localList);
            return new HttpResponseMessage() { Headers = { }, Content = new StringContent(response) };
        }
    }
    public class PhoneNumberResource
    {
        public string FriendlyName { get; set; }
        public string PhoneNumber { get; set; }
    }
}
