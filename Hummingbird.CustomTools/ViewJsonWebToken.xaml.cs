using Hummingbird.TestFramework;
using Hummingbird.TestFramework.Serialization;
using Hummingbird.TestFramework.Util;
using Hummingbird.UI;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
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
    /// Interaction logic for ViewJsonWebToken.xaml
    /// </summary>
    [TestFramework.Extensibility.CustomTool(UseGradientColor = true, StartColor = "Navy", EndColor = "Blue", Name = "JWT Decoder", Key = "JWTDecoder")]
    [TestFramework.ImageSource(ImageSource = "pack://application:,,,/Hummingbird.CustomTools;component/Images/jwt.png")]
    public partial class ViewJsonWebToken : ModernContent
    {

        public ViewJsonWebToken()
        {
            InitializeComponent();
            txtToken.Text = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxMjM0NTY3ODkwIiwibmFtZSI6IkpvaG4gRG9lIiwiaWF0IjoxNTE2MjM5MDIyfQ.SflKxwRJSMeKKF2QT4fwpMeJf36POk6yJV_adQssw5c";
            Common.SetAvalonEditorSyntax(this.IsDarkTheme, txtHeader, TestFramework.Serialization.DocumentFormat.Json);
            Common.SetAvalonEditorSyntax(this.IsDarkTheme, txtPayload, TestFramework.Serialization.DocumentFormat.Json);
            cbKeyFormat.Items.Add(KeyFormat.Hexadecimal);
            cbKeyFormat.Items.Add(KeyFormat.Base64Encoded);
            cbKeyFormat.Items.Add(KeyFormat.Base64UrlEncoded);
            cbKeyFormat.Items.Add(KeyFormat.OrdinaryString);
            cbKeyFormat.SelectedIndex = 3;
            IdentityModelEventSource.ShowPII = true;
        }


        private void TxtToken_TextChanged(object sender, TextChangedEventArgs e)
        {
            JwtSecurityToken jwt = new JwtSecurityToken(txtToken.Text);
            txtHeader.Text = JsonSerializer.GetInstance(typeof(JsonSerializer)).Serialize(jwt.Header, typeof(JwtHeader));
            txtPayload.Text = JsonSerializer.GetInstance(typeof(JsonSerializer)).Serialize(jwt.Payload, typeof(JwtPayload));
        }

        private void BtnCertificate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                OpenFileDialog ofd = new OpenFileDialog()
                {
                    Filter = "All supported certificate (*.p12,*.pfx)|*.pfx;*.p12",
                    Title = "Select a signing certificate",
                    CheckFileExists = true,
                    AddExtension = true,
                };
                var result = ofd.ShowDialog();
                if (result.HasValue && result.Value)
                {
                    string fname = ofd.FileName;
                    WindowInputPassword wip = new WindowInputPassword();
                    var result2 = wip.ShowDialog();
                    if (result2.HasValue && result2.Value)
                    {
                        string password = wip.Password;
                        certificate = new X509Certificate2(fname, password, X509KeyStorageFlags.PersistKeySet);
                        lblCertificate.Text = GetCertificateInfo(certificate);
                    }
                }
            }catch(Exception ex)
            {
                this.ShowMessageBox("Error loading certificate", "An error occurred when loading the certificate.\n" + ex.Message);
            }
        }

        private string GetCertificateInfo(X509Certificate2 certificate)
        {
            return "SUBJECT: " + certificate.Subject + ", ISSUED BY: " + certificate.Issuer + ", SIGNING ALGORITHM: " + certificate.SignatureAlgorithm.FriendlyName;
        }

        X509Certificate2 certificate = null;


        private void BtnValidate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
                SecurityKey key = null;

                var jwtToken = handler.ReadJwtToken(txtToken.Text);
                string algorithm = jwtToken.SignatureAlgorithm;
                if (algorithm.StartsWith("HS"))
                {
                    KeyFormat keyFormat = (KeyFormat)cbKeyFormat.SelectedItem;
                    string base64key = ViewJwtGenerator.GetBase64Key(txtSecret.Text, keyFormat);
                    key = new SymmetricSecurityKey(Base64UrlEncoder.DecodeBytes(base64key));
                }
                else if (algorithm.StartsWith("RS"))
                {
                    key = new RsaSecurityKey(certificate.GetRSAPrivateKey());
                }
                else if (algorithm.StartsWith("ES"))
                {
                    key = new ECDsaSecurityKey(certificate.GetECDsaPrivateKey());
                }
                else
                {
                    throw new NotSupportedException($"The given algorithm {algorithm} is not yet supported for signature validation");
                }

                handler.ValidateToken(txtToken.Text, new TokenValidationParameters()
                {
                    IssuerSigningKey = key,
                    ValidateActor = false,
                    ValidateIssuer = false,
                    ValidateAudience = false,
                }, out SecurityToken token);
            }
            catch (Exception ex)
            {
                this.ShowInformation("Validation error", "There is an error when validating the signature.\n" + ex.Message);
                return;
            }
            this.ShowInformation("Validation OK", "This Json Web Token is validated.");
        }

    }
}
