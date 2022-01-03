using Chest.Core;
using Chest.Core.DependencyInjection;
using Chest.Core.Infrastructure;
using Core.Domain.Accounts.Pipelines;
using Core.Domain.Generator.Pipelines;
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
    /// Logique d'interaction pour PagePassword.xaml
    /// </summary>
    public partial class PagePassword : Page
    {
        private readonly PageAccountInfos _parent;

        public string Password { get; private set; } = null;

        public int Length
        {
            get
            {
                return Convert.ToInt32(SliderLength.Value);
            }

            set
            {
                SliderLength.Value = value;
            }
        }

        public PagePassword (PageAccountInfos parent, string password) : this (parent)
        {
            if (password is not null) LabelPassword.Content = password ;
        }

        public PagePassword (PageAccountInfos parent)
        {
            InitializeComponent();
            this._parent = parent ?? throw new ArgumentNullException(nameof(parent));
        }

        private async void ButtonGenerate_Click(object sender, RoutedEventArgs e)
        {

            // Get the length of the password
            int length = Convert.ToInt32(SliderLength.Value);

            // Check that the length is in the limits
            int min_length = 3;
            int max_length = 20;
            length = length > max_length ? max_length : length;
            length = length < min_length ? min_length : length;

            // Get the required letters
            string[] required = TextBoxRequired.Text.Select(Char.ToString).ToArray();

            // Build the request
            var request = new GeneratePassword.Request(
                length,
                RadioUpper_case.IsChecked.Value,
                RadioLower_case.IsChecked.Value,
                RadioNumbers.IsChecked.Value,
                RadioSymbols.IsChecked.Value,
                required
            );

            // Generate the password
            var result = await ServiceCollection.Handle(request);

            // Display the result
            Password = result.Password;
            update();

            // Update the copy button
            ButtonCopy.IsEnabled = true;
            ButtonCopy.Content = "Copy";
        }

        private void update()
        {
            // Update the displayed password
            LabelLength.Content = "Length : " + Length.ToString();
            LabelPassword.Content = Password ?? "Password";
        }

        private void setInfoMsg(bool visible, string msg = null)
        {
            if (msg is not null) LabelInfo.Content = msg;
            LabelInfo.Visibility = visible ? Visibility.Visible : Visibility.Hidden;
        }

        private void SliderLength_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            update();
        }

        private void OnSubmit(object sender, RoutedEventArgs e) => _parent.ReturnPassword(Password) ;

        private void OnCancel(object sender, RoutedEventArgs e)=> _parent.Return() ;

        private void ButtonCopy_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(Password);
            ButtonCopy.Content = "Copied";
            ButtonCopy.IsEnabled = false;
        }
    }
}
