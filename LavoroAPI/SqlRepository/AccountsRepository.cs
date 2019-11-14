using Dapper;
using LavoroAPI.Models;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace LavoroAPI.SqlRepository
{
    public class AccountsRepository
    {
        public Account GetAccountById(int id)
        {
            Account account = new Account();
            using (IDbConnection db = new SqlConnection(ConfigurationManager.ConnectionStrings["LavoroDB"].ConnectionString))
            {
                string sql = @"
                    SELECT *
                    FROM lavoro_dev.dbo.Accounts
                    WHERE ID = @ID
                ";
                account = db.Query<Account>(sql, new
                {
                    ID = id
                }).FirstOrDefault();
            }
            return account;
        }

        public void CreateAccount(Account account)
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
                db.Execute(sql, new { account });
            }
        }

        public List<Contact> GetAccountContacts(int id)
        {
            List<Contact> contactList = new List<Contact>();
            // Return all of the messages back to the Front End
            using (IDbConnection db = new SqlConnection(ConfigurationManager.ConnectionStrings["LavoroDB"].ConnectionString))
            {
                string sql = @"
                    SELECT *
                    FROM lavoro_dev.dbo.Contacts
                    WHERE AccountID = @ID
                ";
                var result = db.Query<Contact>(sql, new { ID = id }).ToList();
                foreach (var r in result)
                {
                    Contact c = new Contact
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
            return contactList;
        }
        
        public void CreateAccountContact(Contact contact)
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
                db.Execute(sql, new
                {
                    contact.ContactName,
                    contact.Title,
                    contact.Phone,
                    contact.Email,
                    contact.Avatar,
                    contact.Company,
                    contact.Favorite,
                    contact.AccountID,
                    contact.ProviderID
                });
            }
        }

        public PhoneLookup GetConversation(PhoneLookup request)
        {
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
                return value;
            }
        }
    }
}