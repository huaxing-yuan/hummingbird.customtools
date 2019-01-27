using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Highlighting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Hummingbird.CustomTools
{
    internal static class Common
    {
        internal static void SetAvalonEditorSyntax(bool IsDarkTheme, TextEditor txtTextEditor, TestFramework.Serialization.DocumentFormat docFormat)
        {

            if (IsDarkTheme)
            {
                if (docFormat == TestFramework.Serialization.DocumentFormat.XML)
                {
                    txtTextEditor.SyntaxHighlighting = HighlightingManager.Instance.GetDefinition("XML_DARK");
                }
                else if (docFormat == TestFramework.Serialization.DocumentFormat.Json)
                {
                    txtTextEditor.SyntaxHighlighting = HighlightingManager.Instance.GetDefinition("JS_DARK");
                }
                else if (docFormat == TestFramework.Serialization.DocumentFormat.CSharp)
                {
                    txtTextEditor.SyntaxHighlighting = HighlightingManager.Instance.GetDefinition("CS_DARK");
                }
                else if (docFormat == TestFramework.Serialization.DocumentFormat.SQL)
                {
                    txtTextEditor.SyntaxHighlighting = HighlightingManager.Instance.GetDefinition("SQL_DARK");

                }
                txtTextEditor.TextArea.TextView.LinkTextForegroundBrush = (Brush)txtTextEditor.FindResource("HyperLinkLightBrush");
            }
            else
            {
                if (docFormat == TestFramework.Serialization.DocumentFormat.XML)
                {
                    txtTextEditor.SyntaxHighlighting = HighlightingManager.Instance.GetDefinition("XML_LIGHT");
                }
                else if (docFormat == TestFramework.Serialization.DocumentFormat.Json)
                {
                    txtTextEditor.SyntaxHighlighting = HighlightingManager.Instance.GetDefinition("JS_LIGHT");
                }
                else if (docFormat == TestFramework.Serialization.DocumentFormat.CSharp)
                {
                    txtTextEditor.SyntaxHighlighting = HighlightingManager.Instance.GetDefinition("C#");
                }
                else if (docFormat == TestFramework.Serialization.DocumentFormat.SQL)
                {
                    txtTextEditor.SyntaxHighlighting = HighlightingManager.Instance.GetDefinition("SQL_LIGHT");
                }
                txtTextEditor.TextArea.TextView.LinkTextForegroundBrush = (Brush)txtTextEditor.FindResource("HyperLinkBrush");
            }

        }

    }
}
