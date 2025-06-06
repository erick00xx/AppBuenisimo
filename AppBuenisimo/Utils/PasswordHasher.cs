using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Web;

namespace AppBuenisimo.Utils
{
    public static class PasswordHasher
    {
        public static string HashPassword(string password)
        {
            // Usamos PBKDF2 con SHA256
            using (var rng = new RNGCryptoServiceProvider())
            using (var deriveBytes = new Rfc2898DeriveBytes(password, 16, 10000, HashAlgorithmName.SHA256))
            {
                byte[] salt = deriveBytes.Salt;
                byte[] hash = deriveBytes.GetBytes(32);

                byte[] result = new byte[48]; // 16 salt + 32 hash
                Buffer.BlockCopy(salt, 0, result, 0, 16);
                Buffer.BlockCopy(hash, 0, result, 16, 32);

                return Convert.ToBase64String(result);
            }
        }

        public static bool VerifyPassword(string enteredPassword, string storedHash)
        {
            byte[] hashBytes = Convert.FromBase64String(storedHash);
            byte[] salt = new byte[16];
            Buffer.BlockCopy(hashBytes, 0, salt, 0, 16);

            using (var deriveBytes = new Rfc2898DeriveBytes(enteredPassword, salt, 10000, HashAlgorithmName.SHA256))
            {
                byte[] newKey = deriveBytes.GetBytes(32);
                for (int i = 0; i < 32; i++)
                {
                    if (hashBytes[i + 16] != newKey[i])
                        return false;
                }
            }
            return true;
        }
    }
}