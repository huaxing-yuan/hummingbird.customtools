using Hummingbird.TestFramework.Util;
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
        public ControlSymmetricKey(string symmetricKey, KeyFormat keyform)
        {
            InitializeComponent();
            txtSecretKey.Text = symmetricKey;
            cbKeyForm.Items.Add(KeyFormat.Hexadecimal);
            cbKeyForm.Items.Add(KeyFormat.Base64Encoded);
            cbKeyForm.Items.Add(KeyFormat.Base64UrlEncoded);
            cbKeyForm.Items.Add(KeyFormat.OrdinaryString);
            cbKeyForm.SelectedItem = keyform;
        }

        internal string GetKey()
        {
            return txtSecretKey.Text.Trim();
        }

        internal KeyFormat GetKeyForm()
        {
            return (KeyFormat)cbKeyForm.SelectedItem;
        }
    }
}
