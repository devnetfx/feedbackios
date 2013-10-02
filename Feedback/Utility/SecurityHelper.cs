using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace FeedbackIOS.Utility
{
    public static class SecurityHelper
    {
        public static string EncryptString(string loginId, string password, string emailAddress)
        {
            try
            {
                using (Aes aes = new AesManaged())
                {
                    var deriveBytes = new Rfc2898DeriveBytes(password, Encoding.UTF8.GetBytes(emailAddress), 1000);
                    aes.Key = deriveBytes.GetBytes(128/8);
                    aes.IV = aes.Key;

                    using (var encryptionStream = new MemoryStream())
                    {
                        using (
                            var encrypt = new CryptoStream(encryptionStream, aes.CreateEncryptor(),
                                                           CryptoStreamMode.Write))
                        {
                            var utfD1 = Encoding.UTF8.GetBytes(loginId);
                            encrypt.Write(utfD1, 0, utfD1.Length);
                            encrypt.FlushFinalBlock();
                        }
                        return Convert.ToBase64String(encryptionStream.ToArray());
                    }
                }
            }
            catch
            {
                return string.Empty;
            }
        }
    }
}