using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace Jewellery.Application.Common.Security
{
    public class PasswordSecurityHelper
    {
        private static readonly string Key = "12345678901234567890123456789012"; // 32 chars
        private static readonly string IV = "1234567890123456"; // 16 chars

        public string Encrypt(string plainText)
        {
            using Aes aes = Aes.Create();
            aes.Key = Encoding.UTF8.GetBytes(Key);
            aes.IV = Encoding.UTF8.GetBytes(IV);

            using var encryptor = aes.CreateEncryptor();
            byte[] input = Encoding.UTF8.GetBytes(plainText);
            byte[] encrypted = encryptor.TransformFinalBlock(input, 0, input.Length);

            return Convert.ToBase64String(encrypted);
        }

        public string Decrypt(string cipherText)
        {
            using Aes aes = Aes.Create();
            aes.Key = Encoding.UTF8.GetBytes(Key);
            aes.IV = Encoding.UTF8.GetBytes(IV);

            using var decryptor = aes.CreateDecryptor();
            byte[] input = Convert.FromBase64String(cipherText);
            byte[] decrypted = decryptor.TransformFinalBlock(input, 0, input.Length);

            return Encoding.UTF8.GetString(decrypted);
        }
    }
}
