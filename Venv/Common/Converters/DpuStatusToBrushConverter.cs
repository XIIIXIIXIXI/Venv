using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Media;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Venv.Common.Converters
{
    public class DpuStatusToBrushConverter : IValueConverter
    {
        public Microsoft.UI.Xaml.Media.Brush TrueBrush { get; set; }
        public Microsoft.UI.Xaml.Media.Brush FalseBrush { get; set; }
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            
        var status = value as string;
            switch(status)
            {
                case "Started":
                case "Running":
                    return TrueBrush;
                case "Removed":
                case "Stopped":
                case "Off":
                    return FalseBrush;
                default:
                    return DependencyProperty.UnsetValue;

            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
