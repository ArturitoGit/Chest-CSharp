using Chest.Core.DependencyInjection;
using Chest.Core.Infrastructure;
using Core.Domain.Accounts;
using Core.Domain.Accounts.Pipelines;
using Core.Domain.PasswordHash.Pipelines;
using Shouldly;
using Tests.SharedKernel;
using Xunit;

namespace Tests.Domain.Account.Pipelines
{
    public class RegisterAccountTests : JsonTest
    {
        private readonly string GLOBAL_PASSWORD = "Jordy" ; 
        private readonly RegisterAccount.Handler _handler;
        private readonly IValidator<ChestAccount>[] _accountValidators;
        private readonly IValidator<RegisterAccount.Request>[] _requestValidators;

        public RegisterAccountTests()
        {
            _accountValidators = ServiceCollection.GetValidators<ChestAccount>();
            _requestValidators = ServiceCollection.GetValidators<RegisterAccount.Request>();
            _handler = new RegisterAccount.Handler(
                _cryptoAgent,
                _accountProvider, 
                _pwdProvider,
                _pwdChecker,
                _accountValidators, 
                _requestValidators);

            // Setup global password
            new SetPassword.Handler(_accountProvider,_pwdProvider, _cryptoAgent, _pwdChecker)
                .Handle(new SetPassword.Request(GLOBAL_PASSWORD)).GetAwaiter().GetResult() ;
        }

        [Fact]
        public void RegisterAccount_AddsTheAccount()
        {
            // Build the request
            var name = "name of the account";
            var accountClearPassword = "Clear password for my account ... !";
            var link = "link" ;
            var username = "username" ;
            var request = new RegisterAccount.Request(GLOBAL_PASSWORD, name, accountClearPassword, link, username);

            // Call the handler
            var result = _handler.Handle(request).GetAwaiter().GetResult();
            result.Success.ShouldBeTrue();

            // Check that the account provider has the account
            var stored = _accountProvider.GetAccounts().GetAwaiter().GetResult();
            stored.Length.ShouldBe(1);

            var storedAccount = stored[0];
            storedAccount.Name.ShouldBe(name);

            Dispose();
        }

        [Fact]
        public void RegisterAccount_DoesNothingIfWrongPassword ()
        {
            // Build the request
            var name = "name of the account";
            var accountClearPassword = "Clear password for my account ... !";
            var link = "link" ;
            var wrongPassword = GLOBAL_PASSWORD + "wrong_part" ;
            var request = new RegisterAccount.Request(wrongPassword, name, accountClearPassword, link, null) ;

            // Call the handler
            var result = _handler.Handle(request).GetAwaiter().GetResult() ;
            
            result.Success.ShouldBeFalse() ;

            var stored = _accountProvider.GetAccounts().GetAwaiter().GetResult() ;
            stored.Length.ShouldBe(0) ;

            Dispose() ;
        }

        [Fact]
        public void RegisterAccount_AddsTheLink ()
        {
            // Build the request
            var name = "name of the account";
            var accountClearPassword = "Clear password for my account ... !";
            var link = "link" ;
            var request = new RegisterAccount.Request(GLOBAL_PASSWORD, name, accountClearPassword, link, null);

            // Call the handler
            var result = _handler.Handle(request).GetAwaiter().GetResult();
            result.Success.ShouldBeTrue();

            // Check that the account provider has the account
            var stored = _accountProvider.GetAccounts().GetAwaiter().GetResult();
            stored.Length.ShouldBe(1);

            var storedAccount = stored[0];
            storedAccount.Link.ShouldBe(link);

            Dispose();
        }

        [Fact]
        public void RegisterAccount_AddsADecryptableAccount()
        {
            // Build the request
            var name = "name of the account";
            var accountClearPassword = "Clear password for my account ... !";
            var link = "link" ;
            var request = new RegisterAccount.Request(GLOBAL_PASSWORD, name, accountClearPassword, link, null);

            // Call the handler
            _handler.Handle(request).GetAwaiter().GetResult();
            var stored = _accountProvider.GetAccounts().GetAwaiter().GetResult()[0];

            // Try to decrypt the hash
            var decrypted = _cryptoAgent.Decrypt(
                stored.HashedPassword,
                stored.IV,
                GLOBAL_PASSWORD,
                stored.Salt
            );

            decrypted.ShouldBe(accountClearPassword);

            Dispose();
        }

        [Fact]
        public void RegisterAccount_DoesNotAddNotValidAccount()
        {
            // Build the requests
            var name = "valid name";
            var accountClearPassword = "Clear password for my account ... !";
            var link = "link" ;

            var request_1 = new RegisterAccount.Request(GLOBAL_PASSWORD, "", accountClearPassword,link, null);
            var request_2 = new RegisterAccount.Request(GLOBAL_PASSWORD, null, accountClearPassword,link, null);
            var request_3 = new RegisterAccount.Request(GLOBAL_PASSWORD, name, "",link, null);
            var request_4 = new RegisterAccount.Request(GLOBAL_PASSWORD, name, null,link, null);

            // Call the handler
            var result_1 = _handler.Handle(request_1).GetAwaiter().GetResult();
            var result_2 = _handler.Handle(request_2).GetAwaiter().GetResult();
            var result_3 = _handler.Handle(request_3).GetAwaiter().GetResult();
            var result_4 = _handler.Handle(request_4).GetAwaiter().GetResult();

            // All the tries should have failed
            result_1.Success.ShouldBeFalse();
            result_2.Success.ShouldBeFalse();
            result_3.Success.ShouldBeFalse();
            result_4.Success.ShouldBeFalse();

            // The stored accounts list should be empty
            _accountProvider.GetAccounts().GetAwaiter().GetResult().ShouldBeEmpty();

            Dispose();
        }

    }
}