using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;
using System;
using System.Diagnostics.CodeAnalysis;

namespace Venv.Common.Converters
{
    [ExcludeFromCodeCoverage]
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
