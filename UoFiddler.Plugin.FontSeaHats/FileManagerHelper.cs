// /***************************************************************************
//  *
//  * $Author: Turley
//  * 
//  * "THE BEER-WARE LICENSE"
//  * As long as you retain this notice you can do whatever you want with 
//  * this stuff. If we meet some day, and you think this stuff is worth it,
//  * you can buy me a beer in return.
//  *
//  ***************************************************************************/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace UoFiddler.Plugin.FontSeaHats
{
    public static class FileManagerHelper
    {
        public static string DecryptFile(string inputFile, string password)
        {
            using (var fileStream = new FileStream(inputFile, FileMode.Open))
            {
                byte[] salt = new byte[16];
                fileStream.Read(salt, 0, salt.Length); // Legge il sale all'inizio del file

                DeriveKeyAndIV(password, salt, out byte[] key, out byte[] iv);

                using (var aes = Aes.Create())
                {
                    aes.Key = key;
                    aes.IV = iv;
                    aes.Mode = CipherMode.CBC;
                    aes.Padding = PaddingMode.PKCS7;

                    using (var cryptoStream = new CryptoStream(fileStream, aes.CreateDecryptor(), CryptoStreamMode.Read))
                    {
                        using (var memoryStream = new MemoryStream())
                        {
                            try
                            {
                                cryptoStream.CopyTo(memoryStream);
                                byte[] decryptedBytes = memoryStream.ToArray();
                                return Encoding.UTF8.GetString(decryptedBytes); // Converte i byte in stringa
                            }
                            catch
                            {
                                return "";
                            }
                        }
                    }
                }
            }
        }

        // Deriva una chiave e un IV da una password usando PBKDF2
        private static void DeriveKeyAndIV(string password, byte[] salt, out byte[] key, out byte[] iv)
        {
            using (var keyDerivationFunction = new Rfc2898DeriveBytes(password, salt, 100000))
            {
                key = keyDerivationFunction.GetBytes(32); // 256 bit key
                iv = keyDerivationFunction.GetBytes(16); // 128 bit IV
            }
        }

        // Genera un array di byte casuali
        private static byte[] GenerateRandomBytes(int length)
        {
            byte[] randomBytes = new byte[length];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomBytes);
            }
            return randomBytes;
        }

        public static void EncryptFile(string inputFile, string outputFile, string password)
        {
            byte[] salt = GenerateRandomBytes(16);

            DeriveKeyAndIV(password, salt, out byte[] key, out byte[] iv);

            using (var aes = Aes.Create())
            {
                aes.Key = key;
                aes.IV = iv;
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;

                using (var fileStream = new FileStream(outputFile, FileMode.Create))
                {
                    // Scrive il sale all'inizio del file
                    fileStream.Write(salt, 0, salt.Length);

                    using (var cryptoStream = new CryptoStream(fileStream, aes.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        using (var inputFileStream = new FileStream(inputFile, FileMode.Open))
                        {
                            inputFileStream.CopyTo(cryptoStream);
                        }
                    }
                }
            }
        }



    }
}
