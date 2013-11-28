using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace ShiningMeeting.ScatterViewItem.SubItem
{
    public class UnknownScatterViewItem : BaseScatterViewItem
    {
        private ImageSource m_ImageSource;

        public ImageSource ImageSource
        {
            get { return (ImageSource)GetValue(ImageSourceProperty); }
            set { SetValue(ImageSourceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ImageSource.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ImageSourceProperty =
            DependencyProperty.Register("ImageSource", typeof(ImageSource), typeof(UnknownScatterViewItem), new UIPropertyMetadata(null));


        static UnknownScatterViewItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(UnknownScatterViewItem), new FrameworkPropertyMetadata(typeof(UnknownScatterViewItem)));
        }
        public UnknownScatterViewItem()
        {
            InitialCompleted += new RoutedEventHandler(UnknownScatterViewItem_InitialComplated);
        }

        void UnknownScatterViewItem_InitialComplated(object sender, RoutedEventArgs e)
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
                System.Windows.Media.Imaging.BitmapFrame bitmapImage = ShiningMeeting.ToolClasses.WPFUtil.StringToBitmapImage(@"pack://application:,,,/ShineMeeting;component/Resource/未知格式.png")
                    as System.Windows.Media.Imaging.BitmapFrame;

                if (bitmapImage == null) return;
                ///在大于标准像素时 需要进行缩放

                m_ImageSource = bitmapImage;

                this.Dispatcher.BeginInvoke((Action)delegate
                {
                    RoutedEventArgs args = new RoutedEventArgs(InitialCompletedRouteEvent);
                    RaiseEvent(args);
                }, null);
            })).Start();
        }

        public override void CleanScatterViewItem()
        {
            ImageSource = null;
            InitialCompleted -= new RoutedEventHandler(UnknownScatterViewItem_InitialComplated);
        }
    }
}
