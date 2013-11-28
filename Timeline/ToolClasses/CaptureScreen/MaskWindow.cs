using System.Drawing;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using System;

namespace ShiningMeeting.ToolClasses.CaptureScreen
{
    internal class MaskWindow : Window
    {
#pragma warning disable
        private MaskCanvas innerCanvas;
        private Bitmap screenSnapshot;
        private Timer timeOutTimmer;
        private readonly ScreenCaputre screenCaputreOwner;

        public MaskWindow(ScreenCaputre screenCaputreOwner)
        {
            this.screenCaputreOwner = screenCaputreOwner;
            Ini();
        }

        private void Ini()
        {
            
            //ini normal properties
            //Topmost = true;
            WindowStyle = WindowStyle.None;
            ResizeMode = ResizeMode.NoResize;
            ShowInTaskbar = false;

            //set bounds to cover all screens
            var rect = SystemInformation.VirtualScreen;
            Left = rect.X;
            Top = rect.Y;
            Width = rect.Width;
            Height = rect.Height;

            //set background 
            screenSnapshot = HelperMethods.GetScreenSnapshot();
            if (screenSnapshot != null)
            {
                var bmp = screenSnapshot.ToBitmapSource();
                bmp.Freeze();
                Background = new ImageBrush(bmp);
            }

            //ini canvas
            innerCanvas = new MaskCanvas
            {
                MaskWindowOwner = this
            };
            Content = innerCanvas;

        }

        private DispatcherTimer dispatcherTimer = new DispatcherTimer();
        private int ClickCount;

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);

            if (dispatcherTimer.IsEnabled)
                dispatcherTimer.Stop();

            ClickCount++;

            dispatcherTimer.Interval = TimeSpan.FromMilliseconds(300);
            dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
            if (!dispatcherTimer.IsEnabled)
                dispatcherTimer.Start();

            if (ClickCount >= 2)
            {
                CancelCaputre();
            }
            e.Handled = true;
        }

        void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            dispatcherTimer.Stop();
            ClickCount = 0;
        }

        protected override void OnMouseMove(System.Windows.Input.MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if(timeOutTimmer != null && timeOutTimmer.Enabled)
            {
                timeOutTimmer.Stop();
                timeOutTimmer.Start();
            }
        }

        protected override void OnKeyDown(System.Windows.Input.KeyEventArgs e)
        {
            base.OnKeyDown(e);

            if(e.Key == Key.Escape)
            {
                CancelCaputre();
            }
        }

        public void CancelCaputre()
        {
            Close();
            screenCaputreOwner.OnScreenCaputreCancelled(null);
        }

        internal void OnShowMaskFinished(Rect maskRegion)
        {
            
        }

        internal void ClipSnapshot(Rect clipRegion)
        {
            BitmapSource caputredBmp = CopyFromScreenSnapshot(clipRegion);

            if (caputredBmp != null)
            {
                GetScreen.SaveScreen(caputredBmp);
            }

            //close mask window
            Close();
        }


        internal BitmapSource CopyFromScreenSnapshot(Rect region)
        {
            var sourceRect = region.ToRectangle();
            var destRect = new Rectangle(0, 0, sourceRect.Width, sourceRect.Height);

            if (screenSnapshot != null)
            {
                var bitmap = new Bitmap(sourceRect.Width, sourceRect.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                using (Graphics g = Graphics.FromImage(bitmap))
                {
                    g.DrawImage(screenSnapshot, destRect, sourceRect, GraphicsUnit.Pixel);
                }

                return bitmap.ToBitmapSource();
            }

            return null;
        }

        public void Show(System.Windows.Size? defaultSize)
        {           
            if(innerCanvas != null)
            {
                innerCanvas.DefaultSize = defaultSize;
            }

            ShowDialog();
            Focus();

        }        
    }
}
