﻿using System.Collections.Generic;
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
    public class AccountsController : ApiController
    {
        // GET: api/Accounts/5
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
        public void Post([FromBody] Accounts value)
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

        // PUT: api/Accounts/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/Accounts/5
        public void Delete(int id)
        {
        }
    }
}
