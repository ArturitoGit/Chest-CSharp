using Core.Domain.Accounts;
using Core.Domain.Crypto;
using Core.Domain.Crypto.Services;
using Core.Domain.PasswordHash;

namespace Tests.SharedKernel
{
    public abstract class JsonTest
    {

        private string ACCOUNT_FILE = "accounts.txt" ;
        private string PASSWORD_FILE = "pwd.txt" ;
        protected AccountProvider _accountProvider ;
        protected PasswordHashProvider _pwdProvider ;
        protected ICryptoAgent _cryptoAgent ;

        protected JsonTest ()
        {
            _accountProvider = new AccountProvider(ACCOUNT_FILE) ;
            _pwdProvider = new PasswordHashProvider(PASSWORD_FILE) ;
            _cryptoAgent = new CryptoAgent() ;

            Dispose() ;
        }

        protected void Dispose ()
        {
            _pwdProvider.Dispose() ;
            _accountProvider.Dispose() ;
        }
    }
}