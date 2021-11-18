using System.Collections.Generic;
using System.Linq;
using Chest.Core.Infrastructure;
using Core.Domain.Accounts;
using Core.Domain.Accounts.Pipelines;
using Core.Domain.Accounts.Services;
using Core.Domain.Crypto;
using Core.Domain.Crypto.Services;
using Core.Domain.PasswordHash;
using Core.Domain.PasswordHash.Services;
using Shouldly;
using Tests.SharedKernel;
using Xunit;
using static Core.Domain.PasswordHash.Pipelines.SetPassword;

namespace Tests.Domain.Session.Pipelines
{
    public class SetPasswordTests
    {

        private Handler _handler;
        private IPasswordChecker _pwdChecker ;
        private IAccountProvider _accountProvider ;
        private IPasswordHashProvider _pwdProvider ;
        private ICryptoAgent _cryptoAgent ;

        private FakeSessionProvider _sessionProvider ;

        public SetPasswordTests()
        {
            _sessionProvider = new FakeSessionProvider() ;
            _cryptoAgent = new CryptoAgent() ;
            _pwdProvider = FakePasswordHashProvider.INSTANCE ;
            _accountProvider = FakeAccountProvider.INSTANCE ;

            _pwdChecker = new PasswordChecker (
                _pwdProvider,
                _cryptoAgent) ;

            _handler = new Handler (
                _accountProvider, 
                _pwdProvider, 
                _cryptoAgent, 
                _pwdChecker
            );

            _pwdProvider.ResetPasswordHash() ;
        }

        [Fact]
        public void SetPassword_SetsTheHash()
        {
            var clearPassword = "right password will you find it";

            // Send the request to set the password
            var request = new Request(clearPassword);
            _handler.Handle(request).GetAwaiter().GetResult();

            // Get the registered password
            var registeredPassword = _pwdProvider.GetPasswordHash().GetAwaiter().GetResult();
            _cryptoAgent.CheckHash(clearPassword, registeredPassword).ShouldBeTrue();
        }

        [Fact]
        public void SetPassword_ChangesPassword()
        {
            var previousClearPassword = "previous password !";
            var newClearPassword = "new password !";

            // Set a first password
            var request1 = new Request(previousClearPassword);
            _handler.Handle(request1).GetAwaiter().GetResult();

            // Change the password
            var request2 = new Request(previousClearPassword, newClearPassword);
            var result = _handler.Handle(request2).GetAwaiter().GetResult();

            // Get the current stored password
            var registeredPassword = _pwdProvider.GetPasswordHash().GetAwaiter().GetResult();

            result.Success.ShouldBeTrue();
            _cryptoAgent.CheckHash(newClearPassword, registeredPassword).ShouldBeTrue();
        }

        [Fact]
        public void SetPassword_DoesNothingIfOldPasswordIsWrong()
        {
            var previousClearPassword = "previous password !";
            var newClearPassword = "new password !";

            // Set a first password
            var request1 = new Request(previousClearPassword);
            _handler.Handle(request1).GetAwaiter().GetResult();

            // Try to change it with a wrong argument
            var request2 = new Request(newClearPassword, newClearPassword);
            var result = _handler.Handle(request2).GetAwaiter().GetResult();

            // Get the current stored password
            var registeredPassword = _pwdProvider.GetPasswordHash().GetAwaiter().GetResult();

            result.Success.ShouldBeFalse();
            _cryptoAgent.CheckHash(newClearPassword, registeredPassword).ShouldBeFalse();
            _cryptoAgent.CheckHash(previousClearPassword, registeredPassword).ShouldBeTrue();
        }

        [Fact]
        public void SetPassword_UpdatesTheAccounts ()
        {
            var previousClearPassword = "previous password !";
            var newClearPassword = "new password !";

            // Set a first password
            var request1 = new Request(previousClearPassword);
            _handler.Handle(request1).GetAwaiter().GetResult();
            _sessionProvider.Password = previousClearPassword ;

            var requestValidators = new List<IValidator<RegisterAccount.Request>>() ; 
            var accountValidators = new List<IValidator<ChestAccount>>() ; 

            // Add Accounts in the db
            var addAccountHandler = new RegisterAccount.Handler(
                _cryptoAgent,
                _accountProvider, 
                _sessionProvider,
                accountValidators,
                requestValidators
            ) ;
            var password1 = "password of account 1" ;
            var name1 = "name_1" ;
            var resultAddAccount1 = addAccountHandler.Handle(new RegisterAccount.Request(
                name1, password1, "link of account 1"
            )).GetAwaiter().GetResult() ;
            var name2 = "name_2" ;
            var password2 = "second password for a second account" ;
            var resultAddAccount2 = addAccountHandler.Handle(new RegisterAccount.Request(
                name2, password2, "link of account 2"
            )).GetAwaiter().GetResult() ;

            // Change the password
            var request2 = new Request(previousClearPassword, newClearPassword);
            var result = _handler.Handle(request2).GetAwaiter().GetResult();
            _sessionProvider.Password = newClearPassword ;

            // Get the current stored accounts
            var account1 = _accountProvider.GetAccounts().GetAwaiter().GetResult().Where(a => a.Name == name1).First() ;
            var account2 = _accountProvider.GetAccounts().GetAwaiter().GetResult().Where(a => a.Name == name2).First() ;

            // Try to decrypt the stored password with the new password
            _cryptoAgent.Decrypt(
                account1.HashedPassword,
                account1.IV,
                newClearPassword,
                account1.Salt
            ).ShouldBe(password1) ;

            _cryptoAgent.Decrypt(
                account2.HashedPassword,
                account2.IV,
                newClearPassword,
                account2.Salt
            ).ShouldBe(password2) ;
        }
    }
}