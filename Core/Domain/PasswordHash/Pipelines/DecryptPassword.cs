using System.Threading.Tasks;
using Core.Domain.Accounts ;
using Core.Domain.Crypto.Services;
using Core.Domain.PasswordHash.Services;
using static Chest.Core.DependencyInjection.Service;

namespace Core.Domain.PasswordHash.Pipelines
{
    public class DecryptPassword
    {
        public record Request(string GlobalPassword, ChestAccount Account) : IRequest<Result> { }
        public record Result(bool Success, string? ClearPassword) : IResult { }

        public class Handler : IRequestHandler<Request, Result>
        {
            private readonly ICryptoAgent _cryptoAgent;
            private readonly IPasswordHashProvider _pwdProvider ;
            private readonly IPasswordChecker _pwdChecker ;

            public Handler(ICryptoAgent cryptoAgent, IPasswordHashProvider pwdProvider, IPasswordChecker pwdChecker)
            {
                _cryptoAgent = cryptoAgent ?? throw new System.ArgumentNullException(nameof(cryptoAgent));
                _pwdProvider = pwdProvider ?? throw new System.ArgumentNullException(nameof(pwdProvider)) ;
                _pwdChecker = pwdChecker ?? throw new System.ArgumentNullException(nameof(pwdChecker)) ;
            }

            public async Task<Result> Handle(Request request)
            {
                // Check the password
                var isPasswordCorrect = await _pwdChecker.IsPasswordCorrect(request.GlobalPassword, _pwdProvider, _cryptoAgent) ;
                if (!isPasswordCorrect) return new Result(false, null ) ;

                return new Result(
                    true,
                    _cryptoAgent.Decrypt(
                        request.Account.HashedPassword,
                        request.Account.IV,
                        request.GlobalPassword,
                        request.Account.Salt
                ));
            }
        }
    }
}