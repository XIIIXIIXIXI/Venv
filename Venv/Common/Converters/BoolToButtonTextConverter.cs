using Microsoft.UI.Xaml.Data;
using System;
using System.Diagnostics.CodeAnalysis;


namespace Venv.Common.Converters
{
    [ExcludeFromCodeCoverage]
    public class BoolToButtonTextConverter :IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return (bool)value ? "{ThemeResource AncCpTextForeground}" : "{ThemeResource DynTextDisabled}";
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
