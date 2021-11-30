using Chest.Core;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Core.Domain.Accounts.Pipelines;
using Core.Domain.Accounts.Services;
using Core.Domain.Accounts;
using Core.Domain.Crypto.Services;
using Core.Domain.Crypto;
using Core.Domain.PasswordHash.Services;
using Chest.Core.DependencyInjection;
using Core.Domain.Session;
using Core.Domain.Session.Services;
using Core.Domain.PasswordHash.Pipelines;
using Core.Domain.PasswordHash;
using System.Windows.Input;

namespace UI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {

        public Window Window { get; set; }
        public WindowLogin WindowLogin { get; set; }

        public App ()
        {

            // Register the services
            ServiceCollection.GetInstance().RegisterScope<ICryptoAgent, CryptoAgent>();
            ServiceCollection.GetInstance().RegisterScope<IAccountProvider, AccountProvider>();
            ServiceCollection.GetInstance().RegisterScope<IPasswordHashProvider, PasswordHashProvider>();
            ServiceCollection.GetInstance().RegisterScope<IChestSessionProvider, ChestSessionProvider>();
            ServiceCollection.GetInstance().RegisterScope<IPasswordChecker, PasswordChecker>();

            // Reset the password
            // ServiceCollection.Handle(new ResetPassword.Request()).GetAwaiter().GetResult();

            // Test if the application is new or not
            var result = ServiceCollection.Handle(new IsPasswordRegistered.Request()).GetAwaiter().GetResult() ;

            // Redirect to the right page
            if (result.IsPasswordRegistered) InitLogin () ;
            else InitSetupPage () ;
        }

        public void DisplayAccountInfos()
        {
            Window.Content = new PageAccountInfos(this) ;
            Window.Show() ;
        }

        private void initMainWindow ()
        {
            Window = new Window ()
            {
                Height = 450,
                Width = 800
            } ;
        }

        public void InitLogin ()
        {
            WindowLogin = new WindowLogin(this) ;
            WindowLogin.Show() ;
        }

        public void InitMenu ()
        {
            initMainWindow() ;
            Window.Content = new Menu(this) ;
            Window.Show() ;
        }

        public void InitSetupPage ()
        {
            initMainWindow() ;
            Window.Content = new PageSetupPassword (this, PageSetupPassword.SetupPasswordType.SET_INITIAL_PASSWORD) ;
            Window.Show() ;
        }

        public void GoFromLoginToMenu ()
        {
            InitMenu() ;
            WindowLogin.Close() ;
        }

        public void GoFromLoginToSetup ()
        {
            initMainWindow() ;
            Window.Content = new PageSetupPassword(this, PageSetupPassword.SetupPasswordType.CHANGE_PASSWORD) ;
            Window.Show() ;
            WindowLogin.Close() ;
        }
        public void UpdateMenu() => Window.Content = new Menu(this) ;
        public void GoFromSetupToMenu () => Window.Content = new Menu(this) ;

        public void GoFromSetupToLogin () 
        {
            InitLogin() ;
            Window.Close() ;
        }
        public void GoFromAccountToMenu () => Window.Content = new Menu (this) ;
        public void GoFromMenuToSetup() => Window.Content = new PageSetupPassword(this, PageSetupPassword.SetupPasswordType.CHANGE_PASSWORD);
        public void GoFromMenuToNewAccount () => Window.Content = new PageAccountInfos(this) ;
        public void GetFromMenuToEditAccount (ChestAccount account) => Window.Content = new PageAccountInfos(this,account) ;
        public void GoFromAccountToPassword (PageAccountInfos parent, string password) 
            => Window.Content = new PagePassword(parent,password) ; 

        public void GoFromPasswordToAccount (PageAccountInfos parent) => Window.Content = parent ;
    }

    class FakePasswordProvider : IPasswordHashProvider
    {
        private string _p ;
        public Task<string> GetPasswordHash() => Task.FromResult(_p ?? throw new NoPasswordStoredException ()) ;

        public Task ResetPasswordHash()
        {
            _p = null ;
            return Task.CompletedTask;
        }

        public Task SetPasswordHash(string hash)
        {
            _p = hash ;
            return Task.CompletedTask ;
        }
    }
}
