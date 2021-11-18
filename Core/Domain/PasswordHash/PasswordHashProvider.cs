using System;
using System.IO;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using Core.Domain.PasswordHash.Services;

namespace Core.Domain.PasswordHash
{
    public class PasswordHashProvider : IPasswordHashProvider
    {

        private static string DEAULT_PASSWORD_FILE_PATH = Path.Combine(Environment.CurrentDirectory , "passwords.json" );
        private string _passwordFile = DEAULT_PASSWORD_FILE_PATH;

        public PasswordHashProvider() { }
        public PasswordHashProvider(string password_file)
        {
            _passwordFile = password_file;
        }

        public Task<string> GetPasswordHash()
        {
            try
            {
                // Read the password file
                return Task.FromResult(File.ReadAllText(_passwordFile)!);
            }
            catch (FileNotFoundException)
            {
                throw new NoPasswordStoredException("No password has been stored yet");
            }
        }

        public async Task SetPasswordHash(string hash)
        {

            using (StreamWriter file = new(_passwordFile, append: false))
            {
                await file.WriteAsync(hash);
            }

        }

        public void Dispose()
        {
            if (File.Exists(_passwordFile)) File.Delete(_passwordFile);
        }

        public Task ResetPasswordHash()
        {
            Dispose();
            return Task.CompletedTask ;
        }
    }

    [Serializable]
    public class NoPasswordStoredException : Exception
    {
        public NoPasswordStoredException()
        {
        }

        public NoPasswordStoredException(string? message) : base(message)
        {
        }

        public NoPasswordStoredException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected NoPasswordStoredException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}