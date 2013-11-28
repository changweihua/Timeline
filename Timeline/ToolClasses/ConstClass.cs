using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Timeline.Model;
using System.IO;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows;
using System.Collections.ObjectModel;

namespace Timeline.ToolClasses
{
    public static class ConstClass
    {
        public static bool NoteMenuChoosed = false;

        public static bool CanChooseBarMenu = true;

        public static bool IsUsbExited = false;

        public static string CurrentMenu = string.Empty;

        public static string CurrentSelectType = string.Empty;

        public static bool IsUsbRendered = false;

        public static string USBDirectoryPath = string.Empty;

        public static bool ClickIsBottom = false;

        public static bool InkFileIsAllChoosed = false;

        public static string m_MainSettingPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"AppConfig.xml");

        public static string m_TempXPSPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TempXPS");

        public static string m_TempPDFPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TempPDF");

        public static bool CanOpenScatterViewItem = true;

        public static int ZIndex = 0;

        public enum SpecialFolder
        {
            MainSettingPath,
            ImportExportPath,
            PageConfigPath,
            IconConfigPath,
            TempXPSFolder,
            TempPDFFolder
        }

        public static string GetPath(SpecialFolder folder)
        {
            string path = string.Empty;
            switch (folder)
            {
                case SpecialFolder.MainSettingPath:
                    path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), m_MainSettingPath);
                    break;
                //case SpecialFolder.PageConfigPath:
                //    path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), m_PageConfigPath);
                //    break;
                //case SpecialFolder.IconConfigPath:
                //    path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), m_IconConfigPath);
                //    break;
                case SpecialFolder.TempXPSFolder:
                    path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), m_TempXPSPath);
                    break;
                case SpecialFolder.TempPDFFolder:
                    path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), m_TempPDFPath);
                    break;
            }
            return path;
        }

        public static string WordFileExtension = "*.doc|*.docx";
        public static string ExcelFileExtension = "*.xls|*.xlsx";
        public static string PPTFileExtension = "*.ppt|*.pptx";
        public static string PdfFileExtension = "*.pdf";
        public static string PhotoFileExtension = "*.jpg|*.bmp|*.jpeg|*.png";
        public static string VideoFileExtension = "*.avi|*.wmv|*.3gp|*.mp4|*.mov";
        public static string AllKnowExtension = "*.jpg|*.bmp|*.jpeg|*.png|*.avi|*.wmv|*.3gp|*.mp4|*.mov|*.pdf|*.ppt|*.pptx|*.xls|*.xlsx|*.doc|*.docx";

        public const double NORMAL_IMAGE_WIDTH = 2000.0D;
        public const double NORMAL_IMAGE_HEIGHT = 2000.0D;
        public const double APPWIDTH = 1920.0D;
        public const double APPHEIGHT = 1080.0D;

        public static string GetScatterViewItemType(string str)
        {
            string tag = string.Empty;

            switch (str)
            {
                case "Microsoft Office Word 文档":
                case "Microsoft Office Excel 文档":
                case "Microsoft Office PPT 文档":
                    tag = "Office";
                    break;
                case "Adobe PDF 文档":
                    tag = "Pdf";
                    break;
                case "图片文件":
                    tag = "Image";
                    break;
                case "视频文件":
                    tag = "Video";
                    break;
            }

            if (string.IsNullOrEmpty(tag))
                tag = "More";

            return tag;
        }

        public static string GetFileSimpleDeclaration(string type, string fileExtension)
        {
            string str = string.Empty;
            switch (type.ToLower())
            {
                case "word":
                    str = "Microsoft Office Word 文档";
                    break;
                case "excel":
                    str = "Microsoft Office Excel 文档";
                    break;
                case "ppt":
                    str = "Microsoft Office PPT 文档";
                    break;
                case "pdf":
                    str = "Adobe PDF 文档";
                    break;
                case "photo":
                    str = "图片文件";
                    break;
                case "video":
                    str = "视频文件";
                    break;
                case "more":
                    str = fileExtension;
                    break;
            }
            return str;
        }
    }

    public class ImageConvertOperations
    {
        public static System.Drawing.Bitmap GetBitmapFromBitmapSource(System.Windows.Media.Imaging.BitmapSource bmpSource)
        {
            System.Drawing.Bitmap bmp = new System.Drawing.Bitmap(bmpSource.PixelWidth, bmpSource.PixelHeight, System.Drawing.Imaging.PixelFormat.Format32bppPArgb);
            System.Drawing.Imaging.BitmapData data = bmp.LockBits(new System.Drawing.Rectangle(System.Drawing.Point.Empty, bmp.Size), System.Drawing.Imaging.ImageLockMode.WriteOnly, System.Drawing.Imaging.PixelFormat.Format32bppPArgb);
            bmpSource.CopyPixels(System.Windows.Int32Rect.Empty, data.Scan0, data.Height * data.Stride, data.Stride);
            bmp.UnlockBits(data);
            return bmp;
        }

        public static System.Windows.Media.Imaging.BitmapSource GetBitmapSourceFromBitmap(System.Drawing.Bitmap bitmap)
        {
            MemoryStream ms = new MemoryStream();
            bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);

            System.Windows.Media.Imaging.BitmapImage bit = new System.Windows.Media.Imaging.BitmapImage();
            bit.BeginInit();
            bit.StreamSource = ms;
            bit.EndInit();

            return bit;
        }

        public static System.Windows.Media.Imaging.BitmapSource GetBitmapSourceFromDrawImage(System.Drawing.Image img)
        {
            MemoryStream ms = new MemoryStream();
            img.Save(ms, System.Drawing.Imaging.ImageFormat.Png);

            System.Windows.Media.Imaging.BitmapImage bit = new System.Windows.Media.Imaging.BitmapImage();
            bit.BeginInit();
            bit.StreamSource = ms;
            bit.EndInit();

            return bit;
        }

        public static System.Windows.Media.Imaging.BitmapImage ByteArrayToBitmapImage(byte[] byteArray)
        {
            System.Windows.Media.Imaging.BitmapImage bmp = null;

            try
            {
                bmp = new System.Windows.Media.Imaging.BitmapImage();
                bmp.BeginInit();
                bmp.StreamSource = new MemoryStream(byteArray);
                bmp.EndInit();
            }
            catch
            {
                bmp = null;
            }

            return bmp;
        }

        private static byte[] BitmapImageToByte(System.Windows.Media.Imaging.BitmapImage bmp)
        {

            byte[] bytearray = null;

            try
            {
                Stream smarket = bmp.StreamSource;

                if (smarket != null && smarket.Length > 0)
                {
                    //很重要，因为position经常位于stream的末尾，导致下面读取到的长度为0。 
                    smarket.Position = 0;

                    using (BinaryReader br = new BinaryReader(smarket))
                    {
                        bytearray = br.ReadBytes((int)smarket.Length);
                    }
                }
            }
            catch
            {
                //other exception handling 
            }

            return bytearray;


        }

        public static byte[] GetBytesFromFile(string filePath)
        {
            //byte[] imgData;
            //using (System.IO.FileStream fs = new System.IO.FileStream(filePath, System.IO.FileMode.Open))
            //{
            //    imgData = new byte[fs.Length];
            //    fs.Read(imgData, 0, (int)fs.Length);
            //}
            //return imgData;

            byte[] bytes;
            using (BinaryReader reader = new BinaryReader(File.Open(filePath, FileMode.Open)))
            {
                FileInfo fi = new FileInfo(filePath);
                bytes = reader.ReadBytes((int)fi.Length);
            }
            return bytes;
        }

        public static System.Drawing.Image ZoomImageFromFile(string filePath, int width, int height)
        {
            System.Drawing.Image originalImage = System.Drawing.Image.FromFile(filePath);
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

            originalImage.Dispose();
            g.Dispose();

            return bitmap;

            //try
            //{
            //    bitmap.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
            //}
            //catch (System.Exception e)
            //{
            //    throw e;
            //}
            //finally
            //{
            //    originalImage.Dispose();
            //    bitmap.Dispose();
            //    g.Dispose();
            //}
        }

        public static System.Drawing.Image ZoomImageFromStream(Stream sourceImg, int width, int height)
        {
            System.Drawing.Image originalImage = System.Drawing.Image.FromStream(sourceImg);
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

            originalImage.Dispose();
            g.Dispose();

            return bitmap;

            //try
            //{
            //    bitmap.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
            //}
            //catch (System.Exception e)
            //{
            //    throw e;
            //}
            //finally
            //{
            //    originalImage.Dispose();
            //    bitmap.Dispose();
            //    g.Dispose();
            //}
        }

        public static System.Drawing.Image ZoomImageFromFile(string sourceImg, int width, int height, string backColor = "#00FFFFFF", string borderColor = "#00FFFFFF")
        {
            System.Drawing.Image originalImage = System.Drawing.Image.FromFile(sourceImg);
            int towidth = width;
            int toheight = height;
            int x = 0;
            int y = 0;
            int ow = originalImage.Width;
            int oh = originalImage.Height;
            string mode;
            if (ow < towidth && oh < toheight)
            {
                towidth = ow;
                toheight = oh;
            }
            else
            {
                if (originalImage.Width / originalImage.Height >= width / height)
                {
                    mode = "W";
                }
                else
                {
                    mode = "H";
                }
                switch (mode)
                {
                    case "W":
                        //指定宽，高按比例  
                        toheight = originalImage.Height * width / originalImage.Width;
                        break;
                    case "H"://指定高，宽按比例  
                        towidth = originalImage.Width * height / originalImage.Height;
                        break;
                    default:
                        break;
                }
            }
            //新建一个bmp图片        
            System.Drawing.Image bitmap = new System.Drawing.Bitmap(width, height);
            //新建一个画板       
            System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(bitmap);
            //设置高质量插值法        
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;

            //设置高质量,低速度呈现平滑程度
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;

            //清空画布并以指定颜色填充        
            g.Clear(System.Drawing.ColorTranslator.FromHtml(backColor));
            //在指定位置并且按指定大小绘制原图片的指定部分   
            int top = (height - toheight) / 2;
            int left = (width - towidth) / 2;
            g.DrawImage(originalImage, new System.Drawing.Rectangle(left, top, towidth, toheight),
                new System.Drawing.Rectangle(x, y, ow, oh),
                System.Drawing.GraphicsUnit.Pixel);
            System.Drawing.Pen pen = new System.Drawing.Pen(System.Drawing.ColorTranslator.FromHtml(borderColor));
            g.DrawRectangle(pen, 0, 0, width - 1, height - 1);
            //try
            //{
            //    //以png格式保存缩略图   
            //bitmap.Save(toPath, System.Drawing.Imaging.ImageFormat.Png);
            //}
            //catch (System.Exception e)
            //{
            //    throw e;
            //}
            //finally
            //{
            //    originalImage.Dispose();
            //    bitmap.Dispose();
            //    g.Dispose();
            //}
            originalImage.Dispose();
            g.Dispose();


            return bitmap;
        }

        public static System.Drawing.Image ZoomImageFromStream(MemoryStream sourceImg, int width, int height, string backColor = "#00FFFFFF", string borderColor = "#00FFFFFF")
        {
            System.Drawing.Image originalImage = System.Drawing.Image.FromStream(sourceImg);
            int towidth = width;
            int toheight = height;
            int x = 0;
            int y = 0;
            int ow = originalImage.Width;
            int oh = originalImage.Height;
            string mode;
            if (ow < towidth && oh < toheight)
            {
                towidth = ow;
                toheight = oh;
            }
            else
            {
                if (originalImage.Width / originalImage.Height >= width / height)
                {
                    mode = "W";
                }
                else
                {
                    mode = "H";
                }
                switch (mode)
                {
                    case "W":
                        //指定宽，高按比例  
                        toheight = originalImage.Height * width / originalImage.Width;
                        break;
                    case "H"://指定高，宽按比例  
                        towidth = originalImage.Width * height / originalImage.Height;
                        break;
                    default:
                        break;
                }
            }
            //新建一个bmp图片        
            System.Drawing.Image bitmap = new System.Drawing.Bitmap(width, height);
            //新建一个画板       
            System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(bitmap);
            //设置高质量插值法        
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;

            //设置高质量,低速度呈现平滑程度
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;

            //清空画布并以指定颜色填充        
            g.Clear(System.Drawing.ColorTranslator.FromHtml(backColor));
            //在指定位置并且按指定大小绘制原图片的指定部分   
            int top = (height - toheight) / 2;
            int left = (width - towidth) / 2;
            g.DrawImage(originalImage, new System.Drawing.Rectangle(left, top, towidth, toheight),
                new System.Drawing.Rectangle(x, y, ow, oh),
                System.Drawing.GraphicsUnit.Pixel);
            System.Drawing.Pen pen = new System.Drawing.Pen(System.Drawing.ColorTranslator.FromHtml(borderColor));
            g.DrawRectangle(pen, 0, 0, width - 1, height - 1);
            //try
            //{
            //    //以png格式保存缩略图   
            //    bitmap.Save(toPath, System.Drawing.Imaging.ImageFormat.Png);
            //}
            //catch (System.Exception e)
            //{
            //    throw e;
            //}
            //finally
            //{
            //    originalImage.Dispose();
            //    bitmap.Dispose();
            //    g.Dispose();
            //}
            originalImage.Dispose();
            g.Dispose();

            return bitmap;
        }

        public static System.Windows.Media.Imaging.BitmapSource GetStreamBitmapSourceFromPath(string path)
        {
            System.Windows.Media.Imaging.BitmapImage bitmapImage;
            FileStream fs = File.OpenRead(path);
            using (BinaryReader reader = new BinaryReader(fs))
            {
                byte[] bytes = reader.ReadBytes((int)fs.Length);
                fs.Close();
                fs.Dispose();
                reader.Close();
                bitmapImage = new System.Windows.Media.Imaging.BitmapImage();
                try
                {
                    bitmapImage.BeginInit();
                    bitmapImage.StreamSource = new MemoryStream(bytes);
                    bitmapImage.EndInit();
                }
                catch
                {
                    return null;
                }
                bitmapImage.CacheOption = System.Windows.Media.Imaging.BitmapCacheOption.OnLoad;
            }

            return bitmapImage;
        }
    }

    public class LinearMatrixAnimation : AnimationTimeline
    {

        public IEasingFunction EasingFunction
        {
            get { return (IEasingFunction)GetValue(EasingFunctionProperty); }
            set { SetValue(EasingFunctionProperty, value); }
        }

        // Using a DependencyProperty as the backing store for EasingFunction.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty EasingFunctionProperty =
            DependencyProperty.Register("EasingFunction", typeof(IEasingFunction), typeof(LinearMatrixAnimation), new UIPropertyMetadata(null));


        public Matrix? From
        {
            set { SetValue(FromProperty, value); }
            get { return (Matrix)GetValue(FromProperty); }
        }
        public static DependencyProperty FromProperty = DependencyProperty.Register("From", typeof(Matrix?), typeof(LinearMatrixAnimation), new PropertyMetadata(null));
        public Matrix? To
        {
            set { SetValue(ToProperty, value); }
            get { return (Matrix)GetValue(ToProperty); }
        }
        public static DependencyProperty ToProperty = DependencyProperty.Register("To", typeof(Matrix?), typeof(LinearMatrixAnimation), new PropertyMetadata(null));
        public LinearMatrixAnimation()
        {
        }
        public LinearMatrixAnimation(Matrix from, Matrix to, Duration duration)
        {
            Duration = duration;
            From = from;
            To = to;

        }
        public override object GetCurrentValue(object defaultOriginValue, object defaultDestinationValue, AnimationClock animationClock)
        {
            if (animationClock.CurrentProgress == null)
            {
                return null;
            }
            double progress = animationClock.CurrentProgress.Value;
            if (EasingFunction != null)
            {
                progress = EasingFunction.Ease(progress);
            }

            Matrix from = From ?? (Matrix)defaultOriginValue;
            if (To.HasValue)
            {
                Matrix to = To.Value;

                Matrix newMatrix = new Matrix(((to.M11 - from.M11) * progress) + from.M11, ((to.M12 - from.M12) * progress) + from.M12, ((to.M21 - from.M21) * progress) + from.M21, ((to.M22 - from.M22) * progress) + from.M22,
                                              ((to.OffsetX - from.OffsetX) * progress) + from.OffsetX, ((to.OffsetY - from.OffsetY) * progress) + from.OffsetY);

                //defaultDestinationValue = newMatrix;
                return newMatrix;
            }
            return Matrix.Identity;
        }
        protected override System.Windows.Freezable CreateInstanceCore()
        {
            return new LinearMatrixAnimation();
        }
        public override System.Type TargetPropertyType
        {
            get { return typeof(Matrix); }
        }
    }
}
