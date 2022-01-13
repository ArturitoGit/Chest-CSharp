using System.Collections.Generic;
using System.Threading.Tasks;
using Chest.Core.Infrastructure;
using Core.Domain.Accounts.Services;
using Core.Domain.Crypto.Services;
using Core.Domain.PasswordHash.Services;
using Core.Domain.Session.Services;
using static Chest.Core.DependencyInjection.Service;

namespace Core.Domain.Accounts.Pipelines
{
    public class RegisterAccount
    {
        public record Request(
            string GlobalPassword,
            string Name,
            string AccountClearPassword,
            string? Link,
            string? Username) : IRequest<Result> { }
        public record Result(bool Success, string[] Errors) : IResult { }

        public class Handler : IRequestHandler<Request, Result>
        {
            private readonly ICryptoAgent _cryptoAgent;
            private readonly IAccountProvider _accountProvider ;
            private readonly IPasswordHashProvider _passwordProvider ;
            private readonly IPasswordChecker _passwordChecker ;
            private readonly ICollection<IValidator<ChestAccount>> _accountValidators;
            private readonly ICollection<IValidator<Request>> _requestValidators;

            public Handler(
                ICryptoAgent cryptoAgent,
                IAccountProvider accountProvider,
                IPasswordHashProvider passwordProvider,
                IPasswordChecker passwordChecker,
                ICollection<IValidator<ChestAccount>> accountValidators,
                ICollection<IValidator<Request>> requestValidators)
            {
                _cryptoAgent = cryptoAgent ?? throw new System.ArgumentNullException(nameof(cryptoAgent));
                _accountProvider = accountProvider ?? throw new System.ArgumentNullException(nameof(accountProvider));
                _passwordProvider = passwordProvider ?? throw new System.ArgumentNullException(nameof(passwordProvider));
                _passwordChecker = passwordChecker ?? throw new System.ArgumentException(nameof(passwordChecker)) ;
                _accountValidators = accountValidators ?? throw new System.ArgumentNullException(nameof(accountValidators));
                _requestValidators = requestValidators ?? throw new System.ArgumentNullException(nameof(requestValidators));
            }



            public async Task<Result> Handle(Request request)
            {
                // Check if the global password is correct
                var isPasswordCorrect = await _passwordChecker.IsPasswordCorrect(request.GlobalPassword, _passwordProvider, _cryptoAgent) ;
                if (!isPasswordCorrect) return new Result(false, new string[] { "Wrong password" }) ;

                // Check that the request parameters are corrects
                var (isRequestValid, requestErrors) = Validator.Validate(_requestValidators, request);
                if (!isRequestValid) return new Result(false, requestErrors);

                // Get a random salt
                var salt = _cryptoAgent.GenerateSalt();

                // Use the generated salt to encrypt the password
                var (encrypted_password, iv) = _cryptoAgent.Encrypt(
                    request.AccountClearPassword,
                    request.GlobalPassword,
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