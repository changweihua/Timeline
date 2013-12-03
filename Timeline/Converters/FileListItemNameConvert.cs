using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.IO;

namespace ShiningMeeting.Converters
{
    public class FileListItemNameConvert:IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string a = value.ToString().Substring(0, value.ToString().IndexOf('.'));
            if (value.ToString().Substring(0, value.ToString().IndexOf('.')).Length > 15)
            {
                return value.ToString().Substring(0, 12) + "..." + new FileInfo(value.ToString()).Extension;
            }
            else
                return value.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
