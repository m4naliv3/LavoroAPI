using Dapper;
using LavoroAPI.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using System.Web.Http.Cors;
using Twilio.TwiML;
using Twilio.TwiML.Messaging;

namespace LavoroAPI.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class MessagesController : ApiController
    {

        [Route("Messages/Conversation")]
        [HttpPost]
        public HttpResponseMessage ConversationLookup(PhoneLookup request)
        {
            // Get open ConversationId or create one and then return it
            using (IDbConnection db = new SqlConnection(ConfigurationManager.ConnectionStrings["LavoroDB"].ConnectionString))
            {
                string sql = @"
                    SELECT con.ID, con.CallerNo [Phone]
                    FROM lavoro_dev.dbo.Conversations con
	                    INNER JOIN lavoro_dev.dbo.Phones phn
		                    ON phn.PhoneNumber = con.InboundNo
                    WHERE CallerNo = @Phone
                      AND phn.ID = @PhoneNumberID
                ";
                PhoneLookup value = db.Query<PhoneLookup>(sql, new { PhoneNumberID = request.ID, request.Phone }).FirstOrDefault();
                if (value == null)
                {
                    string query = @"SELECT PhoneNumber FROM lavoro_dev.dbo.Phones WHERE ID = @ID";
                    string accountNumber = db.Query<string>(query, new { request.ID }).FirstOrDefault();
                    string insertSql = @"
                        INSERT INTO lavoro_dev.dbo.Conversations(InboundNo, CallerNo)
                        VALUES(@AccountNumber, @Phone)
                    ";
                    db.ExecuteScalar<int>(insertSql, new { AccountNumber = accountNumber, request.Phone });
                    value = db.Query<PhoneLookup>(sql, new { PhoneNumberID = request.ID, request.Phone }).FirstOrDefault();
                }
                var response = JsonConvert.SerializeObject(value);
                return new HttpResponseMessage() { Headers = { }, Content = new StringContent(response) };
            }
        }

        [Route("Messages/GetMessages/{id}")]
        public HttpResponseMessage GetMessages(int id)
        {
            // Get all messages for this conversation
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

        [Route("Messages/Inbound")]
        [HttpPost]
        public HttpResponseMessage PostIncomingMessage([FromBody]TwilioIncomingSmsMessage value)
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

        [Route("Messages/Outbound")]
        [HttpPost]
        public void PostOutgoingMessage([FromBody]OutgoingSmsMessage value)
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
                db.Execute(sql, new { value.MessageText, value.ConversationID, value.Author });
                string query = @"SELECT CallerNo FROM lavoro_dev.dbo.Conversations WHERE ID = CAST(@ID AS nvarchar(16))";
                target = db.Query<string>(query, new { ID = value.ConversationID.ToString() }).FirstOrDefault();
            }
        }
    }
    public class PhoneLookup
    {
        public int ID { get; set; }
        public string Phone { get; set; }
    }
}
