using System;
using System.Security.Cryptography;
using System.Text;

namespace LavoroAPI
{
    public class Encryption
    {
        public static string LoginToken(string password)
        {
            string salt = Encryption.GetSalt();
            return Encryption.HashString(password + salt);
        }

        public static string HashString(string str)
        {
            Encryption encrypt = new Encryption();       
            return encrypt.EncryptString(str).ToString();
        }

        public static string GetSalt()
        {
            var random = new RNGCryptoServiceProvider();

            // Maximum length of salt
            int max_length = 32;

            byte[] salt = new byte[max_length];

            // Build the random bytes

            random.GetNonZeroBytes(salt);
            // Return the string encoded salt
            return Convert.ToBase64String(salt);
        }

        private string EncryptString(string str) { 
            byte[] bytes = Encoding.ASCII.GetBytes(str);
            byte[] hashed = SHA256.Create().ComputeHash(bytes);
            return Convert.ToBase64String(hashed);
        }
    }
}