using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Chest.Core.DependencyInjection;
using Core.Domain.Accounts;
using Core.Domain.Accounts.Pipelines;
using Core.Domain.PasswordHash.Pipelines;
using Core.Domain.Session;

namespace UI
{
    /// <summary>
    /// Logique d'interaction pour PageAccountInfos.xaml
    /// </summary>
    public partial class PageAccountInfos : Page
    {

        private Guid? _accountId ;
        private readonly App _parent;

        public PageAccountInfos(App parent)
        {
            InitializeComponent();
            LabelError.Visibility = Visibility.Hidden;
            this._parent = parent ?? throw new ArgumentNullException(nameof(parent));
        }

        public PageAccountInfos (App parent, ChestAccount account) : this (parent)
        {
            // Use it to decrypt the stored password
            var result = ServiceCollection.Handle(new DecryptPassword.Request(account)).GetAwaiter().GetResult() ;

            // Update the fields of the form
            TextBoxName.Text = account.Name ;
            TextBoxPassword.Text = result.ClearPassword ;
            TextBoxLink.Text = account.Link ?? "" ;
            TextBoxUser.Text = account.Username ?? "" ;
            _accountId = account.Id ;
        }

        private void OnEdit(object sender, RoutedEventArgs e)
        {
            // Edit the password
            var password = TextBoxPassword.Text ;
            _parent.GoFromAccountToPassword(this, password) ;
        }

        private void OnSubmit(object sender, RoutedEventArgs e)
        {

            if (_accountId is null)
            {
                SubmitNewAccount() ;
            }
            else{
                // Update request
                SubmitUpdateAccount() ;
            }
        }

        private void OnCancel(object sender, RoutedEventArgs e)
        {
            _parent.GoFromAccountToMenu() ;
        }

        private async void SubmitNewAccount()
        {
                // Reset the label of errors
                LabelError.Visibility = Visibility.Hidden ;

                // Pick up the password content
                var password = TextBoxPassword.Text ;

                // Create request
                var request = new RegisterAccount.Request (
                    TextBoxName.Text,
                    password,
                    TextBoxLink.Text,
                    TextBoxUser.Text
                ) ;

                // Handle the request
                var result = await ServiceCollection.Handle(request) ;

                // If success then go back to the menu
                if (result.Success)
                {
                    _parent.GoFromAccountToMenu() ;
                    return ;
                }

                // If failure then display the first error message
                LabelError.Content = result.Errors[0] ;
                LabelError.Visibility = Visibility.Visible ;
        }

        private async void SubmitUpdateAccount ()
        {
                // Reset the label of errors
                LabelError.Visibility = Visibility.Hidden ;

                // Pick up the password content
                var password = TextBoxPassword.Text ;

                // Create request
                var request = new UpdateAccount.Request (
                    _accountId.Value,
                    TextBoxName.Text,
                    password,
                    TextBoxLink.Text,
                    TextBoxUser.Text
                ) ;

                // Handle the request
                var result = await ServiceCollection.Handle(request) ;

                // If success then go back to the menu
                if (result.Success)
                {
                    _parent.GoFromAccountToMenu() ;
                    return ;
                }

                // If failure then display the first error message
                LabelError.Content = result.Errors[0] ;
                LabelError.Visibility = Visibility.Visible ;
        }

        // Function that must be called by Page password when password is submited
        public void ReturnPassword (string password)
        {
            TextBoxPassword.Text = password ;
            Return() ;
        }

        public void Return () => _parent.GoFromPasswordToAccount(this) ;
    }
}
