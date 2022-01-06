namespace UI.Domain.Crypto.Services
{
    public interface ICryptoAgent
    {
        /**
         * Generate a random byte[8] salt to save a with a password.
         */
        byte[] GenerateSalt();

        string GetHash(string input);
        bool CheckHash(string input, string hash);

        /**
         * Use the given password and salt values to generate a key and use it to encrypt the given plain text with AES
         *      plain       : The string value to encrypt 
         *      password    : The string value of the password to encrypt the plain with
         *      salt        : The byte[] value of the salt associated with the password 
         * Returns :
         *      Cipher  : The encrypted content
         *      IV      : The random IV that has been created to encrypt the plain text
         */
        (byte[] Cipher, byte[] IV) Encrypt(string plain, string password, byte[] salt);

        /**
         * Use the given password and salt values to generate a key and use it to decrypt the given cipher text with AES
         *      cipher      : The byte[] value to decrypt 
         *      IV          : The byte[] value of the IV the cihper has been encrypted with 
         *      password    : The string value of the password to decrypt the cipher with
         *      salt        : The byte[] value of the salt associated with the password 
         * Returns :
         *      Plain : The plain text that contains the decryption of the given parameters
         */
        string Decrypt(byte[] cipher, byte[] IV, string password, byte[] salt);
    }

}