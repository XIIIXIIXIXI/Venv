using Microsoft.UI.Xaml.Data;
using System;
using System.Collections;


namespace Venv.Common.Converters
{
    public class EmptyDPUListToOpacityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var list = value as IList;
            if (list != null && list.Count == 0)
                return 0.4; //Opacity when no DPUs
            return 1.0; 
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
