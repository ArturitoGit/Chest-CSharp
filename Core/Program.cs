using System;
using Chest.Core.DependencyInjection;
using Core.Domain.Accounts;
using Core.Domain.Accounts.Services;
using Core.Domain.Crypto;
using Core.Domain.Crypto.Services;
using Core.Domain.PasswordHash;
using Core.Domain.PasswordHash.Pipelines;
using Core.Domain.PasswordHash.Services;
using Core.Domain.Session;
using Core.Domain.Session.Pipelines;
using Core.Domain.Session.Services;
using System.Linq ;

namespace Core
{
    public class Program
    {
        public static void Main (string[] args)
        {   
            // Register the services
            ServiceCollection.GetInstance().RegisterScope<ICryptoAgent, CryptoAgent>();
            ServiceCollection.GetInstance().RegisterScope<IAccountProvider, AccountProvider>();
            ServiceCollection.GetInstance().RegisterScope<IChestSessionProvider, ChestSessionProvider>();
            ServiceCollection.GetInstance().RegisterScope<IPasswordHashProvider, PasswordHashProvider>();
            ServiceCollection.GetInstance().RegisterScope<IPasswordChecker, PasswordChecker>();

            // Check if the chest has a registered password
            var result = ServiceCollection.Handle(new IsPasswordRegistered.Request()).GetAwaiter().GetResult() ;
            if ( !result.IsPasswordRegistered )
            {
                Console.WriteLine("Your chest has not been initialized yet ...") ;
                return ;
            }

            // Ask for the password
            Console.WriteLine("Please enter the password of the chest") ;
            var password = Console.ReadLine() ;
            if (string.IsNullOrEmpty(password)) 
            {
                Console.WriteLine("The password must not be null !") ;
                return ;
            }

            // Check it
            var openChestSession_result = ServiceCollection.Handle(new OpenChestSession.Request(password))
                    .GetAwaiter().GetResult() ;
            if (!openChestSession_result.Success) 
            {
                Console.WriteLine("The password is not valid ...") ;
                return ;
            }

            var accountProvider = ServiceCollection.GetInstance().GetScope<IAccountProvider>() ;
            var accounts = accountProvider.GetAccounts().GetAwaiter().GetResult() ;
            var gitAccount = accounts.Single( a => a.Name == "Github") ;

            var uncryptResult = ServiceCollection.Handle(new DecryptPassword.Request(gitAccount)).GetAwaiter().GetResult() ;

            if (!uncryptResult.Success)
            {
                Console.WriteLine("Failed to decrypt the password ...") ;
                return ;
            }

            Console.WriteLine("Password of Git : ") ;
            Console.WriteLine(uncryptResult.ClearPassword) ;
        }
    }
}
