using System.Threading.Tasks;
using Chest.Core.DependencyInjection;
using Core.Domain.Accounts;
using Core.Domain.Accounts.Services;
using static Chest.Core.DependencyInjection.Service;

namespace Core.Domain.Session.Pipelines
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