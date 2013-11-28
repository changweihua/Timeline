using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Windows;
using System.IO;
using System.Windows.Media.Imaging;
using System.Windows.Data;
using System.Threading.Tasks;
using Timeline.ToolClasses;
using Timeline.ScatterViewItem;

namespace Timeline.ScatterViewItem.SubItem
{
    public sealed class ImageScatterViewItem : BaseScatterViewItem
    {
        private ImageSource m_ImageSource;

        public ImageSource ImageSource
        {
            get { return (ImageSource)GetValue(ImageSourceProperty); }
            set { SetValue(ImageSourceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ImageSource.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ImageSourceProperty =
            DependencyProperty.Register("ImageSource", typeof(ImageSource), typeof(ImageScatterViewItem), new UIPropertyMetadata(null));


        static ImageScatterViewItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ImageScatterViewItem), new FrameworkPropertyMetadata(typeof(ImageScatterViewItem)));
        }
        public ImageScatterViewItem()
        {
            InitialCompleted += new RoutedEventHandler(ImageScatterViewItem_InitialComplated);
        }

        void ImageScatterViewItem_InitialComplated(object sender, RoutedEventArgs e)
        {
            if (this.Dispatcher.CheckAccess())
                ImageSource = m_ImageSource;
            Width = ImageSource.Width;
            Height = ImageSource.Height;
            base.BaseScatterViewItem_InitialComplated(sender, e);
        }

        protected override void InitailScatterViewItem()
        {
            new System.Threading.Thread(new System.Threading.ThreadStart(delegate
            {
                System.Windows.Media.Imaging.BitmapFrame bitmapImage = Timeline.ToolClasses.WPFUtil.StringToBitmapImage(SourceModel.FileFullPath) as System.Windows.Media.Imaging.BitmapFrame;

                if (bitmapImage == null) return;
                ///在大于标准像素时 需要进行缩放
                if (bitmapImage.PixelHeight <= ConstClass.NORMAL_IMAGE_HEIGHT && bitmapImage.PixelWidth <= ConstClass.NORMAL_IMAGE_WIDTH)
                {
                    //ImageSource = bitmapImage;
                    //Width = bitmapImage.PixelWidth;
                    //Height = bitmapImage.PixelHeight;
                    m_ImageSource = bitmapImage;

                }
                else
                {
                    double ratio = Math.Min(ConstClass.NORMAL_IMAGE_WIDTH / bitmapImage.PixelWidth, ConstClass.NORMAL_IMAGE_HEIGHT / bitmapImage.PixelHeight);

                    int width = (int)(bitmapImage.PixelWidth * ratio);
                    int height = (int)(bitmapImage.PixelHeight * ratio);

                    /**************对大图片进行压缩,压缩效率有待优化****************/
                    using (System.Drawing.Image drawingImage = System.Drawing.Image.FromStream
                        (File.Open(SourceModel.FileFullPath, FileMode.Open, FileAccess.Read, FileShare.Read)))
                    {
                        MemoryStream ms = new MemoryStream();
                        MakeThumbnail(drawingImage, width, height, "", ms);
                        BitmapFrame bf = BitmapFrame.Create(ms);
                        m_ImageSource = bf;
                    }
                }
                this.Dispatcher.BeginInvoke((Action)delegate
                {
                    RoutedEventArgs args = new RoutedEventArgs(InitialCompletedRouteEvent);
                    RaiseEvent(args);
                }, null);
            })).Start();
        }

        private void MakeThumbnail(System.Drawing.Image originalImage, int width, int height, string mode, Stream stream)
        {
            //System.Drawing.Image originalImage = System.Drawing.Image.FromFile(originalImagePath);
            int towidth = width;
            int toheight = height;
            int x = 0;
            int y = 0;
            int ow = originalImage.Width;
            int oh = originalImage.Height;

            if ((double)originalImage.Width / (double)originalImage.Height > (double)towidth / (double)toheight)
            {
                oh = originalImage.Height;
                ow = originalImage.Height * towidth / toheight;
                y = 0;
                x = (originalImage.Width - ow) / 2;
            }
            else
            {
                ow = originalImage.Width;
                oh = originalImage.Width * height / towidth;
                x = 0;
                y = (originalImage.Height - oh) / 2;
            }

            System.Drawing.Image bitmap = new System.Drawing.Bitmap(towidth, toheight);
            //新建一个画板        
            System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(bitmap);
            //设置高质量插值法        
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;
            //设置高质量,低速度呈现平滑程度 
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            //清空画布并以透明背景色填充   
            g.Clear(System.Drawing.Color.Transparent);
            //在指定位置并且按指定大小绘制原图片的指定部分       
            g.DrawImage(originalImage, new System.Drawing.Rectangle(0, 0, towidth, toheight), new System.Drawing.Rectangle(x, y, ow, oh), System.Drawing.GraphicsUnit.Pixel);
            try
            {
                bitmap.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
            }
            catch (System.Exception e)
            {
                throw e;
            }
            finally
            {
                originalImage.Dispose();
                bitmap.Dispose();
                g.Dispose();
            }
        }

        public override void CleanScatterViewItem()
        {
            ImageSource = null;
            InitialCompleted -= new RoutedEventHandler(ImageScatterViewItem_InitialComplated);
            base.CleanScatterViewItem();
        }
    }
}
