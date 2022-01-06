using System.Threading.Tasks;
using UI.Domain.Crypto.Services;
using UI.Domain.PasswordHash.Services;

namespace UI.Domain.PasswordHash
{
    public class PasswordChecker : IPasswordChecker
    {
        private readonly IPasswordHashProvider _passwordHashProvider;
        private readonly ICryptoAgent _cryptoAgent;

        public PasswordChecker (IPasswordHashProvider passwordHashProvider, ICryptoAgent cryptoAgent)
        {
            _passwordHashProvider = passwordHashProvider ?? throw new System.ArgumentNullException(nameof(passwordHashProvider));
            _cryptoAgent = cryptoAgent ?? throw new System.ArgumentNullException(nameof(cryptoAgent));
        }

        public async Task<bool> IsPasswordCorrect(string password)
        {
                if (string.IsNullOrEmpty(password)) return false ;

                // Get the stored Hash
                var storedHash = await _passwordHashProvider.GetPasswordHash();

                // Use it to check the parameter
                bool result = _cryptoAgent.CheckHash(password, storedHash);

                return result;
        }
    }
}