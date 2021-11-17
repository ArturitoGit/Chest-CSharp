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
    /// Logique d'interaction pour AccountView.xaml
    /// </summary>
    public partial class AccountView : UserControl
    {
        private Menu _menu;

        public AccountView(Menu menu)
        {
            _menu = menu ?? throw new System.ArgumentException(nameof(menu));
            InitializeComponent();
        }

        public AccountView (Menu menu, string name) : this(menu)
        { 
            Name.Content = name ;
        }

        public void OnMouseDown (object sender, MouseButtonEventArgs e)
        {
            _menu.OnViewClicked(this);
        }

        public void OnMouseEnter (object sender, MouseEventArgs e)
        {
            Root.Background = Brushes.LightGray ;
        }

        public void OnMouseLeave (object sender, MouseEventArgs a)
        {
            if (_menu.Selected == this) return ;
            Root.Background = (SolidColorBrush) new BrushConverter().ConvertFrom("#01FFFFFF");

        }

        private void See_Click(object sender, RoutedEventArgs e)
        {
            _menu.OnSeeClicked(this);
        }
    }
}
