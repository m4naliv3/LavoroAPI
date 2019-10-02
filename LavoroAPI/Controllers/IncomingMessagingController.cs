using Dapper;
using LavoroAPI.Models;
using System;
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
    public class IncomingMessagingController : ApiController
    {
        // POST: api/Messaging
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
                    new {
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
