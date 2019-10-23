using Dapper;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Http;

namespace LavoroAPI
{
    public class DbManager
    {
        public static string CreateAccount(string username, string password, string businessName, string email, string avatar)
        {
            if (!CheckUser(username))
            {
                string salt = Encryption.GetSalt();
                string pass = Encryption.HashString(password);
                string hashedAndSalted = Encryption.HashString(pass + salt);
                using (IDbConnection db = new SqlConnection(ConfigurationManager.ConnectionStrings["LavoroDB"].ConnectionString))
                {
                    string query = @"
                    INSERT INTO lavoro_dev.dbo.Accounts(
                        UserName, 
                        Password, 
                        Salt, 
                        PhoneNumberId,
                        BusinessName,
                        Email,
                        Avatar
                    ) 
                    VALUES(
                        @Username, 
                        @Password, 
                        @Salt, 
                        @PhoneNumberId,
                        @BusinessName,
                        @Email,
                        @Avatar
                    )";
                    db.Execute(query, new {
                        Username = username,
                        Password = hashedAndSalted,
                        Salt = salt,
                        PhoneNumberId = 1, // Need to get the phone number id
                        BusinessName = businessName,
                        Email = email,
                        Avatar = avatar
                    }); 
                }
                return "created new Account " + username;
            }
            else
            {
                return "User already exists";
                // retunr a prompt to let the user know that the name already exists
            }
        }

        public static bool CheckUser(string username)
        {
            string user;
            using (IDbConnection db = new SqlConnection(ConfigurationManager.ConnectionStrings["LavoroDB"].ConnectionString))
            {
                string query = @"SELECT UserName FROM lavoro_dev.dbo.Accounts WHERE UserName = @Username";
                user = db.Query<string>(query, new { Username = username }).FirstOrDefault();
            }
            bool kosher = (user == null) ? false : true;
            // Need to query db to see if the username is unique
            return kosher;
        }

        public static int GetIdByUser(string user)
        {
            using (IDbConnection db = new SqlConnection(ConfigurationManager.ConnectionStrings["LavoroDB"].ConnectionString))
            {
                string query = @"SELECT AccountID FROM lavoro_dev.dbo.Accounts WHERE UserName = @UserName";
                int? id = db.Query<int?>(query, new { UserName = user }).FirstOrDefault();
                return id.Value;
            }
        }

        public static int Login(string username, string password)
        {
            // Need to create a User table????
            // Need to get the salt using the user on the account
            // Users table needs to have AccountId, UserName, Password, Salt
            string salt;
            int? accountKey;
            using (IDbConnection db = new SqlConnection(ConfigurationManager.ConnectionStrings["LavoroDB"].ConnectionString))
            {
                string saltQuery = @"SELECT Salt FROM lavoro_dev.dbo.Users WHERE UserName = @Username";
                salt = db.Query<string>(saltQuery, new { Username = username }).FirstOrDefault();
            }
            string pass = Encryption.HashString(password);
            string hashedAndSalted = Encryption.HashString(pass + salt);

            using (IDbConnection db = new SqlConnection(ConfigurationManager.ConnectionStrings["LavoroDB"].ConnectionString))
            {
                // Need to  figure out what to return here that ensures we are logged in and logged into the correct account
                // to return the proper values
                string loginQuery = @"SELECT AccountId FROM lavoro_dev.dbo.Users WHERE UserName = @Username AND Password = @Password";
                accountKey = db.Query<int>(loginQuery, new { Username = username, Password = hashedAndSalted }).FirstOrDefault();
            }
            // need to handle the Exception case to return a retry message
            if (accountKey == null) return 0;

            return accountKey.Value;
        }
    }
}