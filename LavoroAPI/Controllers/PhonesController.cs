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
    public class PhonesController : ApiController
    {
        public HttpResponseMessage Get(int id)
        {
            Phones phone = new Phones();
            // Return all of the messages back to the Front End
            using (IDbConnection db = new SqlConnection(ConfigurationManager.ConnectionStrings["LavoroDB"].ConnectionString))
            {
                string sql = @"
                    SELECT *
                    FROM lavoro_dev.dbo.Phones
                    WHERE ID = @ID
                ";
                phone = db.Query<Phones>(sql, new { ID = id }).FirstOrDefault();
            }
            var response = JsonConvert.SerializeObject(phone);
            return new HttpResponseMessage() { Headers = { }, Content = new StringContent(response) };
        }
    }
}
