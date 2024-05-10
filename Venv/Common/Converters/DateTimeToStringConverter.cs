using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;
using System;
using System.Diagnostics.CodeAnalysis;


namespace Venv.Common.Converters
{
    [ExcludeFromCodeCoverage]
    class DateTimeToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is DateTime dateTime)
            {
                return dateTime.ToString("dd/MM/yyyy");
            }
            return "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            if (DateTime.TryParseExact((string)value, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime dateTime))
            {
                return dateTime;
            }
            return DependencyProperty.UnsetValue;
        }
    }
}
