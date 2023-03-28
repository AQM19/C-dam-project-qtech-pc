using System;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Security;
using System.Windows.Data;

namespace Q_Tech.Converters
{
    internal class PasswordConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is SecureString secureString)
            {
                IntPtr unmanagedString = IntPtr.Zero;
                try
                {
                    unmanagedString = Marshal.SecureStringToGlobalAllocUnicode(secureString);
                    string password = Marshal.PtrToStringUni(unmanagedString);
                    return new string('*', password.Length); // devuelve un string oculto
                }
                finally
                {
                    Marshal.ZeroFreeGlobalAllocUnicode(unmanagedString);
                }
            }
            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
