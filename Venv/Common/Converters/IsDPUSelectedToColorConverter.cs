using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Venv.Common.Converters
{
    public class IsDPUSelectedToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if ((bool)value)
            {
                
                return Application.Current.Resources["SystemControlHighlightAccentBrush"] as Brush;
            }
            return Application.Current.Resources["AncCpTextForeground"] as Brush;
        }
        

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
