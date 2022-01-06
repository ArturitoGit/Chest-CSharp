using System.Threading.Tasks;
using UI.DependencyInjection;
using UI.Domain.Accounts;
using UI.Domain.Accounts.Services;
using static UI.DependencyInjection.Service;

namespace UI.Domain.Session.Pipelines
{
    public class InitMenu
    {
        public record Request() : IRequest<Result> { }

        public record Result(Accounts.ChestAccount[] Accounts) : IResult { }

        public class Handler : IRequestHandler<Request, Result>
        {
            private readonly IAccountProvider _accountProvider;

            public Handler(IAccountProvider accountProvider)
            {
                _accountProvider = accountProvider ?? throw new System.ArgumentNullException(nameof(accountProvider));
            }

            public async Task<Result> Handle(Request request)
            {
                // Pull the accounts
                var accounts = await _accountProvider.GetAccounts();
                return new Result(
                    accounts
                );
            }
        }
    }
}