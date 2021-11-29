using Chest.Core.DependencyInjection;
using Core.Domain.Accounts;
using Core.Domain.PasswordHash.Pipelines;
using Core.Domain.Session;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
    /// Logique d'interaction pour AccountDetailedView.xaml
    /// </summary>
    public partial class AccountDetailedView : Page
    {
        private readonly ChestAccount _account;
        private readonly Menu _parent;

        public AccountDetailedView(ChestAccount account, Menu parent)
        {
            InitializeComponent();
            this._account = account ?? throw new ArgumentNullException(nameof(account));
            this._parent = parent ?? throw new ArgumentNullException(nameof(parent));

            // Get the clear password
            var request = new DecryptPassword.Request(_account) ;
            var result = ServiceCollection.Handle(request).GetAwaiter().GetResult() ;

            // Update the fields
            LabelName.Content = _account.Name ;
            LabelPassword.Content = result.ClearPassword ;

            // Link field is collapsed if there is no link
            if (string.IsNullOrEmpty(_account.Link)) GridAddress.Visibility = Visibility.Collapsed;
            else LabelAddress.Content = GetShortVersionOf(_account.Link, 30);

            // User field is collapsed if there is no link
            if (string.IsNullOrEmpty(_account.Username)) GridUser.Visibility = Visibility.Collapsed;
            else LabelUser.Content = _account.Username;
        }

        private void OnBackClick(object sender, RoutedEventArgs e)
        {
            _parent.Return() ;
        }

        private void OnNameCopy(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText((string) LabelName.Content) ;
            SetButtonCopied(ButtonName) ;
        }

        private void OnAddressCopy(object sender, RoutedEventArgs e)
        {
            if (_account.Link is not null) OpenBrowser.OpenUrlInBrowser(_account.Link) ;
        }

        private void OnPasswordCopy(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText((string) LabelPassword.Content) ;
            SetButtonCopied(ButtonPassword) ;
        }

        // Restrict the size of a string to max_size, by replacing the end with " ..." if necessary
        private string GetShortVersionOf(string content, int max_size)
            => (content.Length > max_size) ? (content.Substring(0,max_size-3) + " ...") : content ;

        private void OnUserCopy(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText((string) this.LabelUser.Content );
            SetButtonCopied(ButtonUser);
        }

        private void SetButtonCopied(Button button)
        {
            ButtonName.Content = "Copy";
            ButtonPassword.Content = "Copy";
            ButtonUser.Content = "Copy";
            button.Content = "Copied";
        }
    }
}
