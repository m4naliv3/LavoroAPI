using Dapper;
using LavoroAPI.Models;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.TwiML;
using Twilio.TwiML.Messaging;

namespace LavoroAPI.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class MessagesController : ApiController
    {
        [Route("Messages/{id}")]
        [HttpGet]
        public HttpResponseMessage Get(int id)
        {
            List<Messages> messageList = new List<Messages>();
            // Return all of the messages back to the Front End
            using (IDbConnection db = new SqlConnection(ConfigurationManager.ConnectionStrings["LavoroDB"].ConnectionString))
            {
                string sql = @"
                    SELECT TOP 100 *
                    FROM lavoro_dev.dbo.ChatMessages
                    WHERE ConversationID = @ID
                    ORDER BY SentDate
                ";
                var result = db.Query<Messages>(sql, new { ID = id }).ToList();
                foreach (var r in result)
                {
                    Messages m = new Messages
                    {
                        ID = r.ID,
                        MessageText = r.MessageText,
                        Direction = r.Direction,
                        ConversationID = r.ConversationID,
                        SentDate = r.SentDate
                    };
                    messageList.Add(m);
                }
            }
            var response = JsonConvert.SerializeObject(messageList);
            return new HttpResponseMessage() { Headers = { }, Content = new StringContent(response) };
        }

        [Route("Messages/Outgoing")]
        [HttpPost]
        public void Post([FromBody]OutgoingSmsMessage value)
        {
            string target = string.Empty;
            using (IDbConnection db = new SqlConnection(ConfigurationManager.ConnectionStrings["LavoroDB"].ConnectionString))
            {
                // Connect to DB and insert each one individually
                string sql = @"
                INSERT INTO lavoro_dev.dbo.ChatMessages
                (
                    MessageText,
                    Author,
                    ConversationID,
                    Direction,
                    SentDate
                )
                VALUES
                (
                    @MessageText,
                    @Author,
                    @ConversationID,
                    1,
                    GETDATE()
                )";
                db.ExecuteScalar(sql, new { value.MessageText, value.ConversationID, value.Author });
                string query = @"SELECT CallerNo FROM lavoro_dev.dbo.Conversations WHERE ID = CAST(@ID AS nvarchar(16))";
                target = db.Query<string>(query, new { ID = value.ConversationID.ToString() }).FirstOrDefault();
            }
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
        public HttpResponseMessage Post([FromBody]TwilioIncomingSmsMessage value)
        {
            // Find the phone number associated with this Twilio number
            Phones phone = new Phones();
            using (IDbConnection getdb = new SqlConnection(ConfigurationManager.ConnectionStrings["LavoroDB"].ConnectionString))
            {
                string query = @"SELECT ID FROM lavoro_dev.dbo.Conversations WHERE InboundNo = @To AND CallerNo = @From";
                phone = getdb.Query<Phones>(query, new { value.To, value.From }).FirstOrDefault();
            }
            // Add the message to the DB
            using (IDbConnection db = new SqlConnection(ConfigurationManager.ConnectionStrings["LavoroDB"].ConnectionString))
            {
                string sql = @"
                INSERT INTO lavoro_dev.dbo.ChatMessages
                (
                    MessageText,
                    Author,
                    ConversationID,
	                Direction,
	                SentDate
                )
                VALUES
                (
                    @MessageText,
	                @Author,
	                @ConversationID,
	                0,
	                @SentDate
                )";
                db.Query(
                    sql,
                    new
                    {
                        MessageText = value.Body,
                        Author = value.From,
                        ConversationID = phone.ID,
                        SentDate = DateTime.Now
                    }
                );
            }

            // Using the phone number found in the db pass the message along to the subscribed Lavoro user
            Message message = new Message();
            message.From = value.To;
            message.To = phone.RingTo;
            message.BodyAttribute = value.Body;
            MessagingResponse response = new MessagingResponse();
            response.Append(message);

            return new HttpResponseMessage() { Content = new StringContent(response.ToString(), Encoding.UTF8, "application/xml") };
        }
    }
}
