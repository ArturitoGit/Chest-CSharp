using System.Threading.Tasks;
using Core.Domain.Crypto.Services;
using Core.Domain.PasswordHash.Pipelines;
using Core.Domain.PasswordHash.Services;
using Core.Domain.Session.Services;
using static Chest.Core.DependencyInjection.Service;

namespace Core.Domain.Session.Pipelines
{
    public class OpenChestSession
    {
        public record Request (string Password) : IRequest<Result> {}
        public record Result (bool Success) : IResult {}

        public class Handler : IRequestHandler<Request, Result>
        {
            private readonly IChestSessionProvider _sessionProvider;
            private readonly ICryptoAgent _cryptoAgent;
            private readonly IPasswordHashProvider _passwordProvider;
            private readonly IPasswordChecker _passwordChecker;

            public Handler (
                IChestSessionProvider sessionProvider,
                ICryptoAgent cryptoAgent,
                IPasswordHashProvider passwordProvider,
                IPasswordChecker passwordChecker)
            {
                _sessionProvider = sessionProvider ?? throw new System.ArgumentNullException(nameof(_sessionProvider));
                _cryptoAgent = cryptoAgent ?? throw new System.ArgumentNullException(nameof(cryptoAgent));
                _passwordProvider = passwordProvider ?? throw new System.ArgumentNullException(nameof(passwordProvider));
                _passwordChecker = passwordChecker ?? throw new System.ArgumentNullException(nameof(passwordChecker));
            }

            public async Task<Result> Handle(Request request)
            {
                // Check if the given password is correct
                var validPassword = await _passwordChecker.IsPasswordCorrect(request.Password) ;

                // If the password is not valid
                if (!validPassword) return new Result (Success : false) ;

                // Open the session and save the password inside
                var session = _sessionProvider.GetSession() ;
                session.IsOpen = true ;
                session.Password = request.Password ;
                return new Result(Success : true) ;
            }
        }
    }
}