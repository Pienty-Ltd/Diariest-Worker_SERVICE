using System;
using System.Security.Cryptography;

namespace Pienty.CRM.Core.Helpers
{
    public class CryptoHelper
    {
        
        public static string GenerateSecureToken()
        {
            using (var rng = new RNGCryptoServiceProvider())
            {
                var tokenData = new byte[32];
                rng.GetBytes(tokenData);
                return Convert.ToBase64String(tokenData);
            }
        }

        public static string EncryptPassword(string input)
        {
            return BCrypt.Net.BCrypt.HashPassword(input);
        }

        public static bool VerifyPassword(string input, string hashInput)
        {
            return BCrypt.Net.BCrypt.Verify(input, hashInput);
        }
        
    }
}