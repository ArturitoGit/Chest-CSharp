using Core.Domain.PasswordHash;
using Core.Domain.PasswordHash.Services;
using Shouldly;
using Tests.SharedKernel;
using Xunit;
using static Core.Domain.PasswordHash.Pipelines.SetPassword;

namespace Tests.Domain.Session.Pipelines
{
    public class SetPasswordTests : JsonTest
    {

        private Handler _handler;

        private IPasswordChecker _pwdChecker ;

        public SetPasswordTests()
        {
            _pwdChecker = new PasswordChecker(_pwdProvider,_cryptoAgent) ;
            _handler = new Handler(_accountProvider, _pwdProvider, _cryptoAgent, _pwdChecker);
        }

        [Fact]
        public void SetPassword_SetsTheHash()
        {
            // Reset the database
            var clearPassword = "right password will you find it";

            // Send the request to set the password
            var request = new Request(clearPassword);
            _handler.Handle(request).GetAwaiter().GetResult();

            // Get the registered password
            var registeredPassword = _pwdProvider.GetPasswordHash().GetAwaiter().GetResult();
            _cryptoAgent.CheckHash(clearPassword, registeredPassword).ShouldBeTrue();

            Dispose();
        }

        [Fact]
        public void SetPassword_ChangesPassword()
        {
            var previousClearPassword = "previous password !";
            var newClearPassword = "new password !";

            var request1 = new Request(previousClearPassword);
            var request2 = new Request(previousClearPassword, newClearPassword);
            _handler.Handle(request1).GetAwaiter().GetResult();
            var result = _handler.Handle(request2).GetAwaiter().GetResult();

            var registeredPassword = _pwdProvider.GetPasswordHash().GetAwaiter().GetResult();

            result.Success.ShouldBe(true);
            _cryptoAgent.CheckHash(newClearPassword, registeredPassword).ShouldBeTrue();

            Dispose();
        }

        [Fact]
        public void SetPassword_DoesNothingIfOldPasswordIsWrong()
        {
            var previousClearPassword = "previous password !";
            var newClearPassword = "new password !";

            var request1 = new Request(previousClearPassword);
            var request2 = new Request(newClearPassword, newClearPassword);
            _handler.Handle(request1).GetAwaiter().GetResult();
            var result = _handler.Handle(request2).GetAwaiter().GetResult();

            var registeredPassword = _pwdProvider.GetPasswordHash().GetAwaiter().GetResult();

            result.Success.ShouldBe(false);
            _cryptoAgent.CheckHash(newClearPassword, registeredPassword).ShouldBeFalse();
            _cryptoAgent.CheckHash(previousClearPassword, registeredPassword).ShouldBeTrue();

            Dispose();
        }
    }
}