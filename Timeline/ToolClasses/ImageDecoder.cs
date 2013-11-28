using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media.Imaging;

namespace ShiningMeeting.ToolClasses
{
    public static class ImageDecoder
    {
        public static readonly DependencyProperty SourceProperty;

        // Using a DependencyProperty as the backing store for ImagePreloadSolve.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ImagePreloadSolveProperty;

        public static string GetSource(Image image)
        {
            if (image == null)
            {
                throw new ArgumentNullException("Image");
            }
            return (string)image.GetValue(ImageDecoder.SourceProperty);
        }
        public static void SetSource(Image image, string value)
        {
            if (image == null)
            {
                throw new ArgumentNullException("Image");
            }
            image.SetValue(ImageDecoder.SourceProperty, value);
        }

        public static ImagePreloadQueue GetImagePreloadSolve(Image obj)
        {
            return (ImagePreloadQueue)obj.GetValue(ImagePreloadSolveProperty);
        }

        public static void SetImagePreloadSolve(Image obj, ImagePreloadQueue value)
        {
            obj.SetValue(ImagePreloadSolveProperty, value);
        }

        static ImageDecoder()
        {
            ImageDecoder.ImagePreloadSolveProperty = DependencyProperty.RegisterAttached("ImagePreloadSolve", typeof(ImagePreloadQueue),
                typeof(ImageDecoder), new UIPropertyMetadata(null, OnImagePreloadSolveChanged));
            ImageDecoder.SourceProperty = DependencyProperty.RegisterAttached("Source", typeof(string), typeof(ImageDecoder),
                new PropertyMetadata(new PropertyChangedCallback(ImageDecoder.OnSourceWithSourceChanged)));
        }

        static void OnImagePreloadSolveChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            ImagePreloadQueue imagePreloadSolve = e.NewValue as ImagePreloadQueue;
            string url = ImageDecoder.GetSource((Image)o);
            if (imagePreloadSolve != null && !string.IsNullOrEmpty(url))
            {
                imagePreloadSolve.Queue((Image)o, url);
            }
        }

        private static void OnSourceWithSourceChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            ImagePreloadQueue imagePreloadSolve = ImageDecoder.GetImagePreloadSolve((Image)o);
            if (imagePreloadSolve != null)
            {
                imagePreloadSolve.Queue((Image)o, (string)e.NewValue);
            }
        }
    }
}
