using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Globalization;
using System.Windows.Media.Imaging;
using System.Windows.Controls;

namespace Timeline.Converters
{
    public class VideoScatterViewItem_Boolean_ImageSource_Converter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) 
        {
            bool bValue = (bool)value;
            Image image = new System.Windows.Controls.Image();

            switch (bValue)
            {
                case true:
                    image.Source = new BitmapImage(new Uri("pack://application:,,,/Icons;component/wp/light/appbar.control.stop.png", UriKind.RelativeOrAbsolute));
                    break;
                case false:
                    image.Source = new BitmapImage(new Uri("pack://application:,,,/Icons;component/wp/light/appbar.control.play.png", UriKind.RelativeOrAbsolute));
                    break;
            }
            return image;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) 
        {
            throw new NotImplementedException();
        }
    }
}
