using System.Threading;
using System.Threading.Tasks;
using Core.Domain.Crypto.Services;
using Core.Domain.PasswordHash.Services;
using static Chest.Core.DependencyInjection.Service ;

namespace Core.Domain.PasswordHash.Pipelines
{
    public class CheckPassword
    {
        public record Request (string Password) : IRequest<Result> {}
        public record Result (bool Success) : IResult {}

        public class Handler : IRequestHandler<Request,Result>
        {
            private IPasswordHashProvider _pwdProvider ;
            private IPasswordChecker _pwdChecker ;
            private ICryptoAgent _cryptoAgent ;

            public Handler (IPasswordHashProvider pwdProvider, IPasswordChecker pwdChecker, ICryptoAgent cryptoAgent)
            {
                _cryptoAgent = cryptoAgent ?? throw new System.Exception(nameof(cryptoAgent)) ;
                _pwdProvider = pwdProvider ?? throw new System.Exception(nameof(pwdProvider)) ;
                _pwdChecker = pwdChecker ?? throw new System.Exception(nameof(pwdChecker)) ;
            }
            public async Task<Result> Handle (Request request)
            {
                var result = await _pwdChecker.IsPasswordCorrect(
                    request.Password,
                    _pwdProvider,
                    _cryptoAgent
                ) ;
                return new Result (result) ;
            }
        }
    }
}