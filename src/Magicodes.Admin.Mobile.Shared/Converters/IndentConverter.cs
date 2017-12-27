using System;
using System.Globalization;
using Xamarin.Forms;

namespace Magicodes.Admin.Converters
{
    public class IndentConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var resultWithIndent = "";
            if (!(value is string code))
            {
                return resultWithIndent;
            }

            var indentCharacter = (string)parameter;
            foreach (var character in code)
            {
                if (character == '.')
                {
                    resultWithIndent += indentCharacter;
                }
            }

            return resultWithIndent;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
 