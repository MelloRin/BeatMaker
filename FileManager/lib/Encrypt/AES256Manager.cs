using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace MelloRin.FileManager.lib.Encrypt
{
    public class AES256Manager
    {
        public static string decrypt(string input, string key)
        {
            RijndaelManaged aes = new RijndaelManaged
            {
                KeySize = 256,
                BlockSize = 128,
                Mode = CipherMode.CBC,
                Padding = PaddingMode.PKCS7,
                Key = Encoding.UTF8.GetBytes(key),
                IV = new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }
            };

            var decrypt = aes.CreateDecryptor();
            byte[] xBuff = null;

            using (var ms = new MemoryStream())
            {
                using (var cs = new CryptoStream(ms, decrypt, CryptoStreamMode.Write))
                {
                    byte[] xXml = Convert.FromBase64String(input);
                    cs.Write(xXml, 0, xXml.Length);
                }
                xBuff = ms.ToArray();
            }

            return Encoding.UTF8.GetString(xBuff);
        }

        public static string encrypt(string Input, string key)
        {
            return encrypt(Encoding.UTF8.GetBytes(Input), key);
        }

        public static string encrypt(byte[] input, string key)
        {
            RijndaelManaged aes = new RijndaelManaged
            {
                KeySize = 256,
                BlockSize = 128,
                Mode = CipherMode.CBC,
                Padding = PaddingMode.PKCS7,
                Key = Encoding.UTF8.GetBytes(key),
                IV = new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }
            };

            var encrypt = aes.CreateEncryptor(aes.Key, aes.IV);

            byte[] xBuff = null;
            using (var ms = new MemoryStream())
            {
                using (var cs = new CryptoStream(ms, encrypt, CryptoStreamMode.Write))
                {
                    cs.Write(input, 0, input.Length);
                }

                xBuff = ms.ToArray();
            }

            return Convert.ToBase64String(xBuff);
        }
    }
}