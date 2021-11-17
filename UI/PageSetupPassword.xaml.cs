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
using Chest.Core;
using Chest.Core.DependencyInjection;
using Core.Domain.PasswordHash.Pipelines;
using Core.Domain.Session.Pipelines ;

namespace UI
{
    /// <summary>
    /// Logique d'interaction pour PageSetupPassword.xaml
    /// </summary>
    public partial class PageSetupPassword : Page
    {
        private readonly App _parent;
        private SetupPasswordType _type ;

        public PageSetupPassword(App parent, SetupPasswordType type)
        {
            InitializeComponent();
            _parent = parent ?? throw new ArgumentNullException(nameof(parent));
            _type = type ;

            if (type == SetupPasswordType.SET_INITIAL_PASSWORD)
            {
                // Hide old password part
                TextBoxOldPassword.Visibility = Visibility.Hidden ;
                LabelOldPassword.Visibility = Visibility.Hidden ;

                // Disable Cancel button
                ButtonCancel.IsEnabled = false ;
            }
        }

        private async void OnConfirm(object sender, RoutedEventArgs e)
        {

            // If the content of the confirmation is not the same
            if (TextBoxNewPassword1.Password != TextBoxNewPassword2.Password)
            {
                LabelError.Content = "The confirmation of the new password must be the same" ;
                LabelError.Visibility = Visibility.Visible ;
                return ;
            }

            // Call the set password handler
            var request = (_type == SetupPasswordType.SET_INITIAL_PASSWORD) ?
                new SetPassword.Request(TextBoxNewPassword1.Password) :
                new SetPassword.Request(TextBoxOldPassword.Password, TextBoxNewPassword1.Password) ;
            var result = await ServiceCollection.Handle(request) ;

            // Try to open the chest
            var resultLogin = await ServiceCollection.Handle(new OpenChestSession.Request(TextBoxNewPassword1.Password));

            if (result.Success) _parent.GoFromSetupToLogin() ;
            else 
            {
                LabelError.Content = "Wrong old password" ;
                LabelError.Visibility = Visibility.Visible ;
            }
        }

        private void OnCancel(object sender, RoutedEventArgs e)
        {
            _parent.GoFromSetupToMenu();
        }

        // Typing "enter" in one of the box will validate the form
        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter) OnConfirm(sender,e) ;
        } 

        public enum SetupPasswordType { SET_INITIAL_PASSWORD, CHANGE_PASSWORD }
    }
}
