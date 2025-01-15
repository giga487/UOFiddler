using System.Security.Cryptography;
using System.Text;

namespace SeaHatsExternal.Crypto
{
    public static class SeaHatsFileManager
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

        // Cripta una stringa e salva in un file
        public static bool EncryptStringToFile(string plaintext, string filePath, string password)
        {
            byte[] salt = GenerateRandomBytes(16);

            DeriveKeyAndIV(password, salt, out byte[] key, out byte[] iv);

            using (var aes = Aes.Create())
            {
                aes.Key = key;
                aes.IV = iv;
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    // Scrive il sale all'inizio del file
                    fileStream.Write(salt, 0, salt.Length);

                    using (var cryptoStream = new CryptoStream(fileStream, aes.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        byte[] plaintextBytes = Encoding.UTF8.GetBytes(plaintext);
                        cryptoStream.Write(plaintextBytes, 0, plaintextBytes.Length);
                        return true;
                    }
                }
            }
        }
        // Cripta una stringa
        public static string EncryptString(string plaintext, string password)
        {
            byte[] salt = GenerateRandomBytes(16);

            DeriveKeyAndIV(password, salt, out byte[] key, out byte[] iv);

            using (var aes = Aes.Create())
            {
                aes.Key = key;
                aes.IV = iv;
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;

                using (var memoryStream = new MemoryStream())
                {
                    // Scrive il sale all'inizio del flusso
                    memoryStream.Write(salt, 0, salt.Length);

                    using (var cryptoStream = new CryptoStream(memoryStream, aes.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        byte[] plaintextBytes = Encoding.UTF8.GetBytes(plaintext);
                        cryptoStream.Write(plaintextBytes, 0, plaintextBytes.Length);
                    }

                    byte[] encryptedBytes = memoryStream.ToArray();
                    return Convert.ToBase64String(encryptedBytes); // Converte in Base64 per una facile gestione
                }
            }
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
