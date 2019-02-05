using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace OCMApp.Internal
{
    public class FileListConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string retVal = null;
            string val = value.ToString();
            if (!string.IsNullOrWhiteSpace(val))
            {
                foreach (var item in val.Split(';'))
                {
                    if (retVal != null)
                        retVal += Environment.NewLine;
                    retVal += item.Trim();
                }
            }
            return retVal;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
