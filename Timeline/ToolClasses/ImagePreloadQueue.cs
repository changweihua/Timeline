using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.IO;

namespace ShiningMeeting.ToolClasses
{
    public class ImagePreloadQueue
    {
        #region 辅助类别

        public int ZoomWidth
        {
            set;
            get;
        }

        public int ZoomHeight
        {
            set;
            get;
        }


        private class ImageQueueInfo
        {
            public Image Image { get; set; }
            public string Url { get; set; }
        }

        #endregion

        private AutoResetEvent _autoEvent;

        private Queue<ImageQueueInfo> _queues;

        private readonly object _syncObject;

        public ImagePreloadQueue()
        {
            ZoomWidth = 160;
            ZoomHeight = 100;
            _queues = new Queue<ImageQueueInfo>();
            _autoEvent = new AutoResetEvent(true);
            _syncObject = new object();
            Thread t = new Thread(new ThreadStart(PreloadZoomImage));
            t.Name = "下载图片";
            t.IsBackground = true;
            t.Start();
        }

        private void PreloadZoomImage()
        {
            while (true)
            {
                ImageQueueInfo t = null;
                lock (_syncObject)
                {
                    if (_queues.Count > 0)
                    {
                        t = _queues.Dequeue();
                    }
                }
                if (t != null)
                {
                    Uri uri = new Uri(t.Url);
                    BitmapSource bitmapSource = null;
                    try
                    {
                        if ("file".Equals(uri.Scheme, StringComparison.CurrentCultureIgnoreCase) && File.Exists(t.Url))
                        {
                            FileStream fs = File.OpenRead(t.Url);
                            BitmapImage bitmapImage = new BitmapImage();
                            bitmapImage.BeginInit();
                            bitmapImage.StreamSource = fs;
                            bitmapImage.EndInit();
                            if (bitmapImage.PixelWidth <= ZoomWidth && bitmapImage.PixelHeight < ZoomHeight)
                            {
                                bitmapSource = ImageConvertOperations.GetStreamBitmapSourceFromPath(t.Url.ToString());
                            }
                            else
                            {
                                bitmapSource = ImageConvertOperations.GetBitmapSourceFromDrawImage(ImageConvertOperations.ZoomImageFromStream(fs, ZoomWidth, ZoomHeight));
                            }
                            fs.Close();
                            fs.Dispose();
                        }
                        if (bitmapSource != null)
                        {
                            if (bitmapSource.CanFreeze) bitmapSource.Freeze();
                            t.Image.Dispatcher.BeginInvoke(new Action<ImageQueueInfo, BitmapImage>((i, bmp) =>
                            {
                                i.Image.Source = bmp;
                            }), new Object[] { t, bitmapSource });
                        }
                    }
                    catch (Exception e)
                    {
                        continue;
                    }
                }
                if (_queues.Count > 0) continue;
                _autoEvent.WaitOne();
            }
        }

        public void Queue(Image img, string url)
        {
            if (string.IsNullOrEmpty(url)) return;
            lock (_syncObject)
            {
                _queues.Enqueue(new ImageQueueInfo { Url = url, Image = img });
                _autoEvent.Set();
            }
        }
    }
}
