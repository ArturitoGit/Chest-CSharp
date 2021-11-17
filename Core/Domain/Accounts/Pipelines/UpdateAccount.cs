using System;
using System.Linq;
using System.Threading.Tasks;
using Chest.Core.DependencyInjection;
using Chest.Core.Infrastructure;
using Core.Domain.Accounts.Services;
using Core.Domain.Crypto.Services;
using Core.Domain.Session.Services;
using static Chest.Core.DependencyInjection.Service;

namespace Core.Domain.Accounts.Pipelines
{
    public class UpdateAccount
    {
        public record Request(Guid Id, string Name, string AccountClearPassword) : IRequest<Result> { }
        public record Result(bool Success, string[] Errors) : IResult { }

        public class Handler : IRequestHandler<Request, Result>
        {
            private readonly IAccountProvider _accountProvider;
            private readonly IChestSessionProvider _sessionProvider;
            private readonly IValidator<RegisterAccount.Request>[] _requestValidators;
            private readonly IValidator<ChestAccount>[] _accountValidators;
            private readonly ICryptoAgent _cryptoAgent;

            public Handler(
                IAccountProvider accountProvider,
                IChestSessionProvider sessionProvider,
                IValidator<RegisterAccount.Request>[] requestValidators,
                IValidator<ChestAccount>[] accountValidators,
                ICryptoAgent cryptoAgent)
            {
                _accountProvider = accountProvider ?? throw new ArgumentNullException(nameof(accountProvider));
                _sessionProvider = sessionProvider ?? throw new ArgumentNullException(nameof(sessionProvider));
                _requestValidators = requestValidators ?? throw new ArgumentNullException(nameof(requestValidators));
                _accountValidators = accountValidators ?? throw new ArgumentNullException(nameof(accountValidators));
                _cryptoAgent = cryptoAgent ?? throw new ArgumentNullException(nameof(cryptoAgent));
            }

            public async Task<Result> Handle(Request request)
            {
                // Use the registerAccount validators to validate the request
                var newAccountRequest = new RegisterAccount.Request(
                    request.Name,
                    request.AccountClearPassword
                );
                var (isRequestValid, requestErrors) = Validator.Validate(_requestValidators, newAccountRequest);
                if (!isRequestValid) return new Result(false, requestErrors);

                // Get a random salt
                var salt = _cryptoAgent.GenerateSalt();

                // Get the session password
                var session = _sessionProvider.GetSession() ;
                if (!session.IsOpen) return new Result( false, new string[] { "Chest closed" } ) ;

                // Use the generated salt to encrypt the password
                var (encrypted_password, iv) = _cryptoAgent.Encrypt(
                    request.AccountClearPassword,
                    session.Password!,
                    salt
                );

                // Pull the object from the db
                var account = (await _accountProvider.GetAccounts())
                    .Where(a => a.Id == request.Id)
                    .First();

                // Update the fields
                account.Name = request.Name;
                account.HashedPassword = encrypted_password;
                account.IV = iv;
                account.Salt = salt;

                // Use the accountvalidators
                var validatorsResult = Validator.Validate<ChestAccount>(_accountValidators, account);
                if (!validatorsResult.IsValid) return new Result(false, validatorsResult.Errors);

                // Update the account in the database
                await _accountProvider.UpdateAccount(account);

                return new Result(true, new string[] { });
            }
        }
    }
}