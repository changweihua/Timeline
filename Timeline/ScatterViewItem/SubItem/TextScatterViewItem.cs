using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.IO;
using System.Windows.Controls;
using System.Windows.Media;

namespace ShiningMeeting.ScatterViewItem.SubItem
{
    public class TextScatterViewItem : BaseScatterViewItem
    {
        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Text.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(TextScatterViewItem), new UIPropertyMetadata(""));

        private ScrollViewer m_ScrollViewer;

        static TextScatterViewItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TextScatterViewItem), new FrameworkPropertyMetadata(typeof(TextScatterViewItem)));
        }
        public TextScatterViewItem()
        {
            Width = 280D;
            Height = 350D;
            Tapped += new TappedRoutedEventHandler(TextScatterViewItem_Tapped);

            base.BaseScatterViewItem_InitialComplated(null, null);
        }

        private static List<ScrollViewer> tt = new List<ScrollViewer>();

        void TextScatterViewItem_Tapped(object sender, TappedEventArgs e)
        {
            tt.Add(((sender as TextScatterViewItem).ContentTemplate.LoadContent() as Grid).Children[0] as ScrollViewer);
            GetPageChange(e.Point);
        }

        public override void OnApplyTemplate()
        {
            tt.Add(((this as TextScatterViewItem).ContentTemplate.LoadContent() as Grid).Children[0] as ScrollViewer);
            // m_ScrollViewer = (this.ContentTemplate.LoadContent() as Grid).Children[0] as ScrollViewer;
            base.OnApplyTemplate();
        }

        void GetPageChange(Point point)
        {
            m_ScrollViewer = ((m_text as TextScatterViewItem).ContentTemplate.LoadContent() as Grid).Children[0] as ScrollViewer;
            //o = VisualTreeHelper.GetParent(o);
            // 在item总宽度的三分之一左方点击显示上一页
            if (point.X < ActualWidth / 3)
            {
            }
            else if (point.X > 2 * ActualWidth / 3) // 在item宽度的三分之二右方点击显示下一页
            {
                foreach (ScrollViewer t in tt)
                {
                    double a = t.VerticalOffset;
                    t.IsHitTestVisible = true;
                    double b = t.VerticalOffset;
                }
            }
        }

        protected override void InitailScatterViewItem()
        {
            new System.Threading.Thread(new System.Threading.ThreadStart(delegate
            {
                string text = string.Empty;
                using (FileStream fileStream = File.Open(SourceModel.FileFullPath, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite))
                {
                    byte[] buffer = new byte[fileStream.Length];
                    fileStream.Read(buffer, 0, buffer.Length);
                    text = Encoding.Default.GetString(buffer);
                }

                this.Dispatcher.BeginInvoke((Action)delegate
                {
                    Text = text;

                    RoutedEventArgs args = new RoutedEventArgs(InitialCompletedRouteEvent);
                    RaiseEvent(args);
                }, null);
            })).Start();

        }
        public override void CleanScatterViewItem()
        {

        }
    }
}
