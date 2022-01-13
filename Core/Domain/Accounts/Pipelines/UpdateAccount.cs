using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Chest.Core.Infrastructure;
using Core.Domain.Accounts.Services;
using Core.Domain.Crypto.Services;
using Core.Domain.PasswordHash.Services;
using static Chest.Core.DependencyInjection.Service;

namespace Core.Domain.Accounts.Pipelines
{
    public class UpdateAccount
    {
        public record Request(
            String GlobalPassword,
            Guid Id,
            string Name,
            string AccountClearPassword,
            string? Link,
            string? Username) : IRequest<Result> { }
        public record Result(bool Success, string[] Errors) : IResult { }

        public class Handler : IRequestHandler<Request, Result>
        {
            private readonly IAccountProvider _accountProvider ;
            private readonly ICryptoAgent _cryptoAgent;
            private readonly IPasswordHashProvider _passwordHashProvider ;
            private readonly IPasswordChecker _passwordChecker ;
            private readonly ICollection<IValidator<RegisterAccount.Request>> _requestValidators;
            private readonly ICollection<IValidator<ChestAccount>> _accountValidators;

            public Handler (
                IAccountProvider accountProvider,
                ICryptoAgent cryptoAgent,
                IPasswordHashProvider passwordHashProvider,
                IPasswordChecker passwordChecker,
                ICollection<IValidator<RegisterAccount.Request>> requestValidators,
                ICollection<IValidator<ChestAccount>> accountValidators)
            {
                _accountProvider = accountProvider ?? throw new ArgumentNullException(nameof(accountProvider));
                _cryptoAgent = cryptoAgent ?? throw new ArgumentNullException(nameof(cryptoAgent));
                _passwordHashProvider = passwordHashProvider ?? throw new ArgumentNullException(nameof(passwordHashProvider));
                _passwordChecker = passwordChecker ?? throw new ArgumentNullException(nameof(passwordChecker));
                _requestValidators = requestValidators ?? throw new ArgumentNullException(nameof(requestValidators));
                _accountValidators = accountValidators ?? throw new ArgumentNullException(nameof(accountValidators));
            }

            public async Task<Result> Handle(Request request)
            {
                // Check the password
                var isPasswordCorrect = await _passwordChecker.IsPasswordCorrect(request.GlobalPassword, _passwordHashProvider, _cryptoAgent) ;
                if (!isPasswordCorrect) return new Result(false, new string[] { "Wrong password" }) ;

                // Use the registerAccount validators to validate the request
                var newAccountRequest = new RegisterAccount.Request(
                    request.GlobalPassword,
                    request.Name,
                    request.AccountClearPassword,
                    request.Link,
                    request.Username
                );
                var (isRequestValid, requestErrors) = Validator.Validate(_requestValidators, newAccountRequest);
                if (!isRequestValid) return new Result(false, requestErrors);

                // Get a random salt
                var salt = _cryptoAgent.GenerateSalt();

                // Use the generated salt to encrypt the password
                var (encrypted_password, iv) = _cryptoAgent.Encrypt(
                    request.AccountClearPassword,
                    request.GlobalPassword,
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
                account.Link = request.Link ;
                account.Username = request.Username ;

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