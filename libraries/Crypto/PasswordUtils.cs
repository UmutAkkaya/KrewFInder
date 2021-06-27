using System;
using System.Security.Cryptography;

namespace Crypto
{
    public static class PasswordUtils
    {
        private const int SaltLength = 16;
        private const int PasswordHashLength = 64;
        private const int HashingIterations = 100;

        // The functions has to run a normal game of League (~20-40 min)
        public static byte[] GenerateRandomSalt()
        {
            byte[] salt = new byte[SaltLength];
            new RNGCryptoServiceProvider().GetBytes(salt);
            return salt;
        }

        public static byte[] HashPassword(String plainText, byte[] salt)
        {
            var pbkdf2 = new Rfc2898DeriveBytes(plainText, salt, HashingIterations);
            byte[] hash = pbkdf2.GetBytes(PasswordHashLength);
            byte[] saltedHash = new byte[PasswordHashLength + SaltLength];

            Array.Copy(salt, 0, saltedHash, 0, SaltLength);
            Array.Copy(hash, 0, saltedHash, SaltLength, PasswordHashLength);

            return saltedHash;
        }

        public static byte[] HashPassword(String plainText)
        {
            return HashPassword(plainText, GenerateRandomSalt());
        }

        public static bool ComparePassword(String plainText, byte[] saltedHash)
        {
            var salt = new byte[SaltLength];
            Array.Copy(saltedHash, 0, salt, 0, SaltLength);

            var providedPasswordHash = HashPassword(plainText, salt);

            for (int i = SaltLength; i < SaltLength + PasswordHashLength; i++)
            {
                if (providedPasswordHash[i] != saltedHash[i])
                    return false;
            }

            return true;
        }
    }
}
