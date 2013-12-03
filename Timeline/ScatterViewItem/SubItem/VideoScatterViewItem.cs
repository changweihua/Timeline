using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using ShiningMeeting.Mvvm.Command;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Runtime.InteropServices;
using System.IO;
using System.Threading;
using System.Windows.Media;
using Timeline.ScatterViewItem;
using Timeline.ScatterViewItem.SubItem;
using Timeline.ToolClasses;

namespace Timeline.ScatterViewItem.SubItem
{
    public class VideoScatterViewItem : BaseScatterViewItem
    {
        public ICommand MediaOpened { get; set; }
        public ICommand MediaEnded { get; set; }
        public ICommand IsPlayingChanged { get; set; }

        public Uri VideoSource
        {
            get { return (Uri)GetValue(VideoSourceProperty); }
            set { SetValue(VideoSourceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for VideoSource.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty VideoSourceProperty =
            DependencyProperty.Register("VideoSource", typeof(Uri), typeof(VideoScatterViewItem), new UIPropertyMetadata(null));

        public double Volume
        {
            get { return (double)GetValue(VolumeProperty); }
            set { SetValue(VolumeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Volume.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty VolumeProperty =
            DependencyProperty.Register("Volume", typeof(double), typeof(VideoScatterViewItem), new UIPropertyMetadata(0.0D));


        public bool IsPlaying
        {
            get { return (bool)GetValue(IsPlayingProperty); }
            set { SetValue(IsPlayingProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsPlaying.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsPlayingProperty =
            DependencyProperty.Register("IsPlaying", typeof(bool), typeof(VideoScatterViewItem), new UIPropertyMetadata(false));

        public BitmapSource Thumbnail
        {
            get { return (BitmapSource)GetValue(ThumbnailProperty); }
            set { SetValue(ThumbnailProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Thumbnail.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ThumbnailProperty =
            DependencyProperty.Register("Thumbnail", typeof(BitmapSource), typeof(VideoScatterViewItem), new UIPropertyMetadata(null));

        public bool IsAutoPlay { get { return m_IsAutoPlay; } set { m_IsAutoPlay = value; } }
        public bool IsUsingVideoThumbnail { get; set; }

        private MediaElement m_MediaElement = null;
        private bool m_IsFirstOpen = true;
        private bool m_IsAutoPlay = true;
        private bool m_IsManualStop = false;

        private static IShellFolder pDestop = null;
        private static IShellFolder pSub = null;
        private static IExtractImage pExtractImage = null;
        private static IntPtr pDesktopPtr = IntPtr.Zero;
        private static IntPtr pIdList = IntPtr.Zero;
        private static IntPtr hBmp = IntPtr.Zero;

        static VideoScatterViewItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(VideoScatterViewItem), new FrameworkPropertyMetadata(typeof(VideoScatterViewItem)));
            pDestop = API.GetDesktopFolder(out pDesktopPtr);
        }
        public VideoScatterViewItem()
        {
            MediaOpened = new DelegateCommand<CommandParameter>(MediaElement_MediaOpened);
            MediaEnded = new DelegateCommand<CommandParameter>(MediaElement_MediaEnded);
            IsPlayingChanged = new DelegateCommand<CommandParameter>(Image_IsPlayingChanged);
            InitialCompleted += new RoutedEventHandler(ImageScatterViewItem_InitialComplated);
        }
        protected override void InitailScatterViewItem()
        {
            Task t = new Task((Action)delegate
            {
                Thread.Sleep(100);
                this.Dispatcher.Invoke((Action)delegate
                  {
                      VideoSource = new Uri(SourceModel.FileFullPath, UriKind.RelativeOrAbsolute);
                      RoutedEventArgs args = new RoutedEventArgs(InitialCompletedRouteEvent);
                      RaiseEvent(args);
                  });
            });
            OfficeOpenTaskPool.Add(new Timeline.ToolClasses.TaskInfo(t));
        }

        void Image_IsPlayingChanged(CommandParameter param)
        {
            if (IsPlaying)
            {
                m_IsManualStop = true;
                Pause();
            }
            else
            {
                Play();
            }
        }
        void MediaElement_MediaEnded(CommandParameter param)
        {
            Play();
        }
        void MediaElement_MediaOpened(CommandParameter param)
        {
            if (m_IsFirstOpen) ///第一次打开触发路由事件
            {
                MediaElement element = param.Sender as MediaElement;

                //element.Pause();
                double dot = double.NaN;
                while (double.IsNaN(dot))
                {
                    dot = element.NaturalVideoWidth / (double)element.NaturalVideoHeight;
                    System.Threading.Thread.Sleep(10);
                }
                double dotScreen = ConstClass.APPWIDTH / ConstClass.APPHEIGHT;
                if (element.NaturalVideoHeight < ConstClass.APPHEIGHT &&
                    element.NaturalVideoWidth < ConstClass.APPWIDTH)
                {
                    Height = element.NaturalVideoHeight;
                    Width = element.NaturalVideoWidth;
                }
                else if (dot < dotScreen)
                {
                    Height = ConstClass.APPHEIGHT;
                    Width = Height * dot;
                }
                else
                {
                    Width = ConstClass.APPWIDTH;
                    Height = Width / dot;
                }

                m_MediaElement = element;

                Play();


            }
        }
        void ImageScatterViewItem_InitialComplated(object sender, RoutedEventArgs e)
        {
            base.BaseScatterViewItem_InitialComplated(sender, e);
        }

        public void Play()
        {
            if (m_MediaElement != null)
            {
                if (!m_IsManualStop)
                    m_MediaElement.LoadedBehavior = MediaState.Stop;
                m_MediaElement.LoadedBehavior = MediaState.Play;
            }

            Volume = 1;
            IsPlaying = true;
        }
        public void Pause()
        {
            if (m_MediaElement != null)
                m_MediaElement.LoadedBehavior = MediaState.Pause;
            Volume = 0;
            IsPlaying = false;
        }

        public override void CleanScatterViewItem()
        {
            if (m_MediaElement != null)
            {
                m_MediaElement.LoadedBehavior = MediaState.Close;
                m_MediaElement.UnloadedBehavior = MediaState.Close;
            }
            m_MediaElement = null;
            this.DataContext = null;
            base.CleanScatterViewItem();
        }

        [DllImport("gdi32")]
        static extern int DeleteObject(IntPtr o);
    }
}
