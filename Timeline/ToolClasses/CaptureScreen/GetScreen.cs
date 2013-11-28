using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Drawing;
using Microsoft.Win32;
using System.Windows.Media;
using System.IO;

namespace ShiningMeeting.ToolClasses.CaptureScreen
{
    public class GetScreen
    {
        private static Bitmap screenSnapshot;

        public static event Action SaveComplete;

        static GetScreen()
        {
            screenSnapshot = HelperMethods.GetScreenSnapshot();
            var bmp = screenSnapshot.ToBitmapSource();
            bmp.Freeze();
        }

        /// <summary>
        /// 保存截图
        /// </summary>
        public static void SaveScreen(BitmapSource bmp)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Image Files (*.jpg)|*.jpg|(*.bmp)|*.bmp|(*.png)|*.png|All Files|*.*";
            saveFileDialog.DefaultExt = "jpg";
            saveFileDialog.RestoreDirectory = true;
            if (saveFileDialog.ShowDialog().GetValueOrDefault() != true)
            {
                return;
            }
            string fileName = saveFileDialog.FileName;

            DrawingVisual drawingVisual = new DrawingVisual();
            DrawingContext drawingContext = drawingVisual.RenderOpen();
            int width = Convert.ToInt32(bmp.Width);
            int height = Convert.ToInt32(bmp.Height);
            Rect rectangle = new Rect(0, 0, width, height);
            FormatConvertedBitmap newFormatedBitmapSource = new FormatConvertedBitmap();
            newFormatedBitmapSource.BeginInit();
            newFormatedBitmapSource.Source = bmp;
            newFormatedBitmapSource.EndInit();
            drawingContext.DrawImage(newFormatedBitmapSource, rectangle);
            drawingContext.Close();

            // 利用RenderTargetBitmap对象，以保存图片
            RenderTargetBitmap renderBitmap = new RenderTargetBitmap(width, height, 96, 96, PixelFormats.Pbgra32);
            renderBitmap.Render(drawingVisual);

            // 利用JpegBitmapEncoder，对图像进行编码，以便进行保存
            JpegBitmapEncoder encoder = new JpegBitmapEncoder();
            encoder.QualityLevel = 100; // 设置jpeg的质量值
            encoder.Frames.Add(BitmapFrame.Create(renderBitmap));
            // 保存文件
            FileStream fileStream = new FileStream(fileName, FileMode.Create, FileAccess.ReadWrite);
            encoder.Save(fileStream);
            // 关闭文件流
            fileStream.Close();

            if (SaveComplete != null)
                SaveComplete();
        }

        /// <summary>
        /// 获得指定区域的截图
        /// </summary>
        public static BitmapSource GetAssignedScreen(int x, int y, int width, int height)
        {
            BitmapSource FullScreen = GetScreen.CopyFromScreenSnapshot();
            CroppedBitmap cb = new CroppedBitmap(FullScreen,
                           new Int32Rect(x, y, width, height));

            return cb;
        }

        /// <summary>
        /// 获得全屏截图
        /// </summary>
        /// <returns></returns>
        public static BitmapSource CopyFromScreenSnapshot()
        {

            Rectangle destRect = new Rectangle(0, 0, (int)SystemParameters.PrimaryScreenWidth, (int)SystemParameters.PrimaryScreenHeight);

            var bitmap = new Bitmap((int)SystemParameters.PrimaryScreenWidth, (int)SystemParameters.PrimaryScreenHeight, System.Drawing.Imaging.PixelFormat.Format16bppRgb555);
            
            screenSnapshot = HelperMethods.GetScreenSnapshot();

            using (Graphics g = Graphics.FromImage(bitmap))
            {
                g.DrawImage(screenSnapshot, destRect, destRect, GraphicsUnit.Pixel);
            }
            return bitmap.ToBitmapSource();
        }

        /// <summary>
        /// 获得全屏截图
        /// </summary>
        /// <returns></returns>
        public static Bitmap CopyFromScreenSnapshotBitmap()
        {

            Rectangle destRect = new Rectangle(0, 0, (int)SystemParameters.PrimaryScreenWidth, (int)SystemParameters.PrimaryScreenHeight);

            var bitmap = new Bitmap((int)SystemParameters.PrimaryScreenWidth, (int)SystemParameters.PrimaryScreenHeight, System.Drawing.Imaging.PixelFormat.Format16bppRgb555);

            screenSnapshot = HelperMethods.GetScreenSnapshot();

            using (Graphics g = Graphics.FromImage(bitmap))
            {
                g.DrawImage(screenSnapshot, destRect, destRect, GraphicsUnit.Pixel);
            }
            return bitmap;
        }

        /*使用方法
         * 
         * private Size? lastSize;

        public Window1()
        {
            InitializeComponent();

            ShiningMeeting.ToolClasses.CaptureScreen.ScreenCaputre.Instance.ScreenCaputreCancelled += OnScreenCaputreCancelled;
            ShiningMeeting.ToolClasses.CaptureScreen.GetScreen.SaveComplete += new System.Action(GetScreen_SaveComplete);
        }

        void GetScreen_SaveComplete()
        {
            Show();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Hide();
            Thread.Sleep(1000);
            ShiningMeeting.ToolClasses.CaptureScreen.ScreenCaputre.Instance.StartCaputre(lastSize);
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Hide();
            Thread.Sleep(1000);
            ShiningMeeting.ToolClasses.CaptureScreen.GetScreen.SaveScreen(ShiningMeeting.ToolClasses.CaptureScreen.GetScreen.CopyFromScreenSnapshot());
        }
         */

    }
}
