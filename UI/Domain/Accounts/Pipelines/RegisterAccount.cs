using System.Collections.Generic;
using System.Threading.Tasks;
using UI.DependencyInjection;
using UI.Infrastructure;
using UI.Domain.Accounts.Services;
using UI.Domain.Crypto.Services;
using UI.Domain.Session.Services;
using static UI.DependencyInjection.Service;

namespace UI.Domain.Accounts.Pipelines
{
    public class RegisterAccount
    {
        public record Request(
            string Name,
            string AccountClearPassword,
            string? Link,
            string? Username) : IRequest<Result> { }
        public record Result(bool Success, string[] Errors) : IResult { }

        public class Handler : IRequestHandler<Request, Result>
        {
            private readonly ICryptoAgent _cryptoAgent;
            private readonly IAccountProvider _accountProvider;
            private readonly IChestSessionProvider _sessionProvider;
            private readonly ICollection<IValidator<ChestAccount>> _accountValidators;
            private readonly ICollection<IValidator<Request>> _requestValidators;

            public Handler(
                ICryptoAgent cryptoAgent,
                IAccountProvider accountProvider,
                IChestSessionProvider sessionProvider,
                ICollection<IValidator<ChestAccount>> accountValidators,
                ICollection<IValidator<Request>> requestValidators)
            {
                _cryptoAgent = cryptoAgent ?? throw new System.ArgumentNullException(nameof(cryptoAgent));
                _accountProvider = accountProvider ?? throw new System.ArgumentNullException(nameof(accountProvider));
                _sessionProvider = sessionProvider ?? throw new System.ArgumentNullException(nameof(sessionProvider));
                _accountValidators = accountValidators ?? throw new System.ArgumentNullException(nameof(accountValidators));
                _requestValidators = requestValidators ?? throw new System.ArgumentNullException(nameof(requestValidators));
            }



            public async Task<Result> Handle(Request request)
            {
                // Check that the request parameters are corrects
                var (isRequestValid, requestErrors) = Validator.Validate(_requestValidators, request);
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

                // Build the account object
                var account = new ChestAccount(
                    encrypted_password,
                    request.Name,
                    salt,
                    iv,
                    request.Link,
                    request.Username
                );

                // Use the validators
                var validatorsResult = Validator.Validate(_accountValidators, account);
                if (!validatorsResult.IsValid) return new Result(false, validatorsResult.Errors);

                // Register the Account in the database
                await _accountProvider.AddAccount(account);

                return new Result(true, new string[] { });
            }
        }

        public class RegisterAccountNameNotNull : IValidator<Request>
        {
            public (bool IsValid, string Error) validate(Request target)
            {
                if (string.IsNullOrEmpty(target.Name)) return (false, "Name of the requested account must not be null ...");
                return (true, string.Empty);
            }
        }

        public class RegisterAccountPasswordNotNull : IValidator<Request>
        {
            public (bool IsValid, string Error) validate(Request target)
            {
                if (string.IsNullOrEmpty(target.AccountClearPassword)) return (false, "Password of the requested account must not be null ...");
                return (true, string.Empty);
            }
        }
    }
}