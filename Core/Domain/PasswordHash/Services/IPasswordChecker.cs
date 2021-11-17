using System.Threading.Tasks;

namespace Core.Domain.PasswordHash.Services
{
    public interface IPasswordChecker
    {
        Task<bool> IsPasswordCorrect (string password) ;
    }
}