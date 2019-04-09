using Hummingbird.TestFramework;
using Hummingbird.TestFramework.Util;
using Hummingbird.UI;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
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
    /// Interaction logic for ViewJwtGenerator.xaml
    /// </summary>
    [TestFramework.Extensibility.CustomTool(UseGradientColor = true, StartColor = "Navy", EndColor = "Blue", Name = "JWT Generator", Key = "JWTGenerator")]
    [TestFramework.ImageSource(ImageSource = "pack://application:,,,/Hummingbird.CustomTools;component/Images/jwt-generator.png")]

    public partial class ViewJwtGenerator : ModernContent
    {

        ObservableCollection<TokenClaim> JwtPayloadPairs = new ObservableCollection<TokenClaim>();

        public ViewJwtGenerator()
        {
            InitializeComponent();
            foreach (var v in Hummingbird.TestFramework.Util.JsonWebTokenUtility.WellknownClaims)
            {
                JwtPayloadPairs.Add(new TokenClaim()
                {
                    Name = v.Name,
                    Value = v.DefaultValue.Invoke(),
                    ValueType = v.ValueType
                });
            }
            lstPayload.ItemsSource = JwtPayloadPairs;
            foreach (var algo in JwtUtility.JwtSignatureAlgorithms)
            {
                cbAlgorithms.Items.Add(new ComboBoxItem()
                {
                    Content = algo.Key,
                    ToolTip = algo.Value,
                    Tag = algo.Key,
                });
            }

        }


        //used for symmetric algorithms
        string symmetricKey;
        KeyForm keyform;
        string certificatePath;
        string certificatePassword;


        UserControl keyParameterControl = null;

        private void CbAlgorithms_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbAlgorithms.SelectedItem is ComboBoxItem item)
            {
                string key = item.Tag as string;
                RemoveExistingControl();
                if (key.StartsWith("HS"))
                {
                    
                    keyParameterControl = new ControlSymmetricKey(symmetricKey, keyform);
                    borderKeyParemeterContainer.Child = keyParameterControl;
                }
                else //Asymmetric algorithms
                {
                    keyParameterControl = new ControlCertificate(certificatePath, certificatePassword);
                    borderKeyParemeterContainer.Child = keyParameterControl;
                }
            }
        }

        private void RemoveExistingControl()
        {
            if (keyParameterControl != null)
            {
                UpdateKeyParameter();
                borderKeyParemeterContainer.Child = null;
                if (keyParameterControl is IDisposable dis)
                {
                    dis.Dispose();
                }
            }
        }

        private void UpdateKeyParameter()
        {
            if (keyParameterControl is ControlSymmetricKey csk)
            {
                symmetricKey = csk.GetKey();
                keyform = csk.GetKeyForm();
            }
            else if (keyParameterControl is ControlCertificate cc)
            {
                certificatePath = cc.GetCertificatePath();
                certificatePassword = cc.GetCertificatePassword();
            }

        }

        private void BtnGenerateToken_Click(object sender, RoutedEventArgs e)
        {
            string step = null;
            try
            {
                UpdateKeyParameter();
                JwtPayload payload = new JwtPayload();
                foreach (var v in JwtPayloadPairs)
                {
                    step = $"Claim: {v.Name}, Value: {v.Value}";
                    object value;
                    switch (v.ValueType)
                    {
                        case ClaimValueType.Numberic:
                            value = long.Parse(v.Value.ToString());
                            break;
                        case ClaimValueType.Decimal:
                            value = decimal.Parse(v.Value.ToString());
                            break;
                        default:
                            value = v.Value;
                            break;
                    }
                    payload.Add(v.Name, value);
                }
                string algorithm = GetAlgotithm(cbAlgorithms.SelectedItem);
                string token;
                if (algorithm.StartsWith("HS"))
                {
                    string base64Key = GetBase64Key(symmetricKey, keyform);
                    JsonWebTokenUtility.CreateHmacShaToken(base64Key, algorithm, payload, out token);
                }
                else if (algorithm.StartsWith("RS"))
                {
                    var importedCertificate = ImportCertificate(certificatePath, certificatePassword);
                    JsonWebTokenUtility.CreateRsaToken(importedCertificate, algorithm, payload, out token);
                }
                else if (algorithm.StartsWith("ES"))
                {
                    var importedCertificate = ImportCertificate(certificatePath, certificatePassword);
                    JsonWebTokenUtility.CreateEcdsaToken(importedCertificate, algorithm, payload, out token);
                }
                else
                {
                    token = "The given algorithm is not supported.";
                }
                txtJwtToken.Text = token;
            }
            catch(CryptographicException ce)
            {
                ShowMessageBox("Error when doing cryptography", ce.Message, ce.ToString());
            }
            catch (Exception ex)
            {
                ShowMessageBox("Error", "An error has occurred during generating " + step + "\nError Message: " + ex.Message, ex.ToString());
            }
        }

        private X509Certificate2 ImportCertificate(string certificatePath, string certificatePassword)
        {
            X509Certificate2 cert = new X509Certificate2(certificatePath, certificatePassword, X509KeyStorageFlags.PersistKeySet);
            return cert;
        }

        private static string GetBase64Key(string symmetricKey, KeyForm keyform)
        {
            switch (keyform)
            {
                case KeyForm.Base64Encoded:
                    byte[] data = Convert.FromBase64String(symmetricKey);
                    return Base64UrlEncoder.Encode(data);
                case KeyForm.Base64UrlEncoded:
                    return symmetricKey;
                case KeyForm.Hexadecimal:
                    data = GetStringFromHex(symmetricKey);
                    return Base64UrlEncoder.Encode(data);
                default:
                    return Base64UrlEncoder.Encode(symmetricKey);
            }
        }

        private static byte[] GetStringFromHex(string symmetricKey)
        {
            string data = symmetricKey.Replace(" ", "").Replace("-", "").Trim();
            byte[] raw = new byte[data.Length / 2];
            for (int i = 0; i < raw.Length; i++)
            {
                raw[i] = Convert.ToByte(data.Substring(i * 2, 2), 16);
            }
            return raw;
        }

        private string GetAlgotithm(object selectedItem)
        {
            if (selectedItem is ComboBoxItem cbi)
            {
                string algo = cbi.Tag as string;
                return algo;
            }
            else
            {
                throw new KeyNotFoundException("Signature algorithm is not valid");
            }
        }

        private void BtnRemoveClaim_Click(object sender, RoutedEventArgs e)
        {
            if(sender is Button b && b.Tag is TokenClaim v)
            {
                JwtPayloadPairs.Remove(v);
            }
        }

        private void BtnAddClaim_Click(object sender, RoutedEventArgs e)
        {
            JwtPayloadPairs.Add(new TokenClaim { Name = "New Claim", ValueType = ClaimValueType.String });
        }
    }
}
