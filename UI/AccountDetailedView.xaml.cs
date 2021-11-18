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
            if (_account.Link is not null) LabelAddress.Content = GetShortVersionOf(_account.Link, 30) ;
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
            // Open the address in a browser
            OpenBrowser.OpenUrlInBrowser(_account.Link) ;
        }

        private void OnPasswordCopy(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText((string) LabelPassword.Content) ;
            SetButtonCopied(ButtonPassword) ;
        }

        private void SetButtonCopied (Button button)
        {
            ButtonName.Content = "Copy" ;
            ButtonAddress.Content = "Copy" ;
            ButtonPassword.Content = "Copy" ;
            button.Content = "Copied" ;
        }

        // Restrict the size of a string to max_size, by replacing the end with " ..." if necessary
        private string GetShortVersionOf(string content, int max_size)
            => (content.Length > max_size) ? (content.Substring(0,max_size-3) + " ...") : content ;
    }
}
