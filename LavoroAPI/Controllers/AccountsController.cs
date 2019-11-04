using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Http;
using System.Web.Http;
using Dapper;
using LavoroAPI.Models;
using Newtonsoft.Json;
using System.Web.Http.Cors;

namespace LavoroAPI.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class AccountsController : ApiController
    {
        [Route("Accounts/{id}")]
        [HttpGet]
        public HttpResponseMessage GetAccountById(int id)
        {
            Accounts account = new Accounts();
            // Return all of the messages back to the Front End
            using (IDbConnection db = new SqlConnection(ConfigurationManager.ConnectionStrings["LavoroDB"].ConnectionString))
            {
                string sql = @"
                    SELECT *
                    FROM lavoro_dev.dbo.Accounts
                    WHERE ID = @ID
                ";
                account = db.Query<Accounts>(sql, new { ID = id }).FirstOrDefault();
            }
            var response = JsonConvert.SerializeObject(account);
            return new HttpResponseMessage() { Headers = { }, Content = new StringContent(response) };
        }

        [Route("Accounts/Create")]
        [HttpPost]
        public void CreateAccount([FromBody] Accounts value)
        {
            using (IDbConnection db = new SqlConnection(ConfigurationManager.ConnectionStrings["LavoroDB"].ConnectionString))
            {
                // Connect to DB and insert each one individually
                string sql = @"
                INSERT INTO lavoro_dev.dbo.Accounts
                (
                    BusinessName, 
                    RingTo, 
                    PhoneNumberID, 
                    UserName, 
                    Avatar
                )
                VALUES
                (
                    @BusinessName, 
                    @RingTo, 
                    @PhoneNumberID, 
                    @UserName, 
                    @Avatar
                )";
                db.ExecuteScalar(sql, new { value });
            }
        }
    }
}
