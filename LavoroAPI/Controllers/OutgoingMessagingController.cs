using Dapper;
using LavoroAPI.Models;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using Twilio.TwiML;
using Twilio.TwiML.Messaging;

namespace LavoroAPI.Controllers
{
    public class OutgoingMessagingController : ApiController
    {
        public HttpResponseMessage Post([FromBody]OutgoingSmsMessage value)
        {
            using (IDbConnection db = new SqlConnection(ConfigurationManager.ConnectionStrings["LavoroDB"].ConnectionString))
            {
                // Connect to DB and insert each one individually
                string sql = @"
                INSERT INTO lavoro_dev.dbo.ChatMessages
                (
                    MessageText,
                    Sender,
                    Recipient
                )
                VALUES
                (
                    @MessageBody,
                    @To,
                    @From
                )";
                db.ExecuteScalar(sql, new { MessageBody = value.Body, To = value.To, From = value.From });
            }

            Message message = new Message();
            message.From = value.From;
            message.To = value.To;
            message.BodyAttribute = value.Body;
            MessagingResponse response = new MessagingResponse();
            response.Append(message);

            return new HttpResponseMessage() { Content = new StringContent(response.ToString(), Encoding.UTF8, "application/xml") };
        }
    }
}
