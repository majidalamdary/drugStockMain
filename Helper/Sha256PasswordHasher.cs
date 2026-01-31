
using System;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNet.Identity;

namespace DrugStockWeb.Helper
{

    public class Sha256PasswordHasher : IPasswordHasher
    {

        private const string FixedSalt = "13650621"; // 🔒 Replace with your own secret
        public string HashPassword(string password)
        {
            // Combine password + fixed salt
            var combined = password + FixedSalt;
            using (var sha256 = SHA256.Create())
            {
                var bytes = Encoding.UTF8.GetBytes(combined);
                var hash = sha256.ComputeHash(bytes);

                // Convert to hex string
                var sb = new StringBuilder(hash.Length * 2);
                foreach (var b in hash)
                    sb.Append(b.ToString("x2"));
                return sb.ToString();
            }
        }
        public PasswordVerificationResult VerifyHashedPassword(string hashedPassword, string providedPassword)
        {
            var hashOfInput = HashPassword(providedPassword);
            return hashOfInput == hashedPassword
                ? PasswordVerificationResult.Success
                : PasswordVerificationResult.Failed;
        }
    }
}