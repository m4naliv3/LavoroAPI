using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Http;
using System.Web.Http;
using Dapper;
using LavoroAPI.Models;
using Newtonsoft.Json;
using Twilio;
using Twilio.Rest.Api.V2010.Account.AvailablePhoneNumberCountry;
using System.Web.Http.Cors;
using System.Collections.Generic;
using Twilio.Rest.Api.V2010.Account;

namespace LavoroAPI.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    [JWTAuthentication]
    public class PhonesController : ApiController
    {

        const string accountSid = "NU UH";
        const string authToken = "BLAH";

        [Route("Phones/{id}")]
        [HttpGet]
        public HttpResponseMessage GetPhoneById(int id)
        {
            Phone phone = new Phone();
            // Return all of the messages back to the Front End
            using (IDbConnection db = new SqlConnection(ConfigurationManager.ConnectionStrings["LavoroDB"].ConnectionString))
            {
                string sql = @"
                    SELECT *
                    FROM lavoro_dev.dbo.Phones
                    WHERE ID = @ID
                ";
                phone = db.Query<Phone>(sql, new { ID = id }).FirstOrDefault();
            }
            var response = JsonConvert.SerializeObject(phone);
            return new HttpResponseMessage() { Headers = { }, Content = new StringContent(response) };
        }

        [Route("Phones/Search")]
        [HttpPost]
        public HttpResponseMessage AvailablePhones([FromBody]int request)
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

        [Route("Phones/Purchase")]
        [HttpPost]
        public HttpResponseMessage PurchasePhone([FromBody]string phone)
        {
            // Need to add the okta token and do a query to see if the token is valid
            // that means we will need to create the use before we can get the phone number

            TwilioClient.Init(accountSid, authToken);

            var incomingPhoneNumber = IncomingPhoneNumberResource.Create(phoneNumber: new Twilio.Types.PhoneNumber(phone));

            var response = JsonConvert.SerializeObject(incomingPhoneNumber);
            return new HttpResponseMessage() { Headers = { }, Content = new StringContent(response) };

        }
    }
    public class PhoneNumberResource
    {
        public string FriendlyName { get; set; }
        public string PhoneNumber { get; set; }
    }
}

