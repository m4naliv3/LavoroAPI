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

namespace LavoroAPI.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class MessagesController : ApiController
    {
        // GET: api/Messages/5
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
    }
}
