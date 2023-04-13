using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace Q_Tech.Converters
{
    public class BoolToImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is sbyte)
            {
                if ((sbyte)value == 1)
                {
                    return new BitmapImage(new Uri("/Recursos/Iconos/check.png", UriKind.Relative));
                }
                else
                {
                    return new BitmapImage(new Uri("/Recursos/Iconos/cross.png", UriKind.Relative));
                }
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
