using System.IO;
using Core.Domain.Accounts;
using Shouldly;
using Xunit;

namespace Tests.Domain.Account
{
    public class AccountProviderTests
    {

        [Fact]
        public void addAccount_CreatesAFile()
        {
            var account_file_path = "accounts.txt";
            var hashedPassword = new byte[] { };
            var name = "name of the account";
            var iv = new byte[] { };
            var salt = new byte[] { };
            var account = new ChestAccount(hashedPassword, name, salt, iv);
            var provider = new AccountProvider(account_file_path);

            if (File.Exists(account_file_path)) File.Delete(account_file_path);

            provider.AddAccount(account).GetAwaiter().GetResult();

            File.Exists(account_file_path).ShouldBeTrue();

            File.Delete(account_file_path);
        }

        [Fact]
        public void addAccount_AddsTheAccount()
        {

            var account_file_path = "accounts.txt";
            var hashedPassword = new byte[] { byte.MaxValue, byte.MinValue };
            var name = "name of the account";
            var iv = new byte[] { };
            var salt = new byte[] { };
            var account = new ChestAccount(hashedPassword, name, salt, iv);
            var provider = new AccountProvider(account_file_path);

            if (File.Exists(account_file_path)) File.Delete(account_file_path);

            provider.AddAccount(account).GetAwaiter().GetResult();

            var result = provider.GetAccounts().GetAwaiter().GetResult();

            result.Length.ShouldBe(1);
            var result_account = result[0];

            result_account.Name.ShouldBe(name);
            result_account.HashedPassword.ShouldBe(hashedPassword);
        }

        [Fact]
        public void addAccount_AppendTheAccount()
        {

            var account_file_path = "accounts.txt";

            var hashedPassword1 = new byte[] { };
            var name1 = "name of the account";
            var iv1 = new byte[] { };
            var salt1 = new byte[] { };
            var account1 = new ChestAccount(hashedPassword1, name1, salt1, iv1);

            var hashedPassword2 = new byte[] { };
            var name2 = "name of the account";
            var iv2 = new byte[] { };
            var salt2 = new byte[] { };
            var account2 = new ChestAccount(hashedPassword2, name2, salt2, iv2);


            var provider = new AccountProvider(account_file_path);

            if (File.Exists(account_file_path)) File.Delete(account_file_path);

            provider.AddAccount(account1).GetAwaiter().GetResult();
            provider.AddAccount(account2).GetAwaiter().GetResult();

            var result = provider.GetAccounts().GetAwaiter().GetResult();

            result.Length.ShouldBe(2);
            var result_account = result[0];
            var result_account_2 = result[1];

            result_account.Id.ShouldNotBe(result_account_2.Id);

            result_account.Name.ShouldBe(name1);
            result_account.HashedPassword.ShouldBe(hashedPassword1);

            result_account_2.Name.ShouldBe(name2);
            result_account_2.HashedPassword.ShouldBe(hashedPassword2);
        }

        [Fact]
        public void DeleteAccount_DeletesTheAccountOnly()
        {
            var account_file_path = "accounts.txt";

            var hashedPassword1 = new byte[] { };
            var name1 = "name of the account";
            var iv1 = new byte[] { };
            var salt1 = new byte[] { };
            var account1 = new ChestAccount(hashedPassword1, name1, salt1, iv1);

            var hashedPassword2 = new byte[] { };
            var name2 = "name of the account";
            var iv2 = new byte[] { };
            var salt2 = new byte[] { };
            var account2 = new ChestAccount(hashedPassword2, name2, salt2, iv2);


            var provider = new AccountProvider(account_file_path);

            if (File.Exists(account_file_path)) File.Delete(account_file_path);

            provider.AddAccount(account1).GetAwaiter().GetResult();
            provider.AddAccount(account2).GetAwaiter().GetResult();

            var accounts = provider.GetAccounts().GetAwaiter().GetResult();

            provider.DeleteAccount(accounts[0].Id).GetAwaiter().GetResult();

            var result = provider.GetAccounts().GetAwaiter().GetResult();

            result.Length.ShouldBe(1);
            var result_account_2 = result[0];

            result_account_2.Name.ShouldBe(name2);
            result_account_2.HashedPassword.ShouldBe(hashedPassword2);
        }

        [Fact]
        public void UpdateAccount_UpdateTheAccountOnly()
        {
            var account_file_path = "accounts.txt";

            var hashedPassword1 = new byte[] { };
            var name1 = "name of the account";
            var iv1 = new byte[] { };
            var salt1 = new byte[] { };
            var account1 = new ChestAccount(hashedPassword1, name1, salt1, iv1);

            var hashedPassword2 = new byte[] { };
            var name2 = "name of the account";
            var iv2 = new byte[] { };
            var salt2 = new byte[] { };
            var account2 = new ChestAccount(hashedPassword2, name2, salt2, iv2);

            var account2_new_name = "new name for the account";
            var provider = new AccountProvider(account_file_path);

            if (File.Exists(account_file_path)) File.Delete(account_file_path);

            provider.AddAccount(account1).GetAwaiter().GetResult();
            provider.AddAccount(account2).GetAwaiter().GetResult();

            account2.Name = account2_new_name;

            provider.UpdateAccount(account2).GetAwaiter().GetResult();

            var result = provider.GetAccounts().GetAwaiter().GetResult();

            result.Length.ShouldBe(2);
            var result_account = result[0];
            var result_account_2 = result[1];

            result_account.Name.ShouldBe(name1);
            result_account.HashedPassword.ShouldBe(hashedPassword1);

            result_account_2.Name.ShouldBe(account2_new_name);
            result_account_2.HashedPassword.ShouldBe(hashedPassword2);
        }

    }
}