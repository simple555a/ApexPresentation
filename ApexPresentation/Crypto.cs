using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;

namespace ApexPresentation
{
    class Crypto
    {

        public static String EncryptString(String in_string)
        {
            String return_value;

            using (TripleDESCryptoServiceProvider tdsAlg = new TripleDESCryptoServiceProvider())
            {
                tdsAlg.GenerateKey();
                tdsAlg.GenerateIV();

                // Create a decrytor to perform the stream transform.
                ICryptoTransform encryptor = tdsAlg.CreateEncryptor(tdsAlg.Key, tdsAlg.IV);

                // Create the streams used for encryption.
                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {

                            //Write all data to the stream.
                            swEncrypt.Write(in_string);
                        }
                        return_value = msEncrypt.ToString();
                    }
                }
            }
            return return_value;
        }

        public static String DecryptString(String in_string)
        {
            String return_value;
            using (TripleDESCryptoServiceProvider tdsAlg = new TripleDESCryptoServiceProvider())
            {
                tdsAlg.GenerateKey();
                tdsAlg.GenerateIV();

                // Create a decrytor to perform the stream transform.
                ICryptoTransform decryptor = tdsAlg.CreateDecryptor(tdsAlg.Key, tdsAlg.IV);

                // Create the streams used for decryption.
                using (MemoryStream msDecrypt = new MemoryStream(System.Text.Encoding.ASCII.GetBytes(in_string)))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {

                            // Read the decrypted bytes from the decrypting stream
                            // and place them in a string.
                            return_value = srDecrypt.ReadToEnd();
                        }
                    }
                }
            }

            return return_value;

        }
    }
}
