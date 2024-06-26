﻿using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml;
using System;
using System.Diagnostics.CodeAnalysis;

namespace Venv.Common.Converters
{
    [ExcludeFromCodeCoverage]
    public class StatusToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var status = value as string;
            var invert = parameter as string == "invert";

            var isVisible = status == "Stopped" || status == "Running" || status == "Started" || status == "Removed";

            //For progress ring
            if (invert) isVisible = !isVisible;

            return isVisible ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
