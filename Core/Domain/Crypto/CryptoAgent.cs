using System.IO;
using System.Security.Cryptography;
using Core.Domain.Crypto.Services;

namespace Core.Domain.Crypto
{
    public class CryptoAgent : ICryptoAgent
    {
        // The number of iterations to make in order to generate the key from the password and the salt
        private const int NB_ITERATIONS = 1500;

        public byte[] GenerateSalt()
        {
            byte[] salt = new byte[8];
            using (RNGCryptoServiceProvider rngCsp = new RNGCryptoServiceProvider())
            {
                rngCsp.GetBytes(salt);
            }
            return salt;

        }
        public string Decrypt(byte[] cipher, byte[] IV, string password, byte[] salt)
        {
            using (RNGCryptoServiceProvider rngCsp = new RNGCryptoServiceProvider())
            {
                // Generate the key
                Rfc2898DeriveBytes k2 = new Rfc2898DeriveBytes(password, salt, NB_ITERATIONS);
                // Build the decryptor object
                Aes decAlg = Aes.Create();
                decAlg.Key = k2.GetBytes(16);
                decAlg.IV = IV;
                // Build the streams
                MemoryStream decryptionStreamBacking = new MemoryStream();
                CryptoStream decrypt = new CryptoStream(decryptionStreamBacking, decAlg.CreateDecryptor(), CryptoStreamMode.Write);
                // Fill the streams
                decrypt.Write(cipher, 0, cipher.Length);
                decrypt.Flush();
                decrypt.Close();
                // Reset the key
                k2.Reset();
                // Convert the result from bytes to string
                string plain = new System.Text.UTF8Encoding(false).GetString(decryptionStreamBacking.ToArray());
                return plain;
            }
        }

        public (byte[] Cipher, byte[] IV) Encrypt(string plain, string password, byte[] salt)
        {
            using (RNGCryptoServiceProvider rngCsp = new RNGCryptoServiceProvider())
            {
                // Generate the key
                Rfc2898DeriveBytes k1 = new Rfc2898DeriveBytes(password, salt, NB_ITERATIONS);
                // Build the encryptor object
                Aes encAlg = Aes.Create();
                encAlg.Key = k1.GetBytes(16);
                // Build the streams
                MemoryStream encryptionStream = new MemoryStream();
                CryptoStream encrypt = new CryptoStream(encryptionStream, encAlg.CreateEncryptor(), CryptoStreamMode.Write);
                // Convert the plain text from string to bytes
                byte[] utfD1 = new System.Text.UTF8Encoding(false).GetBytes(plain);
                // Fill the streams
                encrypt.Write(utfD1, 0, utfD1.Length);
                encrypt.FlushFinalBlock();
                encrypt.Close();
                byte[] cipher = encryptionStream.ToArray();
                // Reset the key
                k1.Reset();
                return (cipher, encAlg.IV);
            }
        }

        public string GetHash(string input) => BCrypt.Net.BCrypt.HashPassword(input);

        public bool CheckHash(string input, string hash) => BCrypt.Net.BCrypt.Verify(input, hash);
    }
}