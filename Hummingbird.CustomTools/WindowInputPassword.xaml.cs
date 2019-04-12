using Hummingbird.UI;
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

namespace Hummingbird.CustomTools
{
    /// <summary>
    /// Interaction logic for WindowInputPassword.xaml
    /// </summary>
    public partial class WindowInputPassword : BasicWindow
    {
        public WindowInputPassword()
        {
            InitializeComponent();
        }


        public string Password { get; private set; }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Password = txtPassword.Password;
            this.DialogResult = true;
        }
    }
}
