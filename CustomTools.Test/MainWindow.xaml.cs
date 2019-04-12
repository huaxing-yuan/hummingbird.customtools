using Hummingbird.UI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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

namespace CustomTools.Test
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : BasicWindow
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void ModernLink_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("https://huaxing-yuan.github.io/hummingbird.doc/html/fb6150ac-4773-4457-afc2-e203cf917dc8.htm");
        }

        private void ModernLink_Click_1(object sender, RoutedEventArgs e)
        {
            Process.Start("https://github.com/huaxing-yuan/hummingbird.customtools");
        }
    }
}
