using Hummingbird.TestFramework.Serialization;
using Hummingbird.UI;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Cryptography;
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
    [TestFramework.Extensibility.CustomTool(UseGradientColor = true, StartColor = "Navy", EndColor = "Blue", Name = "Json Web Token", Key ="JWT")]
    [TestFramework.ImageSource( ImageSource = "pack://application:,,,/Hummingbird.CustomTools;component/Images/jwt.png")]
    public partial class ViewJsonWebToken : ModernContent
    {
        public ViewJsonWebToken()
        {
            InitializeComponent();
            txtToken.Text = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxMjM0NTY3ODkwIiwibmFtZSI6IkpvaG4gRG9lIiwiaWF0IjoxNTE2MjM5MDIyfQ.SflKxwRJSMeKKF2QT4fwpMeJf36POk6yJV_adQssw5c";
            Common.SetAvalonEditorSyntax(this.IsDarkTheme, txtHeader, TestFramework.Serialization.DocumentFormat.Json);
            Common.SetAvalonEditorSyntax(this.IsDarkTheme, txtPayload, TestFramework.Serialization.DocumentFormat.Json);
        }

        bool deserialzing = false;
        bool serializaing = false;
        private void TxtHeader_TextChanged(object sender, EventArgs e)
        {

        }

        private void TxtPayload_TextChanged(object sender, EventArgs e)
        {

        }


        private void TxtToken_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!deserialzing)
            {
                serializaing = true;
                JwtSecurityToken jwt = new JwtSecurityToken(txtToken.Text);
                txtHeader.Text = JsonSerializer.GetInstance(typeof(JsonSerializer)).Serialize(jwt.Header, typeof(JwtHeader));
                txtPayload.Text = JsonSerializer.GetInstance(typeof(JsonSerializer)).Serialize(jwt.Payload, typeof(JwtPayload));
                serializaing = false;
            }
        }

        private void TxtSecret_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}
