using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
namespace Console_CommonModel.CommonLib.LibAuth.AppAuth.AuthorizationServices.ServiceApi
{
    public class CryptoService(ServiceSertificate serviceSertificate, ServiceInfo serviceInfo)
    {

        //byte[] key =
        //                {
        //                    0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08,
        //                    0x09, 0x10, 0x11, 0x12, 0x13, 0x14, 0x15, 0x16
        //                };
        public string Encrypt(string message)
        {
            try
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    using (Aes aes = Aes.Create())
                    {

                        aes.Key = serviceSertificate.PrivateKey.ToArray();


                        byte[] iv = aes.IV;
                        ms.Write(iv, 0, iv.Length);

                        using (CryptoStream cryptoStream = new(
                            ms,
                            aes.CreateEncryptor(),
                            CryptoStreamMode.Write))
                        {
                            // By default, the StreamWriter uses UTF-8 encoding.
                            // To change the text encoding, pass the desired encoding as the second parameter.
                            // For example, new StreamWriter(cryptoStream, Encoding.Unicode).
                            using (StreamWriter encryptWriter = new(cryptoStream, Encoding.UTF8))
                            {
                                encryptWriter.WriteLine(message);
                                return Encoding.UTF8.GetString(ms.ToArray());
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Исключение при шифровании текста => {ex.Message}");
            }
        }

        public async Task<string> Decrypt(string message)
        {

            try
            {
                using (MemoryStream fileStream = new MemoryStream())
                {
                    using (Aes aes = Aes.Create())
                    {
                        byte[] iv = new byte[aes.IV.Length];
                        int numBytesToRead = aes.IV.Length;
                        int numBytesRead = 0;
                        while (numBytesToRead > 0)
                        {
                            int n = fileStream.Read(iv, numBytesRead, numBytesToRead);
                            if (n == 0) break;

                            numBytesRead += n;
                            numBytesToRead -= n;
                        }
                        var key = serviceSertificate.PrivateKey.ToArray();
                        using (CryptoStream cryptoStream = new(
                           fileStream,
                           aes.CreateDecryptor(key, iv),
                           CryptoStreamMode.Read))
                        {
                            using (StreamReader decryptReader = new(cryptoStream, Encoding.UTF8))
                            {
                                return await decryptReader.ReadToEndAsync();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Исключение при расшифровании текста => {ex.Message}");
            }
        }
    }
}
