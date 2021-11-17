using System.IO;
using Core.Domain.PasswordHash;
using Shouldly;
using Xunit;

namespace Tests.Domain.PasswordHash
{
    public class PasswordHashProviderTests
    {

        [Fact]
        public async void SetPasswordHashCreatesAFile()
        {
            var pwd_file_path = "pwd.txt";
            var hash_content = "my haase";
            var provider = new PasswordHashProvider(pwd_file_path);
            await provider.SetPasswordHash(hash_content);

            File.Exists(pwd_file_path).ShouldBeTrue();

            File.Delete(pwd_file_path);
        }

        [Fact]
        public async void GetPasswordHash_ReturnsStoredHash()
        {
            var pwd_file_path = "pwd.txt";
            var hash_content = "my haase";
            var provider = new PasswordHashProvider(pwd_file_path);
            await provider.SetPasswordHash(hash_content);

            provider.GetPasswordHash().GetAwaiter().GetResult().ShouldBe(hash_content);

            File.Delete(pwd_file_path);
        }

        [Fact]
        public async void SetPasswordHash_UpdatesStoredHash()
        {
            var pwd_file_path = "pwd.txt";
            var hash_content = "my haase";
            var hash_content2 = "my new haase";
            var provider = new PasswordHashProvider(pwd_file_path);
            await provider.SetPasswordHash(hash_content);
            await provider.SetPasswordHash(hash_content2);

            provider.GetPasswordHash().GetAwaiter().GetResult().ShouldBe(hash_content2);

            File.Delete(pwd_file_path);
        }

        [Fact]
        public void SetPasswordHash_IsNotBlocking ()
        {
            var pwd_file_path = "pwd.txt";
            var provider = new PasswordHashProvider(pwd_file_path) ;

            provider.SetPasswordHash("testshdnvs").GetAwaiter().GetResult() ;
            provider.GetPasswordHash().GetAwaiter().GetResult().ShouldBe("testshdnvs") ;
            provider.SetPasswordHash("testshsdfopihsd").GetAwaiter().GetResult() ;
            provider.GetPasswordHash().GetAwaiter().GetResult().ShouldBe("testshsdfopihsd") ;
            provider.SetPasswordHash("testssfdnjlsdbf").GetAwaiter().GetResult() ;
            provider.GetPasswordHash().GetAwaiter().GetResult().ShouldBe("testssfdnjlsdbf") ;

        }
    }
}