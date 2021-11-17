using Chest.Core;
using Chest.Core.DependencyInjection;
using Core.Domain.Accounts;
using Core.Domain.Accounts.Services;
using Core.Domain.Session.Pipelines;
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

namespace UI
{
    /// <summary>
    /// Logique d'interaction pour Menu.xaml
    /// </summary>
    public partial class Menu : Page
    {

        private Dictionary<AccountView,ChestAccount> _accounts = new() ;

        private AccountView _selected ;
        private readonly App _parent;

        public AccountView Selected {
            get => _selected;
            set 
            {
                _selected = value;

                ButtonEdit.IsEnabled = true;
                ButtonDelete.IsEnabled = true;

                foreach (var accountView in _accounts.Keys)
                {
                    accountView.Root.Background = (SolidColorBrush)new BrushConverter().ConvertFrom("#01FFFFFF");
                }
                _selected.Root.Background = Brushes.LightGray ;
            } 
        }

        public Menu(App parent)
        {
            InitializeComponent();
            initAccountsView();
            this._parent = parent ?? throw new ArgumentNullException(nameof(parent));
        }

        public async void initAccountsView ()
        {
            var result = await ServiceCollection.Handle(new InitMenu.Request());

            if (result.Accounts.Length == 0)
            {
                StackAccounts.Children.Add(new NoAccountView());
                return ;
            }

            foreach (var account in result.Accounts)
            {
                var view = new AccountView(this,account.Name) ;
                _accounts.Add(view, account) ;
                StackAccounts.Children.Add(view);
            }
        }

        public void OnViewClicked(AccountView view) => Selected = view;

        public void OnSeeClicked (AccountView view)
        {
            _parent.Window.Content = new AccountDetailedView(_accounts[view],this) ;
        }

        public void Return () => _parent.Window.Content = this ;

        private void ButtonEdit_Click(object sender, RoutedEventArgs e)
        {
            if (_selected is null) return;

            var account = _accounts[_selected];
            // Open a new page to edit the account
            _parent.GetFromMenuToEditAccount(account) ;
        }

        private void ButtonDelete_Click (object sender, RoutedEventArgs e)
        {
            if (_selected is null) return;
            var account = _accounts[_selected];

            MessageBoxResult result = MessageBox.Show(
                $"Are you sure you want to delete {account.Name} account ? ",
                 "Delete account",
                MessageBoxButton.OKCancel
            );
            if (result == MessageBoxResult.OK) deleteAccount(account.Id) ;
        }

        private async void deleteAccount (Guid accountId)
        {
            var accountProvider = ServiceCollection.GetInstance().GetScope<IAccountProvider>() ;
            await accountProvider.DeleteAccount(accountId) ;
            _parent.UpdateMenu () ;
        }

        private void AddItem (object sender, MouseEventArgs e) => _parent.GoFromMenuToNewAccount() ;
        private void OnPlusME (object sender, MouseEventArgs e) => LabelPlus.Foreground = Brushes.LightGray;
        private void OnPlusML(object sender, MouseEventArgs e) => LabelPlus.Foreground = Brushes.Black ;

        private void ChangePassword_click(object sender, RoutedEventArgs e) => _parent.GoFromMenuToSetup();
    }
}
