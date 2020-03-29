using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace KeyManagment.Services
{

    public class AESKEY
    {
        /*the initkey is converted from string HR$2p/89()IÄHR# by utf8*/
        private static readonly byte[] Aes_InitKey = {72, 82, 36, 50,
                                                     112, 47, 56, 57,
                                                     40, 41, 73, 195,
                                                     132, 72, 82, 35};

        //want to define Aes_Key as static readonly, can't get it        
        private static byte[] Aes_GenKey { get; set; }

        public static void Set_AesKey(string aeskey)
        {
            byte[] tmpkey = new byte[32];            

            byte[] tmpbyte = System.Text.Encoding.UTF8.GetBytes(aeskey);
            for (int i = 0; i < aeskey.Length; i++)
            {
                tmpkey[i] = tmpbyte[i];
            }
            Aes_GenKey = tmpkey;
        }

        public static void Reset_AesKey()
        {
            Aes_GenKey = null;
        }

        public static string EncryptStringToBytes_Aes(string entrycode)
        {
            try
            {
                byte[] encrypted;
                using (Aes aesAlg = Aes.Create())
                {

                    aesAlg.Key = Aes_GenKey;
                    aesAlg.IV = Aes_InitKey;

                    // Create an encryptor to perform the stream transform.
                    ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                    // Create the streams used for encryption.
                    using (MemoryStream msEncrypt = new MemoryStream())
                    {
                        using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                        {
                            using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                            {
                                //Write all data to the stream.
                                swEncrypt.Write(entrycode);
                            }
                            encrypted = msEncrypt.ToArray();
                        }
                    }
                }
                // Return the encrypted string from the memory stream.            
                return System.Convert.ToBase64String(encrypted);
            }
            catch 
            {
                return "xx pw invalid xxxxx"; 
            }
        }

        public static string DecryptStringFromBytes_Aes(string cipherText)
        {
            byte[] decryptbytearray;
            string plaintext;
            try
            {
                decryptbytearray = System.Convert.FromBase64String(cipherText);
                using (Aes aesAlg = Aes.Create())
                {
                    aesAlg.Key = Aes_GenKey;
                    aesAlg.IV = Aes_InitKey;

                    // Create a decryptor to perform the stream transform.
                    ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                    // Create the streams used for decryption.
                    using (MemoryStream msDecrypt = new MemoryStream(decryptbytearray))
                    {
                        using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                        {
                            using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                            {

                                // Read the decrypted bytes from the decrypting stream
                                // and place them in a string.
                                plaintext = srDecrypt.ReadToEnd();
                            }
                        }
                    }

                }
            }
            catch
            {
                plaintext = "xx pw invalid xxxxx";
            }

            return plaintext;

        }

    }

}
