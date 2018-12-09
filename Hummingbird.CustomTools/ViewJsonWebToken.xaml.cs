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
    /// Interaction logic for ViewJsonWebToken.xaml
    /// </summary>
    [TestFramework.Extensibility.CustomTool(UseGradientColor = true, StartColor = "Navy", EndColor = "Blue", Name = "Json Web Token", Key ="JWT")]
    [TestFramework.ImageSource( ImageSource = "pack://application:,,,/Hummingbird.CustomTools;component/Images/jwt.png")]
    public partial class ViewJsonWebToken : ModernContent
    {
        public ViewJsonWebToken()
        {
            InitializeComponent();
        }

    }
}
