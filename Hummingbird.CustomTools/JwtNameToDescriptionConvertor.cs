using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Hummingbird.CustomTools
{
    public class JwtNameToDescriptionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string name = value?.ToString();
            if(name != null && JwtUtility.JwtPayloadDescription.ContainsKey(name))
            {
                return JwtUtility.JwtPayloadDescription[name];
            }
            else
            {
                return string.Empty;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
