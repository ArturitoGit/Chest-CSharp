using System.Threading.Tasks;
using Core.Domain.Crypto.Services;

namespace Core.Domain.PasswordHash.Services
{
    public interface IPasswordChecker
    {
        Task<bool> IsPasswordCorrect (string password, IPasswordHashProvider passwordProvider, ICryptoAgent cryptoAgent) ;
    }
}