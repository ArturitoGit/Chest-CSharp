using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Chest.Core.DependencyInjection;
using Core.Domain.Accounts;
using Core.Domain.Accounts.Services;
using Core.Domain.Crypto.Services;
using Core.Domain.PasswordHash;
using Core.Domain.PasswordHash.Services;
using static Chest.Core.DependencyInjection.Service;

namespace Core.Domain.PasswordHash.Pipelines
{
    public class SetPassword
    {
        public record Request(string OldPassword, string NewPassword) : IRequest<Result> {}
        public record Result(bool Success) : IResult { }

        public class Handler : IRequestHandler<Request, Result>
        {
            private readonly IAccountProvider _accountProvider;
            private readonly IPasswordHashProvider _pwdProvider;
            private readonly ICryptoAgent _cryptoAgent;
            private readonly IPasswordChecker _passwordChecker;

            public Handler(
                IAccountProvider accountProvider,
                IPasswordHashProvider pwdProvider,
                ICryptoAgent cryptoAgent,
                IPasswordChecker passwordChecker)
            {
                _accountProvider = accountProvider;
                _pwdProvider = pwdProvider;
                _cryptoAgent = cryptoAgent;
                _passwordChecker = passwordChecker ?? throw new System.ArgumentNullException(nameof(passwordChecker));
            }

            public async Task<Result> Handle(Request request)
            {

                // Get current password in the database
                try
                {
                    // If a password was found
                    var currentHash = await _pwdProvider.GetPasswordHash();

                    // Check that the given old password is the right one
                    var valid_password = await _passwordChecker.IsPasswordCorrect(request.OldPassword, _pwdProvider, _cryptoAgent) ;

                    // If the password is not valid cancel the operation
                    if (!valid_password) return new Result(false);

                }
                catch (NoPasswordStoredException)
                {
                    // Erase all the saved passwords
                    await _accountProvider.DeleteAllAccounts();
                }

                // Generate a random salt
                var salt = _cryptoAgent.GenerateSalt();
                // Get the hash from the clear password
                var hash = _cryptoAgent.GetHash(request.NewPassword);
                // Register the password in the database
                await _pwdProvider.SetPasswordHash(hash);

                // Update the password for all the account encryptions
                var accounts = await _accountProvider.GetAccounts() ;

/*                 var tasks = accounts.Select(a => 
                {
                    // Get the clear password from the request old key
                    var clearPassword = _cryptoAgent.Decrypt(a.HashedPassword, a.IV, request.OldPassword, a.Salt);
                    // Re-encrypt the clear password with the new key
                    var (newPassword, iv) = _cryptoAgent.Encrypt(clearPassword, request.NewPassword, a.Salt);
                    a.IV = iv;
                    a.HashedPassword = newPassword;
                    // Update the account in the database
                    return _accountProvider.UpdateAccount(a);
                }) ;
                // Apply this task async to all elements of the list
                await Task.WhenAll(tasks) ; */

                await _accountProvider.ApplyToAllAccounts( a => 
                {
                    // Get the clear password from the request old key
                    var clearPassword = _cryptoAgent.Decrypt(a.HashedPassword, a.IV, request.OldPassword, a.Salt);
                    // Re-encrypt the clear password with the new key
                    var (newPassword, iv) = _cryptoAgent.Encrypt(clearPassword, request.NewPassword, a.Salt);
                    a.IV = iv;
                    a.HashedPassword = newPassword;
                    return a ; 
                });

                return new Result(true);
            }
        }
    }
}