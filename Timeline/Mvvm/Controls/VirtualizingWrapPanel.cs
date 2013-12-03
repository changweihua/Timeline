using System;
using System.Collections.Specialized;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Collections;
using System.Runtime.InteropServices;
using System.Collections.Generic;

namespace ShiningMeeting.Mvvm.Controls
{
    /// <summary>
    /// Contains extension methods for enumerating the parents of an element.
    /// </summary>
    public static class ParentOfTypeExtensions
    {
        /// <summary>
        /// Gets the parent element from the visual tree by given type.
        /// </summary>
        public static T ParentOfType<T>(this DependencyObject element) where T : DependencyObject
        {
            if (element == null) return null;

            return element.GetParents().OfType<T>().FirstOrDefault();
        }

        ///// <summary>
        /////  Determines whether the element is an ancestor of the descendant.
        ///// </summary>
        ///// <returns>true if the visual object is an ancestor of descendant; otherwise, false.</returns>
        //public static bool IsAncestorOf(this DependencyObject element, DependencyObject descendant)
        //{
        //    element.TestNotNull("element");
        //    descendant.TestNotNull("descendant");

        //    return descendant == element || descendant.GetParents().Contains(element);
        //}

        /// <summary>
        /// Searches up in the visual tree for parent element of the specified type.
        /// </summary>
        /// <typeparam name="T">
        /// The type of the parent that will be searched up in the visual object hierarchy. 
        /// The type should be <see cref="DependencyObject"/>.
        /// </typeparam>
        /// <param name="element">The target <see cref="DependencyObject"/> which visual parents will be traversed.</param>
        /// <returns>Visual parent of the specified type if there is any, otherwise null.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
        public static T GetVisualParent<T>(this DependencyObject element) where T : DependencyObject
        {
            return element.ParentOfType<T>();
        }

        /// <summary>  
        /// This recurse the visual tree for ancestors of a specific type.
        /// </summary>  
        internal static IEnumerable<T> GetAncestors<T>(this DependencyObject element) where T : class
        {
            return element.GetParents().OfType<T>();
        }

        /// <summary>  
        /// This recurse the visual tree for a parent of a specific type.
        /// </summary>  
        internal static T GetParent<T>(this DependencyObject element) where T : FrameworkElement
        {
            return element.ParentOfType<T>();
        }

        /// <summary>
        /// Enumerates through element's parents in the visual tree.
        /// </summary>
        public static IEnumerable<DependencyObject> GetParents(this DependencyObject element)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }

            while ((element = element.GetParent()) != null)
            {
                yield return element;
            }
        }

        private static DependencyObject GetParent(this DependencyObject element)
        {
            var parent = VisualTreeHelper.GetParent(element);
            if (parent == null)
            {
                var frameworkElement = element as FrameworkElement;
                if (frameworkElement != null)
                {
                    parent = frameworkElement.Parent;
                }
            }
            return parent;
        }
    }

    internal static class DoubleUtil
    {

        internal const double DblEpsilon = 2.2204460492503131E-16;


        public static bool IsCloseTo(this double value1, double value2)
        {
            return AreClose(value1, value2);
        }

        public static bool IsGreaterThan(this double value1, double value2)
        {
            return GreaterThan(value1, value2);
        }

        public static bool IsGreaterThanOrClose(this double value1, double value2)
        {
            return GreaterThanOrClose(value1, value2);
        }

        public static bool IsLessThan(this double value1, double value2)
        {
            return LessThan(value1, value2);
        }

        public static bool IsLessThanOrClose(this double value1, double value2)
        {
            return LessThanOrClose(value1, value2);
        }

        public static bool AreClose(double value1, double value2)
        {
            if (value1 == value2)
            {
                return true;
            }
            double num = ((Math.Abs(value1) + Math.Abs(value2)) + 10.0) * DblEpsilon;
            double num2 = value1 - value2;
            return (-num < num2) && (num > num2);
        }

        public static bool AreClose(Point point1, Point point2)
        {
            return AreClose(point1.X, point2.X) && AreClose(point1.Y, point2.Y);
        }

        public static bool AreClose(Rect rect1, Rect rect2)
        {
            if (rect1.IsEmpty)
            {
                return rect2.IsEmpty;
            }
            return ((!rect2.IsEmpty && AreClose(rect1.X, rect2.X)) && (AreClose(rect1.Y, rect2.Y) && AreClose(rect1.Height, rect2.Height))) && AreClose(rect1.Width, rect2.Width);
        }

        public static bool AreClose(Size size1, Size size2)
        {
            return AreClose(size1.Width, size2.Width) && AreClose(size1.Height, size2.Height);
        }

        public static bool AreClose(Vector vector1, Vector vector2)
        {
            return AreClose(vector1.X, vector2.X) && AreClose(vector1.Y, vector2.Y);
        }

        public static int DoubleToInt(double val)
        {
            if (0.0 >= val)
            {
                return (int)(val - 0.5);
            }
            return (int)(val + 0.5);
        }

        public static bool GreaterThan(double value1, double value2)
        {
            return (value1 > value2) && !AreClose(value1, value2);
        }

        public static bool GreaterThanOrClose(double value1, double value2)
        {
            if (value1 <= value2)
            {
                return AreClose(value1, value2);
            }
            return true;
        }

        public static bool IsBetweenZeroAndOne(double val)
        {
            return GreaterThanOrClose(val, 0.0) && LessThanOrClose(val, 1.0);
        }

        public static bool IsNaN(double value)
        {
            NanUnion union = new NanUnion
            {
                DoubleValue = value
            };
            ulong num = union.UintValue & 18442240474082181120L;
            ulong num2 = union.UintValue & ((ulong)0xfffffffffffffL);
            if ((num != 0x7ff0000000000000L) && (num != 18442240474082181120L))
            {
                return false;
            }
            return num2 != 0L;
        }

        public static bool IsOne(double value)
        {
            return Math.Abs((double)(value - 1.0)) < 2.2204460492503131E-15;
        }

        public static bool IsZero(double value)
        {
            return Math.Abs(value) < 2.2204460492503131E-15;
        }

        public static bool LessThan(double value1, double value2)
        {
            return (value1 < value2) && !AreClose(value1, value2);
        }

        public static bool LessThanOrClose(double value1, double value2)
        {
            if (value1 >= value2)
            {
                return AreClose(value1, value2);
            }
            return true;
        }

        public static bool RectHasNaN(Rect r)
        {
            if ((!IsNaN(r.X) && !IsNaN(r.Y)) && (!IsNaN(r.Height) && !IsNaN(r.Width)))
            {
                return false;
            }
            return true;
        }

        public static bool IsPositiveRealNumber(double number)
        {
            if (IsRealNumber(number) && number > 0)
            {
                return true;
            }
            return false;
        }

        public static bool IsNegativeRealNumber(double number)
        {
            if (IsRealNumber(number) && number < 0)
            {
                return true;
            }
            return false;
        }

        public static bool IsRealNumber(double number)
        {
            if (!double.IsNaN(number) && !double.IsInfinity(number))
            {
                return true;
            }
            return false;
        }

        [StructLayout(LayoutKind.Explicit)]
        private struct NanUnion
        {
            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
            [FieldOffset(0)]
            internal double DoubleValue;
            [FieldOffset(0)]
            internal ulong UintValue;
        }
    }

    internal interface IPanelHelper
    {
        IList Children { get; }

        Size DesiredSizeAt(int index);

        double Width { get; }

        double Height { get; }

        Rect GetLayoutSlot(FrameworkElement item);
    }

    internal interface IPanelKeyboardHelper
    {
        IPanelHelper PanelHelper { get; set; }

        Point GetOffsets(int index);

        int GetPageUpIndex(int fromIndex);

        int GetPageDownIndex(int fromIndex);
    }

    /// <summary>
    /// Positions child elements in sequential position from left to right, breaking content 
    /// to the next line at the edge of the containing box. Subsequent ordering happens 
    /// sequentially from top to bottom or from right to left, depending on the value of 
    /// the Orientation property.
    /// </summary>
    [System.ComponentModel.DefaultProperty("Orientation")]
    public class VirtualizingWrapPanel : VirtualizingPanel, IScrollInfo, IPanelKeyboardHelper
    {
        /// <summary>
        /// Identifies the ItemHeight dependency property.
        /// </summary>
        public static readonly DependencyProperty ItemHeightProperty =
            DependencyProperty.Register("ItemHeight",
                typeof(double),
                typeof(VirtualizingWrapPanel),
                new PropertyMetadata(100d, OnAppearancePropertyChanged));

        /// <summary>
        /// Identifies the Orientation dependency property.
        /// </summary>
        public static readonly DependencyProperty OrientationProperty =
            DependencyProperty.Register("Orientation",
                typeof(Orientation),
                typeof(VirtualizingWrapPanel),
                new PropertyMetadata(Orientation.Horizontal, OnAppearancePropertyChanged));

        /// <summary>
        /// Identifies the ItemWidth dependency property.
        /// </summary>
        public static readonly DependencyProperty ItemWidthProperty =
            DependencyProperty.Register("ItemWidth",
                typeof(double),
                typeof(VirtualizingWrapPanel),
                new PropertyMetadata(100d, OnAppearancePropertyChanged));

        /// <summary>
        /// Identifies the ScrollStep dependency property.
        /// </summary>
        public static readonly DependencyProperty ScrollStepProperty =
            DependencyProperty.Register("ScrollStep",
                typeof(double),
                typeof(VirtualizingWrapPanel),
                new PropertyMetadata(10d, OnAppearancePropertyChanged));

        private int itemsCount;
        private bool canHorizontallyScroll = false;
        private bool canVerticallyScroll = false;
        private Size contentExtent = new Size(0, 0);
        private Point contentOffset = new Point();
        private ScrollViewer scrollOwner;
        private Size viewport = new Size(0, 0);
        private int previousItemCount;

        /// <summary>
        /// Gets or sets a value that specifies the height of all items that are 
        /// contained within a VirtualizingWrapPanel. This is a dependency property.
        /// </summary>
        public double ItemHeight
        {
            get
            {
                return (double)GetValue(ItemHeightProperty);
            }
            set
            {
                SetValue(ItemHeightProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value that specifies the width of all items that are 
        /// contained within a VirtualizingWrapPanel. This is a dependency property.
        /// </summary>
        public double ItemWidth
        {
            get
            {
                return (double)GetValue(ItemWidthProperty);
            }
            set
            {
                SetValue(ItemWidthProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value that specifies the dimension in which child 
        /// content is arranged. This is a dependency property.
        /// </summary>
        public Orientation Orientation
        {
            get
            {
                return (Orientation)this.GetValue(OrientationProperty);
            }
            set
            {
                this.SetValue(OrientationProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value that indicates whether scrolling on the horizontal axis is possible.
        /// </summary>
        public bool CanHorizontallyScroll
        {
            get
            {
                return this.canHorizontallyScroll;
            }
            set
            {
                if (this.canHorizontallyScroll != value)
                {
                    this.canHorizontallyScroll = value;

                    this.InvalidateMeasure();
                }
            }
        }

        /// <summary>
        /// Gets or sets a value that indicates whether scrolling on the vertical axis is possible.
        /// </summary>
        public bool CanVerticallyScroll
        {
            get
            {
                return canVerticallyScroll;
            }
            set
            {
                if (this.canVerticallyScroll != value)
                {
                    this.canVerticallyScroll = value;

                    this.InvalidateMeasure();
                }
            }
        }

        /// <summary>
        /// Gets or sets a ScrollViewer element that controls scrolling behavior.
        /// </summary>
        public ScrollViewer ScrollOwner
        {
            get
            {
                return this.scrollOwner;
            }
            set
            {
                this.scrollOwner = value;
            }
        }

        /// <summary>
        /// Gets the vertical offset of the scrolled content.
        /// </summary>
        public double VerticalOffset
        {
            get
            {
                return this.contentOffset.Y;
            }
        }

        /// <summary>
        /// Gets the vertical size of the viewport for this content.
        /// </summary>
        public double ViewportHeight
        {
            get
            {
                return this.viewport.Height;
            }
        }

        /// <summary>
        /// Gets the horizontal size of the viewport for this content.
        /// </summary>
        public double ViewportWidth
        {
            get
            {
                return this.viewport.Width;
            }
        }

        /// <summary>
        /// Gets or sets a value for mouse wheel scroll step.
        /// </summary>
        public double ScrollStep
        {
            get
            {
                return (double)this.GetValue(ScrollStepProperty);
            }
            set
            {
                SetValue(ScrollStepProperty, value);
            }
        }

        /// <summary>
        /// Gets the vertical size of the extent.
        /// </summary>
        public double ExtentHeight
        {
            get
            {
                return this.contentExtent.Height;
            }
        }

        /// <summary>
        /// Gets the horizontal size of the extent.
        /// </summary>
        public double ExtentWidth
        {
            get
            {
                return this.contentExtent.Width;
            }
        }

        /// <summary>
        /// Gets the horizontal offset of the scrolled content.
        /// </summary>
        public double HorizontalOffset
        {
            get
            {
                return this.contentOffset.X;
            }
        }

        IPanelHelper IPanelKeyboardHelper.PanelHelper
        {
            get;
            set;
        }

        /// <summary>
        /// Scrolls down within content by one logical unit.
        /// </summary>
        public void LineDown()
        {
            this.SetVerticalOffset(this.VerticalOffset + this.ScrollStep);
        }

        /// <summary>
        /// Scrolls left within content by one logical unit.
        /// </summary>
        public void LineLeft()
        {
            this.SetHorizontalOffset(this.HorizontalOffset - this.ScrollStep);
        }

        /// <summary>
        /// Scrolls right within content by one logical unit.
        /// </summary>
        public void LineRight()
        {
            this.SetHorizontalOffset(this.HorizontalOffset + this.ScrollStep);
        }

        /// <summary>
        /// Scrolls up within content by one logical unit.
        /// </summary>
        public void LineUp()
        {
            this.SetVerticalOffset(this.VerticalOffset - this.ScrollStep);
        }


        /// <summary>
        /// Forces content to scroll until the coordinate space of a Visual object is visible.
        /// </summary>
        public Rect MakeVisible(Visual visual, Rect rectangle)
        {
            this.MakeVisible(visual as UIElement);

            return rectangle;
        }

        /// <summary>
        /// Scrolls down within content after a user clicks the wheel button on a mouse.
        /// </summary>
        public void MouseWheelDown()
        {
            this.SetVerticalOffset(this.VerticalOffset + this.ScrollStep);
        }

        /// <summary>
        /// Scrolls left within content after a user clicks the wheel button on a mouse.
        /// </summary>
        public void MouseWheelLeft()
        {
            this.SetHorizontalOffset(this.HorizontalOffset - this.ScrollStep);
        }

        /// <summary>
        /// Scrolls right within content after a user clicks the wheel button on a mouse.
        /// </summary>
        public void MouseWheelRight()
        {
            this.SetHorizontalOffset(this.HorizontalOffset + this.ScrollStep);
        }

        /// <summary>
        /// Scrolls up within content after a user clicks the wheel button on a mouse.
        /// </summary>
        public void MouseWheelUp()
        {
            this.SetVerticalOffset(this.VerticalOffset - this.ScrollStep);
        }

        /// <summary>
        /// Scrolls down within content by one page.
        /// </summary>
        public void PageDown()
        {
            this.SetVerticalOffset(this.VerticalOffset + this.ViewportHeight);
        }

        /// <summary>
        /// Scrolls left within content by one page.
        /// </summary>
        public void PageLeft()
        {
            this.SetHorizontalOffset(this.HorizontalOffset - this.ViewportHeight);
        }

        /// <summary>
        /// Scrolls right within content by one page.
        /// </summary>
        public void PageRight()
        {
            this.SetHorizontalOffset(this.HorizontalOffset + this.ViewportHeight);
        }

        /// <summary>
        /// Scrolls up within content by one page.
        /// </summary>
        public void PageUp()
        {
            this.SetVerticalOffset(this.VerticalOffset - this.viewport.Height);
        }

        /// <summary>
        /// Sets the amount of vertical offset.
        /// </summary>
        public void SetVerticalOffset(double offset)
        {
            if (offset < 0 || this.ViewportHeight >= this.ExtentHeight)
            {
                offset = 0;
            }
            else
            {
                if (offset + this.ViewportHeight >= this.ExtentHeight)
                {
                    offset = this.ExtentHeight - this.ViewportHeight;
                }
            }

            this.contentOffset.Y = offset;

            if (this.ScrollOwner != null)
            {
                this.ScrollOwner.InvalidateScrollInfo();
            }

            this.InvalidateMeasure();
        }

        /// <summary>
        /// Sets the amount of horizontal offset.
        /// </summary>
        public void SetHorizontalOffset(double offset)
        {
            if (offset < 0 || this.ViewportWidth >= this.ExtentWidth)
            {
                offset = 0;
            }
            else
            {
                if (offset + this.ViewportWidth >= this.ExtentWidth)
                {
                    offset = this.ExtentWidth - this.ViewportWidth;
                }
            }

            this.contentOffset.X = offset;

            if (this.ScrollOwner != null)
            {
                this.ScrollOwner.InvalidateScrollInfo();
            }

            this.InvalidateMeasure();
        }

        /// <summary>
        /// Note: Works only for vertical.
        /// </summary>
        internal void PageLast()
        {
            this.contentOffset.Y = this.ExtentHeight;

            if (this.ScrollOwner != null)
            {
                this.ScrollOwner.InvalidateScrollInfo();
            }

            this.InvalidateMeasure();
        }

        /// <summary>
        /// Note: Works only for vertical.
        /// </summary>
        internal void PageFirst()
        {
            this.contentOffset.Y = 0.0d;

            if (this.ScrollOwner != null)
            {
                this.ScrollOwner.InvalidateScrollInfo();
            }

            this.InvalidateMeasure();
        }

        /// <summary>
        /// When items are removed, remove the corresponding UI if necessary.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        protected override void OnItemsChanged(object sender, ItemsChangedEventArgs args)
        {
            switch (args.Action)
            {
                case NotifyCollectionChangedAction.Remove:
                case NotifyCollectionChangedAction.Replace:

                case NotifyCollectionChangedAction.Move:

                    RemoveInternalChildRange(args.Position.Index, args.ItemUICount);
                    break;
                case NotifyCollectionChangedAction.Reset:

                    var itemsControl = System.Windows.Controls.ItemsControl.GetItemsOwner(this);
                    if (itemsControl != null)
                    {
                        if (previousItemCount != itemsControl.Items.Count)
                        {
                            if (this.Orientation == System.Windows.Controls.Orientation.Horizontal)
                            {
                                this.SetVerticalOffset(0);
                            }
                            else
                            {
                                this.SetHorizontalOffset(0);
                            }
                        }

                        previousItemCount = itemsControl.Items.Count;
                    }

                    break;
            }
        }

        /// <summary>
        /// Measure the children.
        /// </summary>
        /// <param name="availableSize">The available size.</param>
        /// <returns>The desired size.</returns>
        protected override Size MeasureOverride(Size availableSize)
        {
            this.InvalidateScrollInfo(availableSize);

            int firstVisibleIndex, lastVisibleIndex;

            if (this.Orientation == System.Windows.Controls.Orientation.Horizontal)
            {
                this.GetVerticalVisibleRange(out firstVisibleIndex, out lastVisibleIndex);
            }
            else
            {
                this.GetHorizontalVisibleRange(out firstVisibleIndex, out lastVisibleIndex);
            }

            var children = this.Children;
            var generator = this.ItemContainerGenerator;

            if (generator != null)
            {
                var startPos = generator.GeneratorPositionFromIndex(firstVisibleIndex);

                var childIndex = (startPos.Offset == 0) ? startPos.Index : startPos.Index + 1;

                if (childIndex == -1)
                {
                    this.RefreshOffset();
                }

                using (generator.StartAt(startPos, GeneratorDirection.Forward, true))
                {
                    for (var itemIndex = firstVisibleIndex; itemIndex <= lastVisibleIndex; itemIndex++, childIndex++)
                    {
                        bool newlyRealized;

                        var child = generator.GenerateNext(out newlyRealized) as UIElement;
                        if (newlyRealized)
                        {
                            if (childIndex >= children.Count)
                            {
                                this.AddInternalChild(child);
                            }
                            else
                            {
                                this.InsertInternalChild(childIndex, child);
                            }

                            generator.PrepareItemContainer(child);
                        }

                        if (child != null)
                        {
                            child.Measure(new Size(this.ItemWidth, this.ItemHeight));
                        }
                    }
                }

                this.CleanUpChildren(firstVisibleIndex, lastVisibleIndex);
            }

            if (availableSize.Height.IsCloseTo(double.PositiveInfinity) || availableSize.Width.IsCloseTo(double.PositiveInfinity))
            {
                return base.MeasureOverride(availableSize);
            }

            return availableSize;
        }

        /// <summary>
        /// Arranges the children.
        /// </summary>
        /// <param name="finalSize">The available size.</param>
        /// <returns>The used size.</returns>
        protected override Size ArrangeOverride(Size finalSize)
        {
            var isHorizontal = this.Orientation == System.Windows.Controls.Orientation.Horizontal;

            this.InvalidateScrollInfo(finalSize);
            int i = 0;

            foreach (var item in this.Children)
            {
                this.ArrangeChild(isHorizontal, finalSize, i++, item as UIElement);
            }

            return finalSize;
        }

        private static void OnAppearancePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var panel = d as UIElement;
            if (panel != null)
            {
                panel.InvalidateMeasure();
            }
        }

        private void MakeVisible(UIElement element)
        {
            var generator = this.ItemContainerGenerator.GetItemContainerGeneratorForPanel(this);

            if (element != null && generator != null)
            {
                int itemIndex = generator.IndexFromContainer(element);

                // Try to get the real item if the current is some child of the real item
                while (itemIndex == -1)
                {
                    element = element.ParentOfType<UIElement>();
                    itemIndex = generator.IndexFromContainer(element);
                }

                var scrollViewer = element.ParentOfType<ScrollViewer>();
                if (scrollViewer != null)
                {
                    var elementTransform = element.TransformToVisual(scrollViewer);
                    var elementRectangle = elementTransform.TransformBounds(new Rect(new Point(0, 0), element.RenderSize));

                    if (Orientation == Orientation.Horizontal)
                    {
                        if (elementRectangle.Bottom > this.ViewportHeight)
                        {
                            this.SetVerticalOffset(contentOffset.Y + elementRectangle.Bottom - this.ViewportHeight);
                        }
                        else if (elementRectangle.Top < 0)
                        {
                            this.SetVerticalOffset(contentOffset.Y + elementRectangle.Top);
                        }
                    }
                    else
                    {
                        if (elementRectangle.Right > this.ViewportWidth)
                        {
                            this.SetHorizontalOffset(contentOffset.X + elementRectangle.Right - this.ViewportWidth);
                        }
                        else if (elementRectangle.Left < 0)
                        {
                            this.SetHorizontalOffset(contentOffset.X + elementRectangle.Left);
                        }
                    }
                }
            }
        }

        private void GetVerticalVisibleRange(out int firstVisibleItemIndex, out int lastVisibleItemIndex)
        {
            var childrenPerRow = this.GetVerticalChildrenCountPerRow(contentExtent);

            firstVisibleItemIndex = (int)Math.Floor(this.VerticalOffset / this.ItemHeight) * childrenPerRow;
            lastVisibleItemIndex = ((int)Math.Ceiling((this.VerticalOffset + this.ViewportHeight) / this.ItemHeight) * childrenPerRow) - 1;

            AdjustVisibleRange(ref firstVisibleItemIndex, ref lastVisibleItemIndex);
        }

        private void GetHorizontalVisibleRange(out int firstVisibleItemIndex, out int lastVisibleItemIndex)
        {
            var childrenPerRow = this.GetHorizontalChildrenCountPerRow(contentExtent);

            firstVisibleItemIndex = (int)Math.Floor(this.HorizontalOffset / this.ItemWidth) * childrenPerRow;
            lastVisibleItemIndex = ((int)Math.Ceiling((this.HorizontalOffset + this.ViewportWidth) / this.ItemWidth) * childrenPerRow) - 1;

            AdjustVisibleRange(ref firstVisibleItemIndex, ref lastVisibleItemIndex);
        }

        private void AdjustVisibleRange(ref int firstVisibleItemIndex, ref int lastVisibleItemIndex)
        {
            firstVisibleItemIndex--;
            lastVisibleItemIndex++;

            var itemsControl = System.Windows.Controls.ItemsControl.GetItemsOwner(this);

            if (itemsControl != null)
            {
                if (firstVisibleItemIndex < 0)
                {
                    firstVisibleItemIndex = 0;
                }

                if (lastVisibleItemIndex >= itemsControl.Items.Count)
                {
                    lastVisibleItemIndex = itemsControl.Items.Count - 1;
                }
            }
        }

        private void CleanUpChildren(int minIndex, int maxIndex)
        {
            var children = this.Children;
            var generator = this.ItemContainerGenerator;

            for (var i = children.Count - 1; i >= 0; i--)
            {
                var pos = new GeneratorPosition(i, 0);
                var itemIndex = generator.IndexFromGeneratorPosition(pos);
                if (itemIndex < minIndex || itemIndex > maxIndex)
                {
                    generator.Remove(pos, 1);
                    RemoveInternalChildRange(i, 1);
                }
            }
        }

        private void ArrangeChild(bool isHorizontal, Size finalSize, int index, UIElement child)
        {
            if (child == null)
                return;

            var count = isHorizontal ? this.GetVerticalChildrenCountPerRow(finalSize) : this.GetHorizontalChildrenCountPerRow(finalSize);
            var itemIndex = this.ItemContainerGenerator.IndexFromGeneratorPosition(new GeneratorPosition(index, 0));

            var row = isHorizontal ? itemIndex / count : itemIndex % count;
            var column = isHorizontal ? itemIndex % count : itemIndex / count;

            var rect = new Rect(column * this.ItemWidth, row * this.ItemHeight, this.ItemWidth, this.ItemHeight);

            if (isHorizontal)
            {
                rect.Y -= this.VerticalOffset;
            }
            else
            {
                rect.X -= this.HorizontalOffset;
            }

            child.Arrange(rect);
        }

        private void InvalidateScrollInfo(Size availableSize)
        {
            var ownerItemsControl = System.Windows.Controls.ItemsControl.GetItemsOwner(this);

            if (ownerItemsControl != null)
            {
                itemsCount = ownerItemsControl.Items.Count;
                var extent = this.GetExtent(availableSize, itemsCount);

                if (extent != this.contentExtent)
                {
                    this.contentExtent = extent;
                    this.RefreshOffset();
                }

                if (availableSize != viewport)
                {
                    this.viewport = availableSize;

                    this.InvalidateScrollOwner();
                    this.RefreshOffset();
                }
            }
        }

        private void RefreshOffset()
        {
            if (this.Orientation == System.Windows.Controls.Orientation.Horizontal)
            {
                this.SetVerticalOffset(this.VerticalOffset);
            }
            else
            {
                this.SetHorizontalOffset(this.HorizontalOffset);
            }
        }

        private void InvalidateScrollOwner()
        {
            if (this.ScrollOwner != null)
            {
                this.ScrollOwner.InvalidateScrollInfo();
            }
        }

        private Size GetExtent(Size availableSize, int itemCount)
        {
            if (this.Orientation == System.Windows.Controls.Orientation.Horizontal)
            {
                var childrenPerRow = this.GetVerticalChildrenCountPerRow(availableSize);

                return new Size(childrenPerRow * this.ItemWidth,
                    this.ItemHeight * Math.Ceiling((double)itemCount / childrenPerRow));
            }
            else
            {
                var childrenPerRow = this.GetHorizontalChildrenCountPerRow(availableSize);

                return new Size(this.ItemWidth * Math.Ceiling((double)itemCount / childrenPerRow),
                    childrenPerRow * this.ItemHeight);
            }
        }

        private int GetVerticalChildrenCountPerRow(Size availableSize)
        {
            var childrenCountPerRow = 0;

            if (availableSize.Width == double.PositiveInfinity)
            {
                childrenCountPerRow = this.Children.Count;
            }
            else
            {
                childrenCountPerRow = Math.Max(1, (int)Math.Floor(availableSize.Width / this.ItemWidth));
            }

            return childrenCountPerRow;
        }

        private int GetHorizontalChildrenCountPerRow(Size availableSize)
        {
            var childrenCountPerRow = 0;

            if (availableSize.Height == double.PositiveInfinity)
            {
                childrenCountPerRow = this.Children.Count;
            }
            else
            {
                childrenCountPerRow = Math.Max(1, (int)Math.Floor(availableSize.Height / this.ItemHeight));
            }

            return childrenCountPerRow;
        }

        Point IPanelKeyboardHelper.GetOffsets(int index)
        {
            var firstVisibleContainer = this.GetFirstContainerInViewport();
            var lastVisibleContainer = this.GetLastContainerInViewport();
            if (firstVisibleContainer != null && lastVisibleContainer != null)
            {
                var startIndex = ((System.Windows.Controls.ItemContainerGenerator)this.ItemContainerGenerator).IndexFromContainer(firstVisibleContainer);
                var lastIndex = ((System.Windows.Controls.ItemContainerGenerator)this.ItemContainerGenerator).IndexFromContainer(lastVisibleContainer);
                if (index >= startIndex && index <= lastIndex)
                {
                    return new Point(this.HorizontalOffset, this.VerticalOffset);
                }
            }

            var rowVertIndex = (int)(index / this.GetVerticalChildrenCountPerRow(this.viewport));
            var verticalOffset = rowVertIndex * this.ItemHeight;
            var horizontalOffset = rowVertIndex * this.ItemWidth;

            var point = new Point(horizontalOffset, verticalOffset);
            if (verticalOffset + this.ItemHeight > this.VerticalOffset + this.ViewportHeight)
            {
                point.Y = verticalOffset - this.ViewportHeight + this.ItemHeight;
            }

            if (verticalOffset + this.ItemWidth > this.HorizontalOffset + this.ViewportWidth)
            {
                point.X = horizontalOffset - this.ViewportWidth + this.ItemWidth;
            }

            return point;
        }

        int IPanelKeyboardHelper.GetPageUpIndex(int fromIndex)
        {
            var firstVisibleContainer = this.GetFirstContainerInViewport();
            var lastVisibleContainer = this.GetLastContainerInViewport();
            if (firstVisibleContainer != null && lastVisibleContainer != null)
            {
                var startIndex = ((System.Windows.Controls.ItemContainerGenerator)this.ItemContainerGenerator).IndexFromContainer(firstVisibleContainer);
                var lastIndex = ((System.Windows.Controls.ItemContainerGenerator)this.ItemContainerGenerator).IndexFromContainer(lastVisibleContainer);
                if (startIndex != fromIndex)
                {
                    return startIndex;
                }
            }
            var rowCount = this.GetHorizontalChildrenCountPerRow(this.viewport);
            var columnCount = this.GetVerticalChildrenCountPerRow(this.viewport);
            var index = fromIndex - (rowCount * columnCount);
            return index < 0 ? 0 : index;
        }

        int IPanelKeyboardHelper.GetPageDownIndex(int fromIndex)
        {
            var firstVisibleContainer = this.GetFirstContainerInViewport();
            var lastVisibleContainer = this.GetLastContainerInViewport();
            if (firstVisibleContainer != null && lastVisibleContainer != null)
            {
                var startIndex = ((System.Windows.Controls.ItemContainerGenerator)this.ItemContainerGenerator).IndexFromContainer(firstVisibleContainer);
                var lastIndex = ((System.Windows.Controls.ItemContainerGenerator)this.ItemContainerGenerator).IndexFromContainer(lastVisibleContainer);

                if (lastIndex != fromIndex)
                {
                    return lastIndex;
                }
            }
            var rowCount = this.GetHorizontalChildrenCountPerRow(this.viewport);
            var columnCount = this.GetVerticalChildrenCountPerRow(this.viewport);
            var index = fromIndex + (rowCount * columnCount);
            return index > this.itemsCount - 1 ? this.itemsCount - 1 : index;
        }

        private bool IsInTheViewport(FrameworkElement item)
        {
            if (item == null)
            {
                return false;
            }

            var slot = (this as IPanelKeyboardHelper).PanelHelper.GetLayoutSlot(item);
            return slot.Y >= 0 &&
                slot.Height + slot.Y <= this.ViewportHeight &&
                slot.X >= 0 &&
                slot.Width + slot.X <= this.ViewportWidth;
        }

        private FrameworkElement GetFirstContainerInViewport()
        {
            return this.Children.Cast<FrameworkElement>().FirstOrDefault(item => this.IsInTheViewport(item));
        }

        private FrameworkElement GetLastContainerInViewport()
        {
            return this.Children.Cast<FrameworkElement>().LastOrDefault(item => this.IsInTheViewport(item));
        }
    }
}