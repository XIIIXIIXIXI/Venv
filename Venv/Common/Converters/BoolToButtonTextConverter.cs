using Microsoft.UI.Xaml.Data;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
