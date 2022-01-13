using System.Threading.Tasks;
using Core.Domain.Crypto;
using Core.Domain.Crypto.Services;
using Core.Domain.PasswordHash;
using Core.Domain.PasswordHash.Pipelines;
using Core.Domain.PasswordHash.Services;
using Shouldly;
using Tests.SharedKernel;
using Xunit;

namespace Tests.Domain.PasswordHash
{
    public class PasswordCheckerTests 
    {

        private IPasswordChecker _pwdChecker ;
        private IPasswordHashProvider _pwdProvider ;
        private ICryptoAgent _cryptoAgent ;

        public PasswordCheckerTests()
        {
            _cryptoAgent = new CryptoAgent() ;
            _pwdProvider = new FakePasswordHashProvider() ;
            _pwdChecker = new PasswordChecker() ;
        }

        [Fact]
        public void CheckPassword_ReturnSuccessIfRightPassword()
        {
            // Register a password
            var right_password = "right password will you find it";
            SetPassword (right_password);

            // Call the check pipeline with the right password
            var result = _pwdChecker.IsPasswordCorrect(right_password, _pwdProvider, _cryptoAgent).GetAwaiter().GetResult() ;

            result.ShouldBeTrue();
        }

        [Fact]
        public void CheckPassword_ReturnsFailureIfWrongPassword()
        {
            // Register a password
            var right_password = "right password will you find it";
            var wrong_password = "right password will you find i";
            SetPassword(right_password);

            // Call the check pipeline with the right password
            var result = _pwdChecker.IsPasswordCorrect(wrong_password, _pwdProvider, _cryptoAgent).GetAwaiter().GetResult() ;

            result.ShouldBeFalse();
        }

        [Fact]
        public void CheckPassword_ThrowsExceptionIfNoPasswordIsStored()
        {
            // Call the check pipeline with the right password
            _pwdChecker.IsPasswordCorrect("something", _pwdProvider, _cryptoAgent).ShouldThrow(typeof(NoPasswordStoredException));
        }

        private void SetPassword(string password)
        {
            var setPasswordHandler = new SetPassword.Handler( 
                FakeAccountProvider.INSTANCE, 
                _pwdProvider, 
                _cryptoAgent, 
                _pwdChecker);
            var setPasswordRequest = new SetPassword.Request(null, password);
            setPasswordHandler.Handle(setPasswordRequest).GetAwaiter().GetResult();
        }
    }
}