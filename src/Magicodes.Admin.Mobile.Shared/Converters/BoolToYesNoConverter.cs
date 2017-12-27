using System;
using System.Globalization;
using Magicodes.Admin.Localization;
using Xamarin.Forms;

namespace Magicodes.Admin.Converters
{
    public class BoolToYesNoConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool booleanValue)
            {
                return booleanValue ? L.Localize("Yes") : L.Localize("No");
            }

            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}