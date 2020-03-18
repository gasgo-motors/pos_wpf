using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace PosClient.Converters
{
    public class BooleanVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            try
            {
                if (value == DependencyProperty.UnsetValue || value == null) return Visibility.Collapsed;
                bool? res = (bool?)value;
                if (!res.HasValue || !res.Value)
                    return Visibility.Collapsed;
                return Visibility.Visible;
            }
            catch (Exception ex)
            {
                return "";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
