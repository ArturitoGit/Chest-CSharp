using System.Threading.Tasks;
using Core.Domain.PasswordHash;
using Core.Domain.PasswordHash.Services;

namespace Tests.SharedKernel
{
    public class FakePasswordHashProvider : IPasswordHashProvider
    {
        private string _hash ;

        public Task<string> GetPasswordHash() => Task.FromResult(_hash ?? throw new NoPasswordStoredException ()) ;

        public Task ResetPasswordHash()
        {
            _hash = null ;
            return Task.CompletedTask ;
        }

        public Task SetPasswordHash(string hash)
        {
            _hash = hash ;
            return Task.CompletedTask ;
        }

        public static FakePasswordHashProvider INSTANCE = new FakePasswordHashProvider () ;
    }
}