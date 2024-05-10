using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;
using System;


namespace Venv.Common.Converters
{
    public class IntToVisibilityConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            int count = (int)value;
            return count > 0 ? Visibility.Collapsed : Visibility.Visible;
        }
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
