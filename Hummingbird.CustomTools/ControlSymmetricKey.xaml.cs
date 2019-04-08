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
    public partial class ControlSymmetricKey : UserControl
    {
        public ControlSymmetricKey(string symmetricKey, KeyForm keyform)
        {
            InitializeComponent();
            txtSecretKey.Text = symmetricKey;
            cbKeyForm.Items.Add(KeyForm.Hexadecimal);
            cbKeyForm.Items.Add(KeyForm.Base64Encoded);
            cbKeyForm.Items.Add(KeyForm.Base64UrlEncoded);
            cbKeyForm.Items.Add(KeyForm.OrdinaryString);
            cbKeyForm.SelectedItem = keyform;
        }

        internal string GetKey()
        {
            return txtSecretKey.Text.Trim();
        }

        internal KeyForm GetKeyForm()
        {
            return (KeyForm)cbKeyForm.SelectedItem;
        }
    }
}
