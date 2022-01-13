using Core.Domain.Accounts;
using Core.Domain.Crypto;
using Core.Domain.Crypto.Services;
using Core.Domain.PasswordHash;
using Core.Domain.PasswordHash.Services;

namespace Tests.SharedKernel
{
    public abstract class JsonTest
    {

        private readonly string ACCOUNT_FILE = "accounts.txt" ;
        private readonly string PASSWORD_FILE = "pwd.txt" ;
        protected AccountProvider _accountProvider ;
        protected PasswordHashProvider _pwdProvider ;
        protected ICryptoAgent _cryptoAgent ;

        protected IPasswordChecker _pwdChecker ;

        protected JsonTest ()
        {
            _accountProvider = new AccountProvider(ACCOUNT_FILE) ;
            _pwdProvider = new PasswordHashProvider(PASSWORD_FILE) ;
            _cryptoAgent = new CryptoAgent() ;
            _pwdChecker = new PasswordChecker() ;

            Dispose() ;
        }

        protected void Dispose ()
        {
            _pwdProvider.Dispose() ;
            _accountProvider.Dispose() ;
        }
    }
}