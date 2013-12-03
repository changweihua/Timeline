using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Globalization;
using System.Windows;

namespace Timeline.Converters
{
    class VideoScatterViewItem_Boolean_Visibility_Converter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool bValue = (bool)value;
            Visibility visibility = Visibility.Collapsed;

            switch (bValue)
            {
                case true:
                    visibility = Visibility.Hidden;
                    break;
                case false:
                    visibility = Visibility.Visible;
                    break;
            }
            return visibility;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
