using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Core.Domain.Session.Pipelines;
using Core.Domain.Session;
using Chest.Core.DependencyInjection;

namespace UI
{
    /// <summary>
    /// Logique d'interaction pour WindowLogin.xaml
    /// </summary>
    public partial class WindowLogin : Window
    {
        private App _parent;
        public WindowLogin(App parent)
        {
            _parent = parent;
            InitializeComponent();
        }

        public WindowLogin () : this((App) Application.Current) {}

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var password = TextBoxPassword.Password;
            tryOpen(password);
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter) return ;

            var password = TextBoxPassword.Password;
            tryOpen(password);
        }

        private async void tryOpen(string password)
        {
            LabelError.Visibility = Visibility.Hidden;
            var result = await ServiceCollection.Handle(new OpenChestSession.Request(password)) ;
            
            if (result.Success)
            {
                _parent.GoFromLoginToMenu ();
            }
            else
            {
                LabelError.Visibility = Visibility.Visible;
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e) => _parent.GoFromLoginToSetup() ;
    }
}
