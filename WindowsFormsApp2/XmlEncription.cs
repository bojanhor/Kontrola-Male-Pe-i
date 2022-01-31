using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace WindowsFormsApp2
{
    public class XmlEncription
    {
        private const string InitVector = "T=A,rAČ u4%z-d a";
        private const int KeySize = 256;
        private const int PasswordIterations = 2; //2;
        private const string SaltValue = "W&h59_Xe9P 2za-eFr2fa&@ras! a+uc@$%/&sd fasu=ćščapwe";
        private static readonly string passPhrase = "793qwegfz' q2394zf174a ĐŠŠ%/(&U7WHRBR4!# %&$5A86";

        public static string Decrypt(string encryptedText)
        {
            byte[] encryptedTextBytes = Convert.FromBase64String(encryptedText);
            byte[] initVectorBytes = Encoding.UTF8.GetBytes(InitVector);
            byte[] passwordBytes = Encoding.UTF8.GetBytes(passPhrase);
            string plainText;
            byte[] saltValueBytes = Encoding.UTF8.GetBytes(SaltValue);

            Rfc2898DeriveBytes password = new Rfc2898DeriveBytes(passwordBytes, saltValueBytes, PasswordIterations);
            byte[] keyBytes = password.GetBytes(KeySize / 8);

            RijndaelManaged rijndaelManaged = new RijndaelManaged { Mode = CipherMode.CBC };

            try
            {
                using (ICryptoTransform decryptor = rijndaelManaged.CreateDecryptor(keyBytes, initVectorBytes))
                {
                    using (MemoryStream memoryStream = new MemoryStream(encryptedTextBytes))
                    {
                        using (CryptoStream cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
                        {                            
                            byte[] plainTextBytes = new byte[encryptedTextBytes.Length];

                            int decryptedByteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length);
                            plainText = Encoding.UTF8.GetString(plainTextBytes, 0, decryptedByteCount);
                        }
                    }
                }
            }
            catch (CryptographicException ex)
            {
                plainText = string.Empty; // Assume the error is caused by an invalid password
                var message = "Xml file decription error: " + ex.Message;
                SysLog.SetMessage(message);
                throw new Exception(message);
            }

            return plainText;
        }

        public static string Encrypt(string plainText)
        {
            string encryptedText;
            byte[] initVectorBytes = Encoding.UTF8.GetBytes(InitVector);
            byte[] passwordBytes = Encoding.UTF8.GetBytes(passPhrase);
            byte[] plainTextBytes = Encoding.UTF8.GetBytes(plainText);
            byte[] saltValueBytes = Encoding.UTF8.GetBytes(SaltValue);

            Rfc2898DeriveBytes password = new Rfc2898DeriveBytes(passwordBytes, saltValueBytes, PasswordIterations);
            byte[] keyBytes = password.GetBytes(KeySize / 8);

            RijndaelManaged rijndaelManaged = new RijndaelManaged { Mode = CipherMode.CBC };

            using (ICryptoTransform encryptor = rijndaelManaged.CreateEncryptor(keyBytes, initVectorBytes))
            {
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    using (CryptoStream cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                    {
                        cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
                        cryptoStream.FlushFinalBlock();

                        byte[] cipherTextBytes = memoryStream.ToArray();
                        encryptedText = Convert.ToBase64String(cipherTextBytes);
                    }
                }
            }

            return encryptedText;
        }
        
        
    }
}