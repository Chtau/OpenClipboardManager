using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace OCMApp.Internal
{
    public class VisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool reverse = false;
            if (parameter != null)
            {
                if (bool.TryParse(parameter.ToString(), out bool param))
                    reverse = param;
            }
            if (bool.TryParse(value.ToString(), out bool show))
            {
                if (reverse)
                    show = !show;
                if (show)
                    return System.Windows.Visibility.Visible;
                else
                    return System.Windows.Visibility.Collapsed;
            }
            if (reverse)
                return System.Windows.Visibility.Collapsed;
            else
                return System.Windows.Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
