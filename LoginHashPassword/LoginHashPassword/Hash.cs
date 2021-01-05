using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace LoginHashPassword
{
    public static class Hash
    {
        private const int HasingIterationsCount = 50000;
        private const int HashByteSize = 24;

        public static byte[] GenerateSalt()
        {
            const int saltLength = 32;

            using (RNGCryptoServiceProvider randomNumberGenerator = new RNGCryptoServiceProvider())
            {
                var randomNumber = new byte[saltLength];
                randomNumberGenerator.GetBytes(randomNumber);

                return randomNumber;
            }
            //byte[] b = new byte[0];
            //return b;
        }

        //private static byte[] Combine(byte[] first, byte[] second)
        //{
        //    byte[] ret = new byte[first.Length + second.Length];

        //    Buffer.BlockCopy(first, 0, ret, 0, first.Length);
        //    Buffer.BlockCopy(second, 0, ret, first.Length, second.Length);

        //    return ret;
        //}

        //public static byte[] HashPasswordWithSalt(byte[] toBeHashed, byte[] salt)
        //{
        //    using (SHA256 sha256 = SHA256.Create())
        //    {
        //        return sha256.ComputeHash(Combine(toBeHashed, salt));
        //    }
        //}
        internal static byte[] HashPasswordWithSalt(string password, byte[] salt, int iterations = HasingIterationsCount, int hashByteSize = HashByteSize)
        {
            Rfc2898DeriveBytes pbkdf2 = new Rfc2898DeriveBytes(password, salt, iterations);
            return pbkdf2.GetBytes(hashByteSize);
        }
    }
}
