using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Http;
using System.Web.Http;
using Dapper;
using LavoroAPI.Models;
using Newtonsoft.Json;

namespace LavoroAPI.Controllers
{
    public class ContactsController : ApiController
    {
        // GET: api/Contacts
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/Contacts/5
        public HttpResponseMessage Get(int id)
        {
            List<Contacts> contactList = new List<Contacts>();
            // Return all of the messages back to the Front End
            using (IDbConnection db = new SqlConnection(ConfigurationManager.ConnectionStrings["LavoroDB"].ConnectionString))
            {
                string sql = @"
                    SELECT *
                    FROM lavoro_dev.dbo.Contacts
                    WHERE AccountID = @ID
                ";
                var result = db.Query<Contacts>(sql, new { ID = id }).ToList();
                foreach (var r in result)
                {
                    Contacts c = new Contacts
                    {
                        ID = r.ID,
                        ContactName = r.ContactName,
                        Title = r.Title,
                        Email = r.Email,
                        Phone = r.Phone,
                        Avatar = r.Avatar,
                        Company = r.Company,
                        Favorite = r.Favorite,
                        AccountID = r.AccountID,
                        ProviderID = r.ProviderID
                    };
                    contactList.Add(c);
                }
            }
            var response = JsonConvert.SerializeObject(contactList);
            return new HttpResponseMessage() { Headers = { }, Content = new StringContent(response) };
        }

        // POST: api/Contacts
        public void Post([FromBody]Contacts value)
        {
            using (IDbConnection db = new SqlConnection(ConfigurationManager.ConnectionStrings["LavoroDB"].ConnectionString))
            {
                // Connect to DB and insert each one individually
                string sql = @"
                INSERT INTO lavoro_dev.dbo.Contacts
                (
                    ContactName 
	                Title, 
	                Phone,
	                Email, 
	                Avatar, 
                    Company, 
                    Favorite, 
                    AccountID, 
                    ProviderID
                )
                VALUES
                (
                    @ContactName, 
                    @Title, 
                    @Phone, 
                    @Email, 
                    @Avatar,
                    @Company,
                    @Favorite,
                    @AccountID,
                    @ProviderID
                )";
                db.ExecuteScalar(sql, new { value });
            }
        }

        // PUT: api/Contacts/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/Contacts/5
        public void Delete(int id)
        {
        }
    }
}
