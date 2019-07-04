using Dapper;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Text;
using System.Web.Http;
using Twilio.TwiML;
using Twilio.TwiML.Messaging;

namespace LavoroAPI.Controllers
{
    public class MessagingController : ApiController
    {
        // POST: api/Messaging
        public HttpResponseMessage Post([FromBody]string value)
        {
            /*
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
            }*/
            
            // Need to post message  to the db
            // if it is incoming then I only need to post it
            // if it is outgoing then I need to also send it back to the correct recipient
            
            Message message = new Message();
            message.From = "+16613494046";
            message.To = "+16615930958";
            message.BodyAttribute = "Yo";
            MessagingResponse response = new MessagingResponse();
            response.Append(message);

            return new HttpResponseMessage() { Content = new StringContent(response.ToString(), Encoding.UTF8, "application/xml") };            
        }
    }

    public class MessagingPost
    {
        public string To { get; set; }
        public string From { get; set; }
        public string Body { get; set; }
    }
}
