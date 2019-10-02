using Dapper;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace LavoroAPI
{
    public class DbManager
    {
        public static string CreateUser(string username, string password)
        {
            if (!CheckUser(username))
            {
                // Need to tie it into an account id
                string salt = Encryption.GetSalt();
                string pass = Encryption.HashString(password);
                string hashedAndSalted = Encryption.HashString(pass + salt);
                using (IDbConnection db = new SqlConnection(ConfigurationManager.ConnectionStrings["LavoroDB"].ConnectionString))
                {
                    string query = @"INSERT INTO lavoro_dev.dbo.Users(UserName, Password, Salt, AccountId) VALUES(@User, @Password, @Salt, @AccountId)";
                    db.Execute(query, new { Username = username, Password = hashedAndSalted, Salt = salt, AccountId = 1 });
                }
                return "created new user " + username;
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
                string query = @"SELECT UserName FROM lavoro_dev.dbo.Users WHERE UserName = @Username";
                user = db.Query<string>(query, new { Username = username }).FirstOrDefault();
            }
            bool kosher = (user == null) ? false : true;
            // Need to query db to see if the username is unique
            return kosher;
        }

        public static string Login(string username, string password)
        {
            // Need to create a User table????
            // Need to get the salt using the user on the account
            // Users table needs to have AccountId, UserName, Password, Salt
            string salt;
            string accountKey;
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
                accountKey = db.Query<string>(loginQuery, new { Username = username, Password = hashedAndSalted }).FirstOrDefault();
            }
            // need to handle the Exception case to return a retry message
            if (accountKey == null) return "You screwed up";
            return accountKey;
        }
    }
}