using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using Notice.Data.Core;
using System.Security.Cryptography;
using System.IO;
using Notice.Data.Core;

namespace TestConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            Hash.HashSalt hasSalt = Hash.GenerateSHA256Hash(32, "P@ssw0rd");

            Console.WriteLine(hasSalt.Hash);
            Console.WriteLine(hasSalt.Salt);

            var saltBytes = Convert.FromBase64String(hasSalt.Salt);
            var rfc2898DeriveBytes = new Rfc2898DeriveBytes("P@ssw0rd", saltBytes, 10000);

            string hash = SHA256WithSalt("P@ssw0rd", Hash.GetSalt(32));

            Console.WriteLine(hash);
        }

        static string SHA256Hash(string data)
        {
            SHA256 sha = SHA256.Create();
            byte[] hash = sha.ComputeHash(Encoding.ASCII.GetBytes(data));
            StringBuilder sb = new StringBuilder();
            foreach(byte b in hash)
            {
                sb.AppendFormat("{0:x2}", b);
            }
            return sb.ToString();
        }

        static string SHA256WithSalt(string plainText, byte[] salt)
        {
            byte[] plainTextBytes = Encoding.UTF8.GetBytes(plainText);

            byte[] plainTextWithSaltBytes = new byte[plainTextBytes.Length + salt.Length];

            for (int i = 0; i < plainTextBytes.Length; i++)
                plainTextWithSaltBytes[i] = plainTextBytes[i];

            for (int i = 0; i < salt.Length; i++)
                plainTextWithSaltBytes[plainTextBytes.Length + i] = salt[i];

            SHA256Managed hash = new SHA256Managed();

            // Compute hash value of our plain text with appended salt.
            byte[] hashBytes = hash.ComputeHash(plainTextWithSaltBytes);

            // Create array which will hold hash and original salt bytes.
            byte[] hashWithSaltBytes = new byte[hashBytes.Length + salt.Length];

            // Copy hash bytes into resulting array.
            for (int i = 0; i < hashBytes.Length; i++)
                hashWithSaltBytes[i] = hashBytes[i];

            // Append salt bytes to the result.
            for (int i = 0; i < salt.Length; i++)
                hashWithSaltBytes[hashBytes.Length + i] = salt[i];

            // Convert result into a base64-encoded string.
            string hashValue = Convert.ToBase64String(hashWithSaltBytes);

            // Return the result.
            return hashValue;
        }

        static string getSalt()
        {
            var random = new RNGCryptoServiceProvider();

            // Maximum length of salt
            int max_length = 32;

            // Empty salt array
            byte[] salt = new byte[max_length];

            // Build the random bytes
            random.GetNonZeroBytes(salt);

            // Return the string encoded salt
            return Convert.ToBase64String(salt);
        }

        static byte[][] GetHashKeys(string key)
        {
            byte[][] result = new byte[2][];
            Encoding enc = Encoding.UTF8;

            SHA256 sha2 = new SHA256CryptoServiceProvider();

            byte[] rawKey = enc.GetBytes(key);
            byte[] rawIV = enc.GetBytes(key);

            byte[] hashKey = sha2.ComputeHash(rawKey);
            byte[] hashIV = sha2.ComputeHash(rawIV);

            Array.Resize(ref hashIV, 16);

            result[0] = hashKey;
            result[1] = hashIV;

            return result;
        }
        
    }
}
