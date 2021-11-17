using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Chest.Core;
using Chest.Core.DependencyInjection;
using Chest.Core.Infrastructure;
using Core.Domain.Accounts;
using Core.Domain.Accounts.Services;
using Core.Domain.Crypto;
using Core.Domain.Crypto.Services;
using Core.Domain.PasswordHash;
using Core.Domain.PasswordHash.Pipelines;
using Core.Domain.PasswordHash.Services;
using Core.Domain.Session;
using Core.Domain.Session.Services;
using static Chest.Core.DependencyInjection.Service;
using static Core.Domain.Accounts.Pipelines.RegisterAccount;

namespace Core
{
    public class Program
    {
        public static void Main (string[] args)
        {   
            RegisterServices() ;

            var cryptoAgent = ServiceCollection.GetInstance().GetScope<ICryptoAgent>() ;
            //Console.WriteLine(cryptoAgent.Decrypt(null,null,null,null)) ;
        }

        public static void RegisterServices ()
        {
            ServiceCollection.GetInstance().RegisterScope<ICryptoAgent, FakeCryptoAgent>();
        }
    }

    class FakeCryptoAgent : ICryptoAgent
    {
        public bool CheckHash(string input, string hash)
        {
            throw new NotImplementedException();
        }

        public string Decrypt(byte[] cipher, byte[] IV, string password, byte[] salt)
        {
            return "Hello world" ;
        }

        public (byte[] Cipher, byte[] IV) Encrypt(string plain, string password, byte[] salt)
        {
            throw new NotImplementedException();
        }

        public byte[] GenerateSalt()
        {
            throw new NotImplementedException();
        }

        public string GetHash(string input)
        {
            throw new NotImplementedException();
        }
    }
}
