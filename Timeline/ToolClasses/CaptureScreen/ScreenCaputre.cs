using System;
using System.Windows;
using System.Windows.Media.Imaging;

namespace ShiningMeeting.ToolClasses.CaptureScreen
{
    public class ScreenCaputre
    {
        public static readonly ScreenCaputre Instance;

        static ScreenCaputre()
        {
            if (Instance == null)
                Instance = new ScreenCaputre();
        }

        public void StartCaputre()
        {
            StartCaputre(null);
        }

        public void StartCaputre(Size? defaultSize)
        {
            var mask = new MaskWindow(this);
            mask.Show(defaultSize);
        }

        public event EventHandler<ScreenCaputredEventArgs> ScreenCaputred;
        public event EventHandler<EventArgs> ScreenCaputreCancelled;

        internal void OnScreenCaputred(object sender, BitmapSource caputredBmp)
        {
            if (ScreenCaputred != null)
            {
                ScreenCaputred(sender, new ScreenCaputredEventArgs(caputredBmp));
            }
        }

        internal void OnScreenCaputreCancelled(object sender)
        {
            if (ScreenCaputreCancelled != null)
            {
                ScreenCaputreCancelled(sender, EventArgs.Empty);
            }
        }
    }

   
}
