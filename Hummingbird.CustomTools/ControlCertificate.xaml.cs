using Microsoft.Win32;
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
    /// Interaction logic for ControlSymmetricKey.xaml
    /// </summary>
    public partial class ControlCertificate : UserControl
    {
        public ControlCertificate(string certPath, string certPassword)
        {
            InitializeComponent();
            txtCertificatePath.Text = certPath;
            txtCertificatePassword.Password = certPassword;

        }

        internal string GetCertificatePath()
        {
            return txtCertificatePath.Text;
        }

        internal string GetCertificatePassword()
        {
            return txtCertificatePassword.Password;
        }

        private void BtnOpenCertificate_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog()
            {
                Filter = "Certificate (*.p12, *.pfx)|*.pfx;*.p12",
                CheckFileExists = true,
            };
            var result = ofd.ShowDialog();
            if (result.HasValue && result.Value)
            {
                txtCertificatePath.Text = ofd.FileName;
            }

        }
    }
}
