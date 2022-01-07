using ElectronNET.API;
using static UI.DependencyInjection.ServiceCollection ;
using UI.PasswordHash.Pipelines;
using UI.Domain.Session.Pipelines;
using UI.Domain.PasswordHash.Pipelines;
using System.Text.Json;
using UI.Domain.Accounts.Services;
using UI.Domain.Accounts;
using UI.Domain.Accounts.Pipelines;
using System;
using UI.Domain.Generator.Pipelines;

namespace UI
{
    public class UIRequestHandler
    {
        public static void Subscribe ()
        {
            // Register for each pipeline
            Electron.IpcMain.OnSync("is-password-registered", args =>
            {
                // Call the handler
                var result = Handle(new IsPasswordRegistered.Request())
                    .GetAwaiter().GetResult() ;

                // Serialize the answer
                var serialized_result = JsonSerializer.Serialize(result) ;
                return serialized_result ;
            });

            // Register for each pipeline
            Electron.IpcMain.OnSync("delete-account", args =>
            {
                Debug("Delete account request received : " + args.ToString()) ;
                Debug("ID to be deleted : " + (Guid.Parse(args.ToString()!)).ToString()) ;

                // Get the account provider
                var provider = GetInstance().GetScope<IAccountProvider>() ;

                // Use the provider to delete the account
                provider.DeleteAccount(Guid.Parse(args.ToString()!)).GetAwaiter().GetResult() ;

                Debug("Account deleted !") ;

                // Answer something
                return null ;
            });

            Electron.IpcMain.OnSync("generate-password", args =>
            {
                Debug("Generate password request received : " + args.ToString()) ;

                Debug(JsonSerializer.Serialize(new GeneratePassword.Request(
                    10,
                    true,
                    true,
                    true,
                    true,
                    new string[] {}
                )));

                // Handle the request
                var request = (GeneratePassword.Request) JsonSerializer.Deserialize(args.ToString()!, typeof(GeneratePassword.Request))! ;
                var result = Handle(request).GetAwaiter().GetResult() ;

                // Serialize the result
                var serialized_result = JsonSerializer.Serialize(result) ;
                
                Debug("Send Generate password result : ") ;
                Debug(serialized_result.ToString()) ;

                return serialized_result ;
            });

            Electron.IpcMain.OnSync("create-password", args =>
            {
                // Call the handler
                var result = Handle(new SetPassword.Request(args.ToString()!))
                    .GetAwaiter().GetResult() ;

                // Serialize the answer
                var serialized_result = JsonSerializer.Serialize(result) ;
                return serialized_result ;
            });

            Electron.IpcMain.OnSync("get-accounts", args =>
            {
                // Get the accounts from the provider
                var accounts = GetInstance().GetScope<IAccountProvider>().GetAccounts().GetAwaiter().GetResult() ;

                // Serialize the answer
                var serialized_result = JsonSerializer.Serialize(accounts) ;
                return serialized_result ;
            });

            Electron.IpcMain.OnSync("open-chest", args =>
            {
                // Call the handler
                var result = Handle(new OpenChestSession.Request(args.ToString()!))
                    .GetAwaiter().GetResult() ;

                // Serialize the answer
                var serialized_result = JsonSerializer.Serialize(result) ;
                return serialized_result ;
            });

            Electron.IpcMain.OnSync("decrypt-password", args =>
            {

                Debug("Decrypt Password request has been received") ;

                // Deserialize the request
                var request = new DecryptPassword.Request( (ChestAccount) JsonSerializer.Deserialize(args.ToString()!, typeof(ChestAccount))!) ;
                
                Debug ("Decrypt password request has been deserialized : ") ;
                Debug (request.ToString()) ;

                // Call the handler
                var result = Handle(request)
                    .GetAwaiter().GetResult() ;

                Debug ("Result of decrypt password request is : ") ;
                Debug(result.ToString()) ;

                // Serialize the answer
                var serialized_result = JsonSerializer.Serialize(result) ;
                return serialized_result ;
            });

            Electron.IpcMain.OnSync("add-account", args =>
            {
                Debug("Add account request received !") ;
                var request = JsonSerializer.Deserialize(args.ToString()!, typeof(RegisterAccount.Request)) ;
                if (request is null) throw new Exception("Request not deserializable : " + nameof(request)) ;

                var result = Handle( (RegisterAccount.Request) request!)
                    .GetAwaiter().GetResult() ;

                // Sezrialize the answer
                var serialized_result = JsonSerializer.Serialize(result) ;

                Debug("Add account response sent : " + serialized_result.ToString()) ;

                return serialized_result ;
            }) ;
        }

        private static void Debug (string message)
        {
            if (Startup.DEBUG) Console.WriteLine(message) ;
        }
    }
}