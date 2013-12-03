using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Windows;
using System.Diagnostics;
using Timeline.ToolClasses;
using Microsoft.Surface.Presentation.Controls;
using System.Threading;
using Timeline.Model;
using Timeline;
using Timeline.ScatterViewItem.SubItem;

namespace Timeline.ScatterViewItem
{
    public class TappedEventArgs : RoutedEventArgs
    {
        public TappedEventArgs(RoutedEvent routedEvent) : base(routedEvent) { }
        public Point Point { get; set; }
    }
    public abstract class BaseScatterViewItem : Microsoft.Surface.Presentation.Controls.ScatterViewItem, ICloneable
    {
        public delegate void TappedRoutedEventHandler(object sender, TappedEventArgs e);

        public static RoutedEvent TappedRouteEvent =
            EventManager.RegisterRoutedEvent("Tapped", RoutingStrategy.Bubble, typeof(TappedRoutedEventHandler), typeof(BaseScatterViewItem));
        public static RoutedEvent DoubleTappedRouteEvent =
            EventManager.RegisterRoutedEvent("DoubleTapped", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(BaseScatterViewItem));
        public static RoutedEvent InitialCompletedRouteEvent =
            EventManager.RegisterRoutedEvent("InitialCompleted", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(BaseScatterViewItem));

        public event Action<BaseScatterViewItem> ObjectRemoved;


        public FileListItemModel SourceModel = null;
        private Stopwatch m_MouseDownWatch = new Stopwatch();
        private Stopwatch m_TappedWatch = new Stopwatch();
        private Point m_MouseDownPoint = new Point();
        private Point m_TappedPoint = new Point();
        public BaseScatterViewItem m_text;

        public event TappedRoutedEventHandler Tapped
        {
            add { AddHandler(TappedRouteEvent, value); }
            remove { RemoveHandler(TappedRouteEvent, value); }
        }
        public event RoutedEventHandler DoubleTapped
        {
            add { AddHandler(DoubleTappedRouteEvent, value); }
            remove { RemoveHandler(DoubleTappedRouteEvent, value); }
        }
        public event RoutedEventHandler InitialCompleted
        {
            add { AddHandler(InitialCompletedRouteEvent, value); }
            remove { RemoveHandler(InitialCompletedRouteEvent, value); }
        }
        public bool IsInitialCompleted { get; set; }

        protected BaseScatterViewItem()
        {
            ShowsActivationEffects = false;
            Deceleration *= 2;
            CanRotate = false;
            //InitialCompleted += new RoutedEventHandler(BaseScatterViewItem_InitialComplated);
        }

        static BaseScatterViewItem()
        {
            try
            {
                CenterProperty.OverrideMetadata(typeof(BaseScatterViewItem), new FrameworkPropertyMetadata(new Point(0, 0), (s, e) =>
                {
                    if (Application.Current.MainWindow == null)
                        return;

                    BaseScatterViewItem item = (s as BaseScatterViewItem);
                    if (item.Center.X > 1920)
                    {
                        new Thread(new ParameterizedThreadStart(delegate
                        {
                            item.Dispatcher.Invoke((Action)delegate { item.Opacity = .5; });
                            Thread.Sleep(500);
                            item.Dispatcher.Invoke((Action)delegate
                            {
                                item.Center = new Point(1920, item.Center.Y);
                                if (item.Parent != null)
                                    (item.Parent as ScatterView).Items.Remove(item);
                                item.CleanScatterViewItem();
                                if (item.ObjectRemoved != null)
                                    item.ObjectRemoved((s as BaseScatterViewItem));
                                item = null;

                            });
                        })) { IsBackground = true }.Start();
                    }
                    else if (item.Center.X < 0)
                        item.Center = new Point(0, item.Center.Y);
                    else if (item.Center.Y > 1080- 88)
                        item.Center = new Point(item.Center.X, 1080 - 88);
                    else if (item.Center.Y < 0 + (Application.Current.MainWindow as MainWindow).TopHeight)
                        item.Center = new Point(item.Center.X, (Application.Current.MainWindow as MainWindow).TopHeight);
                }));
            }
            catch (Exception e)
            {

            }
        }

        protected void BaseScatterViewItem_InitialComplated(object sender, RoutedEventArgs e)
        {
            IsInitialCompleted = true;

            if (!double.IsNaN(Height))
            {
                this.MaxHeight = this.Height * 3;
            }
            if (!double.IsNaN(Width))
            {
                this.MaxWidth = this.Width * 3;
            }

        }

        protected abstract void InitailScatterViewItem();
        public virtual void CleanScatterViewItem()
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
        }

        #region Override Method
        protected override void OnInitialized(EventArgs e)
        {
            this.AddHandler(TappedRouteEvent, new TappedRoutedEventHandler(this.ScatterViewItem_Tapped));
            base.OnInitialized(e);
        }
        public override void OnApplyTemplate()
        {
            FrameworkElement shadow = this.Template.FindName("shadow", this) as FrameworkElement;
            if (shadow != null)
                shadow.Visibility = Visibility.Hidden;
            base.OnApplyTemplate();
        }
        protected override void OnMouseDown(System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.StylusDevice != null)
                return;
            m_MouseDownPoint = e.GetPosition(this);
            m_MouseDownWatch.Restart();
            base.OnMouseDown(e);
        }
        protected override void OnMouseUp(System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.StylusDevice != null)
                return;
            m_text = this as BaseScatterViewItem;
            m_MouseDownWatch.Stop();
            Point mouseUpPoint = e.GetPosition(this);
            if (m_MouseDownWatch.ElapsedMilliseconds < 250 &&
                Math.Abs(m_MouseDownPoint.X - mouseUpPoint.X) < 5 &&
                Math.Abs(m_MouseDownPoint.Y - mouseUpPoint.Y) < 5)
            {
                TappedEventArgs args = new TappedEventArgs(TappedRouteEvent);
                args.Point = mouseUpPoint;
                RaiseEvent(args);
            }
            base.OnMouseUp(e);
        }
        protected override void OnTouchDown(System.Windows.Input.TouchEventArgs e)
        {
            m_MouseDownPoint = e.GetTouchPoint(this).Position;
            m_MouseDownWatch.Restart();
            base.OnTouchDown(e);
            e.Handled = true;
        }
        protected override void OnTouchUp(System.Windows.Input.TouchEventArgs e)
        {
            m_MouseDownWatch.Stop();
            Point mouseUpPoint = e.GetTouchPoint(this).Position;
            if (m_MouseDownWatch.ElapsedMilliseconds < 250 &&
                Math.Abs(m_MouseDownPoint.X - mouseUpPoint.X) < 10 &&
                Math.Abs(m_MouseDownPoint.Y - mouseUpPoint.Y) < 10)
            {
                TappedEventArgs args = new TappedEventArgs(TappedRouteEvent);
                args.Point = mouseUpPoint;
                RaiseEvent(args);
            }
            base.OnTouchUp(e);
        }
        ~BaseScatterViewItem()
        {
            this.RemoveHandler(TappedRouteEvent, new TappedRoutedEventHandler(this.ScatterViewItem_Tapped));
            InitialCompleted -= new RoutedEventHandler(BaseScatterViewItem_InitialComplated);
        }
        #endregion

        private void ScatterViewItem_Tapped(object sender, TappedEventArgs e)
        {
            if (m_TappedWatch.IsRunning)
            {
                m_TappedWatch.Stop();
                Point newTappedPoint = e.Point;
                if (m_TappedWatch.ElapsedMilliseconds < 250 &&
                Math.Abs(m_TappedPoint.X - newTappedPoint.X) < 20 &&
                Math.Abs(m_TappedPoint.Y - newTappedPoint.Y) < 20)
                {
                    RoutedEventArgs args = new RoutedEventArgs(DoubleTappedRouteEvent);
                    RaiseEvent(args);
                }
            }
            m_TappedPoint = e.Point;
            m_TappedWatch.Restart();
        }

        public static BaseScatterViewItem CreateScatterViewItem(FileListItemModel item)
        {
            BaseScatterViewItem scatterViewItem = null;

            string tag = ConstClass.GetScatterViewItemType(item.FileType);

            scatterViewItem =
                Assembly.LoadFrom("Timeline.exe").CreateInstance("Timeline.ScatterViewItem.SubItem." + tag + "ScatterViewItem") as BaseScatterViewItem;

            string entension = System.IO.Path.GetExtension(item.FileFullPath).ToLower();
            if (scatterViewItem is OfficeScatterViewItem)
            {
                (scatterViewItem as SubItem.OfficeScatterViewItem).IsUsingOfficeThumbnail = false;
            }
            if (entension.Equals(".ppt") || entension.Equals(".pptx"))
            {
                (scatterViewItem as SubItem.OfficeScatterViewItem).IsPptFile = true;
                (scatterViewItem as SubItem.OfficeScatterViewItem).IsUsingOfficeThumbnail = false;
            }
            if (scatterViewItem is VideoScatterViewItem)
            {
                (scatterViewItem as SubItem.VideoScatterViewItem).IsUsingVideoThumbnail = false;
            }

            scatterViewItem.SourceModel = item;
            scatterViewItem.InitailScatterViewItem();
            return scatterViewItem;
        }

        public virtual object Clone()
        {
            string tag = ConstClass.GetScatterViewItemType(SourceModel.FileType);

            BaseScatterViewItem scatterViewItem =
                Assembly.LoadFrom("Timeline.exe").CreateInstance("Timeline.ScatterViewItem.SubItem." + tag + "ScatterViewItem") as BaseScatterViewItem;
            string entension = System.IO.Path.GetExtension(SourceModel.FileFullPath).ToLower();
            if (entension.Equals(".ppt") || entension.Equals(".pptx"))
            {
                (scatterViewItem as SubItem.OfficeScatterViewItem).IsPptFile = true;
                (scatterViewItem as SubItem.OfficeScatterViewItem).IsInsertTask = true;
            }

            scatterViewItem.SourceModel = SourceModel;
            scatterViewItem.InitailScatterViewItem();
            return scatterViewItem;
        }
    }
}
