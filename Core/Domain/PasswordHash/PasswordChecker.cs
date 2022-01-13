using System.Threading.Tasks;
using Core.Domain.Crypto.Services;
using Core.Domain.PasswordHash.Services;

namespace Core.Domain.PasswordHash
{
    public class PasswordChecker : IPasswordChecker
    {
        public async Task<bool> IsPasswordCorrect(string password, IPasswordHashProvider passwordHashProvider, ICryptoAgent cryptoAgent)
        {
                if (string.IsNullOrEmpty(password)) return false ;

                // Get the stored Hash
                var storedHash = await passwordHashProvider.GetPasswordHash();

                // Use it to check the parameter
                bool result = cryptoAgent.CheckHash(password, storedHash);

                return result;
        }
    }
}