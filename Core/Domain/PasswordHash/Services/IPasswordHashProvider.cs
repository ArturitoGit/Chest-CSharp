using System.Threading.Tasks;

namespace Core.Domain.PasswordHash.Services
{
    public interface IPasswordHashProvider
    {
        Task<string> GetPasswordHash();
        Task SetPasswordHash(string hash);
        Task ResetPasswordHash() ;
    }
}