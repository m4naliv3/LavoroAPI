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
    public class ConversationsController : ApiController
    {
        // POST: api/Conversations
        public HttpResponseMessage Post(PhoneLookup request)
        {
            // Get the conversation id and the phone number associated
            // If no open conversation then create one
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
    }

    public class PhoneLookup
    {
        public int ID { get; set; }
        public string Phone { get; set; }
    }
}
