using Hummingbird.UI;
using ICSharpCode.AvalonEdit;
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
using Hummingbird.TestFramework.Serialization;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Search;
using System.Xml;
using System.IO;
using Newtonsoft.Json.Linq;

namespace Hummingbird.CustomTools
{
    /// <summary>
    /// Interaction logic for ViewDocFormatter.xaml
    /// </summary>
    [TestFramework.Extensibility.CustomTool(UseGradientColor = true, StartColor = "OrangeRed", EndColor = "Orange", Name = "Document Formatter", Key = "DOCFORMAT")]
    [TestFramework.ImageSource(ImageSource = "pack://application:,,,/Hummingbird.CustomTools;component/Images/indent.png")]
    public partial class ViewDocFormatter : ModernContent
    {
        private bool leftCtrlDown;

        public ViewDocFormatter()
        {
            InitializeComponent();
            SearchPanel.Install(txtDocument);
            Common.SetAvalonEditorSyntax(this.IsDarkTheme, txtDocument, TestFramework.Serialization.DocumentFormat.XML);
        }


        private void TxtFormatXML_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string formattedXML = FormatXmlDocument(txtDocument.Text);
                txtDocument.Text = formattedXML;
                Common.SetAvalonEditorSyntax(this.IsDarkTheme, txtDocument, TestFramework.Serialization.DocumentFormat.XML);
            }
            catch(XmlException xmlex)
            {
                if (xmlex.LineNumber > 0)
                {
                    var line = txtDocument.Document.GetLineByNumber(xmlex.LineNumber);
                    txtDocument.TextArea.Caret.Offset = line.Offset + xmlex.LinePosition;
                    txtDocument.TextArea.Caret.BringCaretToView();
                    
                }
                this.ShowMessageBox("Error occurred when formatting XML document", xmlex.Message);
            }
            catch(Exception ex)
            {
                this.ShowMessageBox("Error occurred when formatting XML document", ex.Message);
            }
        }

        private string FormatXmlDocument(string xml)
        {
            string result = "";

            using (MemoryStream mStream = new MemoryStream())
            {
                using (XmlTextWriter writer = new XmlTextWriter(mStream, Encoding.Unicode))
                {
                    XmlDocument document = new XmlDocument();


                    // Load the XmlDocument with the XML.
                    document.LoadXml(xml);

                    writer.Formatting = Formatting.Indented;

                    // Write the XML into a formatting XmlTextWriter
                    document.WriteContentTo(writer);
                    writer.Flush();
                    mStream.Flush();

                    // Have to rewind the MemoryStream in order to read
                    // its contents.
                    mStream.Position = 0;

                    // Read MemoryStream contents into a StreamReader.
                    StreamReader sReader = new StreamReader(mStream);

                    // Extract the text from the StreamReader.
                    string formattedXml = sReader.ReadToEnd();

                    result = formattedXml;


                }
            }

            return result;
        }

        private void TxtFormatJSON_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string formattedJson = FormatJsonDocument(txtDocument.Text);
                txtDocument.Text = formattedJson;
                Common.SetAvalonEditorSyntax(this.IsDarkTheme, txtDocument, TestFramework.Serialization.DocumentFormat.Json);
            }catch(Exception ex)
            {
                this.ShowMessageBox("Error occurred when formatting JSON document", ex.Message);
            }
        }

        private string FormatJsonDocument(string text)
        {
            JToken token = JToken.Parse(text);
            var prettyFormatted = token.ToString(Newtonsoft.Json.Formatting.Indented);
            return prettyFormatted;
        }

        private void BtnZoomIn_Click(object sender, RoutedEventArgs e)
        {
            ZoomIn();
        }

        private void BtnZoomOut_Click(object sender, RoutedEventArgs e)
        {
            ZoomOut();
        }


        int index = 5;
        private void ZoomIn()
        {
            if(index < fontSizes.Length - 1)
            {
                txtDocument.FontSize = fontSizes[++index];
            }
        }

        private void ZoomOut() {
            if (index > 0)
            {
                txtDocument.FontSize = fontSizes[--index];
            }
        }

        private double[] fontSizes = { 6, 7, 8, 9, 10, 12, 14, 16, 18, 20, 24, 28, 32, 36, 40, 48, 56, 72 };
        

        private void TxtDocument_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (leftCtrlDown)
            {
                if (e.Delta > 0)
                {
                    ZoomIn();
                }
                else
                {
                    ZoomOut();
                }
                e.Handled = true;
            }
        }

        private void TxtDocument_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.LeftCtrl)
            {
                leftCtrlDown = true;
            }
        }

        private void TxtDocument_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.LeftCtrl)
            {
                leftCtrlDown = false;
            }
        }

            
    }


}
