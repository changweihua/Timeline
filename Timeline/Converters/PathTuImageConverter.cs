using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using ShiningMeeting.ToolClasses;
using System.Windows.Media.Imaging;

namespace ShiningMeeting.Converters
{
    public class PathTuImageConverter : IValueConverter
    {
        private static readonly int ZoomWidth = 160, ZoomHeight = 100;

        private static readonly double _dot = ZoomWidth * 1.0 / ZoomHeight;

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            BitmapImage bitmapImage = new BitmapImage(new Uri(value.ToString(), UriKind.RelativeOrAbsolute));
            if (bitmapImage.PixelWidth <= ZoomWidth && bitmapImage.PixelHeight < ZoomHeight)
            {
                bitmapImage = null;
                return ImageConvertOperations.GetStreamBitmapSourceFromPath(value.ToString());
            }
            else
            {
                //double dot = bitmapImage.PixelWidth * 1.0 / bitmapImage.PixelHeight;
                //int zoomW, zoomH;
                //if (dot < _dot)
                //{
                //    zoomW = ZoomWidth;
                //    zoomH = (int)(ZoomWidth * dot);
                //}
                //else
                //{
                //    zoomH = ZoomHeight;
                //    zoomW = (int)(zoomH / dot);
                //}
                bitmapImage = null;
                return ImageConvertOperations.GetBitmapSourceFromDrawImage(ImageConvertOperations.ZoomImageFromFile(value.ToString(), ZoomWidth, ZoomHeight));
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
