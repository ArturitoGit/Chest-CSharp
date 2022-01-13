using System;
using System.Threading.Tasks;
using Core.Domain.Accounts.Services;
using Core.Domain.Crypto.Services;
using Core.Domain.PasswordHash.Services;
using static Chest.Core.DependencyInjection.Service;

namespace Core.Domain.Accounts.Pipelines
{
    public class DeleteAccount
    {
        public record Request (string GlobalPassword, ChestAccount account) : IRequest<Result> {}
        public record Result (bool Success) : IResult {} 

        public class Handler : IRequestHandler<Request, Result>
        {

            private readonly IAccountProvider _accountProvider ;
            private readonly ICryptoAgent _cryptoAgent;
            private readonly IPasswordHashProvider _passwordHashProvider ;
            private readonly IPasswordChecker _passwordChecker ;

            public Handler (
                IAccountProvider accountProvider,
                ICryptoAgent cryptoAgent,
                IPasswordHashProvider passwordHashProvider,
                IPasswordChecker passwordChecker) 
            {
                _accountProvider = accountProvider ?? throw new ArgumentNullException(nameof(accountProvider));
                _cryptoAgent = cryptoAgent ?? throw new ArgumentNullException(nameof(cryptoAgent));
                _passwordHashProvider = passwordHashProvider ?? throw new ArgumentNullException(nameof(passwordHashProvider));
                _passwordChecker = passwordChecker ?? throw new ArgumentNullException(nameof(passwordChecker));
            }

            public async Task<Result> Handle (Request request)
            {
                // Check the password
                var isPasswordCorrect = await _passwordChecker.IsPasswordCorrect(request.GlobalPassword, _passwordHashProvider, _cryptoAgent) ;
                if (!isPasswordCorrect) return new Result(false) ;

                // If the password is valid
                await _accountProvider.DeleteAccount(request.account.Id) ;
                return new Result(true) ;
            }

        }
    }
}