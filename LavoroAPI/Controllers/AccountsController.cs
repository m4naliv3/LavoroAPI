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
using System.Collections.Generic;

namespace LavoroAPI.Controllers

    ///
    ///
    ///  GONNA HAVE TO CHANGE ALL OF THIS
    ///
    ///
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class AccountsController : ApiController
    {
        // GET: api/Accounts/5
        [Route("Account/GetAccount/{id}")]
        [HttpGet]
        public HttpResponseMessage Get(int id)
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

        // POST: api/Accounts
        [Route("Account/Create")]
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

        [Route("Account/CheckName")]
        [HttpGet]
        public HttpResponseMessage CheckUsername()
        {
            string stuff = true ? "This username has already been taken" : "This username is available";
            var response = JsonConvert.SerializeObject(stuff);
            return new HttpResponseMessage() { Headers = { }, Content = new StringContent(response) };
        }

        [Route("Account/Contact/{id}")]
        [HttpGet]
        public HttpResponseMessage GetAccountContacts(int id)
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
        [Route("Account/Contact")]
        [HttpPost]
        public void PostAccountContact([FromBody]Contacts value)
        {
            using (IDbConnection db = new SqlConnection(ConfigurationManager.ConnectionStrings["LavoroDB"].ConnectionString))
            {
                // Connect to DB and insert each one individually
                string sql = @"
                INSERT INTO lavoro_dev.dbo.Contacts
                (
                    ContactName, 
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
                db.ExecuteScalar(sql, new
                {
                    value.ContactName,
                    value.Title,
                    value.Phone,
                    value.Email,
                    value.Avatar,
                    value.Company,
                    value.Favorite,
                    value.AccountID,
                    value.ProviderID
                }
                );
            }
        }
    }

    public class NewUser
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string BusinessName { get; set; }
        public string Email { get; set; }
        public string Avatar { get; set; }
    }
}