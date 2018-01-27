using System;
using System.Globalization;
using System.Windows.Data;

namespace MedicalDiamondSearch.Wpf.Converters
{
    public class StringWithLabelConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(parameter is string stringParameter)) return null;
            return stringParameter + ": " + value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
