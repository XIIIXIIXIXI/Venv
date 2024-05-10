using Microsoft.UI.Xaml.Data;
using System;
using System.Diagnostics.CodeAnalysis;


namespace Venv.Common.Converters
{
    [ExcludeFromCodeCoverage]
    public class IsSelectedToOpacityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return (bool)value ? 1.0 : 0.3; 
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException(); 
        }
    }
}