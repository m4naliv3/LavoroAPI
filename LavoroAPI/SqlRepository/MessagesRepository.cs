using Dapper;
using LavoroAPI.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace LavoroAPI.SqlRepository
{
    public class MessagesRepository
    {

        public List<Message> GetConversationMessages(int conversationId)
        {
            List<Message> messageList = new List<Message>();
            // Return all of the messages back to the Front End
            using (IDbConnection db = new SqlConnection(ConfigurationManager.ConnectionStrings["LavoroDB"].ConnectionString))
            {
                string sql = @"
                    SELECT TOP 100 *
                    FROM lavoro_dev.dbo.ChatMessages
                    WHERE ConversationID = @ConversationId
                    ORDER BY SentDate
                ";
                var result = db.Query<Message>(sql, new { ConversationId = conversationId }).ToList();
                foreach (var r in result)
                {
                    Message m = new Message
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
            return messageList;
        }

        public string SendMessage(OutgoingSmsMessage message)
        {
            string target = string.Empty;
            using (IDbConnection db = new SqlConnection(ConfigurationManager.ConnectionStrings["LavoroDB"].ConnectionString))
            {
                // Connect to DB and insert each one individually
                string sql = @"
                INSERT INTO lavoro_dev.dbo.ChatMessages
                (
                    MessageText,
                    Author,
                    ConversationID,
                    Direction,
                    SentDate
                )
                VALUES
                (
                    @MessageText,
                    @Author,
                    @ConversationID,
                    1,
                    GETDATE()
                )";
                db.Execute(sql, new { message.MessageText, message.ConversationID, message.Author });
                string query = @"SELECT CallerNo FROM lavoro_dev.dbo.Conversations WHERE ID = CAST(@ID AS nvarchar(16))";
                return db.Query<string>(query, new { ID = message.ConversationID.ToString() }).FirstOrDefault();
            }
        }

        public Phone CreateIncomingMessage(TwilioIncomingSmsMessage message)
        {
            // Find the phone number associated with this Twilio number
            Phone phone = new Phone();
            using (IDbConnection getdb = new SqlConnection(ConfigurationManager.ConnectionStrings["LavoroDB"].ConnectionString))
            {
                string query = @"SELECT ID FROM lavoro_dev.dbo.Conversations WHERE InboundNo = @To AND CallerNo = @From";
                phone = getdb.Query<Phone>(query, new { message.To, message.From }).FirstOrDefault();
            }
            // Add the message to the DB
            using (IDbConnection db = new SqlConnection(ConfigurationManager.ConnectionStrings["LavoroDB"].ConnectionString))
            {
                string sql = @"
                INSERT INTO lavoro_dev.dbo.ChatMessages
                (
                    MessageText,
                    Author,
                    ConversationID,
	                Direction,
	                SentDate
                )
                VALUES
                (
                    @MessageText,
	                @Author,
	                @ConversationID,
	                0,
	                @SentDate
                )";
                db.Query(
                    sql,
                    new
                    {
                        MessageText = message.Body,
                        Author = message.From,
                        ConversationID = phone.ID,
                        SentDate = DateTime.Now
                    }
                );
            }
            return phone;
        }
    }
}