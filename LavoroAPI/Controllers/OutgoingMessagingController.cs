using Dapper;
using LavoroAPI.Models;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Cors;
using Twilio;
using Twilio.Rest.Api.V2010.Account;

namespace LavoroAPI.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class OutgoingMessagingController : ApiController
    {
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
    }
}
