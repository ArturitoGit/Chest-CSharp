using System.Threading.Tasks;
using UI.Domain.Accounts ;
using UI.Domain.Crypto.Services;
using UI.Domain.Session.Services;
using static UI.DependencyInjection.Service;

namespace UI.Domain.PasswordHash.Pipelines
{
    public class DecryptPassword
    {
        public record Request(ChestAccount Account) : IRequest<Result> { }
        public record Result(bool Success, string ClearPassword) : IResult { }

        public class Handler : IRequestHandler<Request, Result>
        {
            private readonly ICryptoAgent _cryptoAgent;
            private readonly IChestSessionProvider _sessionProvider;

            public Handler(ICryptoAgent cryptoAgent, IChestSessionProvider sessionProvider)
            {
                _cryptoAgent = cryptoAgent ?? throw new System.ArgumentNullException(nameof(cryptoAgent));
                _sessionProvider = sessionProvider ?? throw new System.ArgumentNullException(nameof(sessionProvider));
            }

            public Task<Result> Handle(Request request)
            {

                // Get the session password
                var session = _sessionProvider.GetSession() ;
                if (!session.IsOpen) return Task.FromResult(new Result( false, string.Empty )) ;

                return Task.FromResult(
                    new Result(
                        true,
                        _cryptoAgent.Decrypt(
                            request.Account.HashedPassword,
                            request.Account.IV,
                            session.Password!,
                            request.Account.Salt
                )));
            }
        }
    }
}