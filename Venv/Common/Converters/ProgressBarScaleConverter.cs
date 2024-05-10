using Microsoft.UI.Xaml.Data;
using System;
using System.Diagnostics.CodeAnalysis;


namespace Venv.Common.Converters
{
    [ExcludeFromCodeCoverage]
    public class ProgressBarScaleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is double cpuUsage)
            {
                double maxWidth = 600;
                return maxWidth * (cpuUsage / 100);
            }

            return 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
