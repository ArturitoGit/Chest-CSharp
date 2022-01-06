using System.Threading.Tasks;

namespace UI.Domain.PasswordHash.Services
{
    public interface IPasswordChecker
    {
        Task<bool> IsPasswordCorrect (string password) ;
    }
}