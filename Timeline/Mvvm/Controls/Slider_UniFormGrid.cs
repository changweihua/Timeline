/*介绍
 * 本控件实现的是类似智能手机中的菜单滑动和停止时缓冲效果


使用方法：

引用bin目录中的dll 或者 直接添加sliderControl到你的工程 

此控件集成UniformGrid向控件加添内容会自动排列


注意事项：
1.此控件必须有父容器，例如grid  ，canvas等
父容器必须设置大小（宽，高）

2.需设置 父容器 ClipToBounds属性 或者 此控件ParentClipToBounds属性  ，
ParentClipToBounds属性已默认为true，即超出父容器部分，不显示

3.需要设置 _CanLeftAndRight="True"  可左右滑动 
   _CanUpAndDown="True" 可上下滑动
   _SliderSpeed="0.95"  滑动速度   必须项
    Columns="3" 列
    Rows = "3" 行

4.如此控件内容大小 已超过父容器，滑动才有效
   例如 此控件中只有一张100*100的图片 而父容器的大小是200*200，则无法滑动
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls.Primitives;
using System.Windows;
using System.Windows.Media;
using System.Windows.Input;
using System.Windows.Controls;
using System.Collections.ObjectModel;
using System.Diagnostics;
using Timeline.ToolClasses;

namespace ShiningMeeting.Mvvm.Controls
{
    public class SliderSelectionChangedArgs : EventArgs
    {
        public FrameworkElement FrameworkElement { get; set; }
        public Point StartPoint { get; set; }
    }

    public class Slider_UniFormGrid : UniformGrid
    {
        public event EventHandler<SliderSelectionChangedArgs> SelectionChanged;

        #region Internal Members

        protected double oldX;
        protected double oldY;
        protected double newX;
        protected double newY;

        protected bool isMove;
        protected bool MoveLR;
        public bool MoveUD;

        protected double DvalueX;
        protected double DvalueY;
        protected double left;
        protected double top;
        protected double rollBack;

        protected MoveTypeLeftOrRight moveTypeLeftOrRight = MoveTypeLeftOrRight.None;
        public MoveTypeUpOrDown moveTypeUpOrDown = MoveTypeUpOrDown.None;


        private FrameworkElement objectFrameworkElement;//将要选中,将被改变背景的元素
        private FrameworkElement ChangeColorFrameElement;//当前被选中已经改变背景的元素
        private Point UpStartPoint;

        #endregion

        #region External Members

        private bool CanLR;//是否允许左右滑动
        private bool CanUD;//是否允许上下滑动
        private bool canSelect = default(bool);//子项是否允许被选中

        private bool ParentClipToBounds = true;//父容器裁剪状态,用于隐藏超出部分

        private double SliderSpeed = 0.95;//滑动速率

        private Thickness margin = new Thickness(0);//设置margin属性

        private SolidColorBrush BackGroundColor = Brushes.Transparent;//背景颜色，默认透明

        private Brush selectedBorderBrushes = Brushes.Transparent;

        private Brush selectedBorderBackground = Brushes.Transparent;

        private CornerRadius itemCornerRadius = new CornerRadius(0);

        private Thickness itemBorderThicness = new Thickness(0);

        private FrameworkElement selectedItem;

        #endregion

        //被选中项
        public FrameworkElement SelectedItem
        {
            get { return selectedItem; }
            set
            {
                selectedItem = value;
                if (SelectionChanged != null)
                {
                    SliderSelectionChangedArgs args = new SliderSelectionChangedArgs() { FrameworkElement = SelectedItem, StartPoint = UpStartPoint };
                    SelectionChanged(this, args);
                }

                if (SelectedItem != null)
                {
                    Border element = VisualTreeHelper.GetParent(SelectedItem) as Border;
                    if (element != null)
                    {
                        element.BorderBrush = selectedBorderBrushes;
                        element.Background = selectedBorderBackground;
                        element.CornerRadius = itemCornerRadius;

                        if (ChangeColorFrameElement != null && !object.ReferenceEquals(ChangeColorFrameElement, element))
                        {
                            (ChangeColorFrameElement as Border).BorderBrush = Brushes.Transparent;
                            (ChangeColorFrameElement as Border).Background = Brushes.Transparent;
                            (ChangeColorFrameElement as Border).CornerRadius = new CornerRadius(0);
                        }
                        ChangeColorFrameElement = element;
                    }
                }

            }
        }

        //子项边框厚度
        public Thickness _ItemBorderThicness
        {
            get { return itemBorderThicness; }
            set { itemBorderThicness = value; }
        }

        #region Propertites

        //子项边框弧度
        public CornerRadius _ItemCornerRadius
        {
            get { return itemCornerRadius; }
            set { itemCornerRadius = value; }
        }

        //子项被选中时的背景颜色
        public Brush _SelectedBorderBackground
        {
            get { return selectedBorderBackground; }
            set { selectedBorderBackground = value; }
        }

        //子项被选中时的边框颜色
        public Brush _SelectedBorderBrushes
        {
            get { return selectedBorderBrushes; }
            set { selectedBorderBrushes = value; }
        }

        //是否允许被选中
        public bool _CanSelect
        {
            get { return canSelect; }
            set { canSelect = value; }
        }



        //子项集合
        public ObservableCollection<FrameworkElement> _ItemsSource
        {
            get { return (ObservableCollection<FrameworkElement>)GetValue(_ItemsSourceProperty); }
            set { SetValue(_ItemsSourceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for _ItemsSource.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty _ItemsSourceProperty =
            DependencyProperty.Register("_ItemsSource", typeof(ObservableCollection<FrameworkElement>), typeof(Slider_UniFormGrid), new UIPropertyMetadata(null));

        /// CanLR 是否允许左右滑动
        public bool _CanLeftAndRight
        {
            get { return CanLR; }
            set { CanLR = value; }
        }

        //CanUD 是否允许上下滑动
        public bool _CanUpAndDown
        {
            get { return CanUD; }
            set { CanUD = value; }
        }

        //滑动速率 外部定义
        public double _SliderSpeed
        {
            get { return SliderSpeed; }
            set { SliderSpeed = value; }
        }

        //背景颜色
        public SolidColorBrush _BackGroundColor
        {
            get { return BackGroundColor; }
            set { BackGroundColor = value; }
        }

        //父容器裁剪状态
        public bool _ParentClipToBounds
        {
            get { return ParentClipToBounds; }
            set { ParentClipToBounds = value; }
        }

        //设置margin属性
        public Thickness _Margin
        {
            get { return margin; }
            set { margin = value; }
        }

        #endregion

        #region Constructor

        public Slider_UniFormGrid()
        {
            this.Loaded += new System.Windows.RoutedEventHandler(Slider_UniFormGrid_Loaded);//加载完成

        }

        #endregion

        #region Function

        public void AddObject(ObservableCollection<FrameworkElement> FList)
        {
            if (FList == null || FList.Count == 0) return;
            foreach (FrameworkElement F in FList)
            {
                AddObject(F);
            }
        }
        public void AddObject(FrameworkElement F)
        {
            Border border = new Border() { Margin = new Thickness(20, 10, 0, 0) };
            border.Width = 150;
            border.Child = F;
            F.Margin = new Thickness(0,0,0,0);
            F.Width = ConstClass.APPWIDTH * 0.1;
            border.BorderThickness = itemBorderThicness;
            border.BorderBrush = Brushes.Transparent;
            border.Background = Brushes.Transparent;
            border.CornerRadius = new CornerRadius(0);
            border.AddHandler(FrameworkElement.MouseDownEvent, new MouseButtonEventHandler(FrameworkElement_MouseDown), true);
            border.AddHandler(FrameworkElement.MouseUpEvent, new MouseButtonEventHandler(FrameworkElement_MouseUp), true);
            border.AddHandler(FrameworkElement.MouseMoveEvent, new MouseEventHandler(FrameworkElement_MouseMove), true);
            border.LostFocus += delegate
            {
                if (ChangeColorFrameElement != null)
                {
                    (ChangeColorFrameElement as Border).BorderBrush = Brushes.Transparent;
                    (ChangeColorFrameElement as Border).Background = Brushes.Transparent;
                    (ChangeColorFrameElement as Border).CornerRadius = new CornerRadius(0);
                }
            };
            this.Children.Add(border);
        }

        public void Clear()
        {
            this.Children.Clear();
        }

        void FrameworkElement_MouseDown(object sender, MouseButtonEventArgs e)
        {
            objectFrameworkElement = sender as FrameworkElement;

        }

        void FrameworkElement_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if ((sender as FrameworkElement) == objectFrameworkElement)
            {
                //if (ChangeColorFrameElement != null)
                //{
                //    ChangeColorFrameElement.Focus();
                //    (ChangeColorFrameElement as Border).BorderBrush = Brushes.Transparent;
                //    (ChangeColorFrameElement as Border).Background = Brushes.Transparent;
                //    (ChangeColorFrameElement as Border).CornerRadius = new CornerRadius(0);
                //}

                //(objectFrameworkElement as Border).BorderBrush = selectedBorderBrushes;
                //(objectFrameworkElement as Border).Background = selectedBorderBackground;
                //(objectFrameworkElement as Border).CornerRadius = itemCornerRadius;

                //ChangeColorFrameElement = objectFrameworkElement;
                UpStartPoint = e.GetPosition(null);
                SelectedItem = (objectFrameworkElement as Border).Child as FrameworkElement;
            }
        }

        void FrameworkElement_MouseMove(object sender, MouseEventArgs e)
        {
                //objectFrameworkElement = null;
        }

        void Slider_UniFormGrid_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            this.Background = _BackGroundColor;//初始化背景,默认透明
            this.Margin = _Margin;//初始化margin,默认0
            this.Parent.SetValue(ClipToBoundsProperty, _ParentClipToBounds);//设置父容器裁剪状态，隐藏超出部分,默认true

            //对齐方式
            // this.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            //this.VerticalAlignment = System.Windows.VerticalAlignment.Top;

            //注册事件
            CompositionTarget.Rendering += new EventHandler(CompositionTarget_Rendering);
            this.PreviewMouseDown += new System.Windows.Input.MouseButtonEventHandler(Slider_UniFormGrid_MouseDown);
            this.PreviewMouseMove += new System.Windows.Input.MouseEventHandler(Slider_UniFormGrid_MouseMove);
            this.PreviewMouseUp += new System.Windows.Input.MouseButtonEventHandler(Slider_UniFormGrid_MouseUp);

            this.PreviewTouchDown += new EventHandler<TouchEventArgs>(Slider_UniFormGrid_PreviewTouchDown);
            this.PreviewTouchMove += new EventHandler<TouchEventArgs>(Slider_UniFormGrid_PreviewTouchMove);


            AddObject(_ItemsSource);

            if (_ItemsSource != null)
            {
                _ItemsSource.CollectionChanged += (s, args) =>
                {
                    if (args.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
                    {
                        AddObject(args.NewItems[0] as FrameworkElement);
                    }
                };
            }

            if (SelectedItem != null)
            {
                Border element = VisualTreeHelper.GetParent(SelectedItem) as Border;
                element.BorderBrush = selectedBorderBrushes;
                element.Background = selectedBorderBackground;
                element.CornerRadius = itemCornerRadius;
                ChangeColorFrameElement = element;
            }
        }


        void Slider_UniFormGrid_PreviewTouchMove(object sender, TouchEventArgs e)
        {
            newX = e.GetTouchPoint(null).Position.X;
            newY = e.GetTouchPoint(null).Position.Y;
            Point p = e.GetTouchPoint(null).Position;
            if (newX > oldX)
            {
                moveTypeLeftOrRight = MoveTypeLeftOrRight.Right;
                MoveLR = true;
                isMove = true;
            }
            else if (newX < oldX)
            {
                moveTypeLeftOrRight = MoveTypeLeftOrRight.Left;
                MoveLR = true;
                isMove = true;
            }

            if (newY < oldY)
            {
                moveTypeUpOrDown = MoveTypeUpOrDown.Up;
                MoveUD = true;
                isMove = true;
            }
            else if (newY > oldY)
            {
                moveTypeUpOrDown = MoveTypeUpOrDown.Down;
                MoveUD = true;
                isMove = true;
            }
            //this.CaptureMouse();
            DvalueX = newX - oldX;
            DvalueY = newY - oldY;
            oldX = newX;
            oldY = newY;
        }

        void Slider_UniFormGrid_PreviewTouchDown(object sender, TouchEventArgs e)
        {
            oldX = e.GetTouchPoint(null).Position.X;
            oldY = e.GetTouchPoint(null).Position.Y;
        }

        public bool CheckMove = true;
        ///滑动操作执行步骤
        void CompositionTarget_Rendering(object sender, EventArgs e)
        {
            if (CheckMove)
            {
                moveStatus();

                if (!isMove)
                    return;
            }

            if (_CanLeftAndRight)
            {
                Debug.Print(((double)this.GetValue(ActualWidthProperty)).ToString());
                Debug.Print(((double)this.Parent.GetValue(ActualWidthProperty)).ToString());
                if ((double)this.GetValue(ActualWidthProperty) > (double)this.Parent.GetValue(ActualWidthProperty))
                {
                    if (MoveLR)
                        if (moveTypeLeftOrRight == MoveTypeLeftOrRight.Left)
                        {
                            DvalueX = DvalueX * _SliderSpeed;
                            if (this.Margin.Left + this.ActualWidth > (double)this.Parent.GetValue(ActualWidthProperty) - 5)
                            {
                                if (Math.Abs(DvalueX) < 0.1)
                                {
                                    MoveLR = false;
                                }
                                left = this.Margin.Left + DvalueX;
                                //this.Margin = new Thickness(this.Margin.Left + DvalueX, 0, 0, 0);//设置新Margin
                            }
                            else
                            {

                                rollBack = this.Margin.Left - ((double)this.Parent.GetValue(ActualWidthProperty) - this.ActualWidth);
                                rollBack *= 0.07;


                                if (this.Margin.Left - rollBack > (double)this.Parent.GetValue(ActualWidthProperty) - 5 - this.ActualWidth)
                                {
                                    left = (double)this.Parent.GetValue(ActualWidthProperty) - 5 - this.ActualWidth;
                                    //this.Margin = new Thickness((double)this.Parent.GetValue(WidthProperty) - 5 - this.ActualWidth, top, 0, 0);
                                    MoveLR = false;
                                }
                                left = this.Margin.Left - rollBack;
                                //this.Margin = new Thickness(this.Margin.Left - rollBack, 0, 0, 0);//设置新Margin，递减缓慢向右边缘靠拢
                            }
                        }
                        else if (moveTypeLeftOrRight == MoveTypeLeftOrRight.Right)
                        {
                            DvalueX = DvalueX * _SliderSpeed;
                            if (this.Margin.Left < 5)
                            {
                                if (Math.Abs(DvalueX) < 0.1)
                                {
                                    MoveLR = false;
                                }
                                left = this.Margin.Left + DvalueX;
                                // this.Margin = new Thickness(this.Margin.Left + DvalueX, 0, 0, 0);//设置新margin
                            }
                            else
                            {

                                rollBack = this.Margin.Left;
                                rollBack *= 0.95;
                                if (Math.Abs(rollBack) < 5)
                                {
                                    left = 5;
                                    //this.Margin = new Thickness(5, top, 0, 0);
                                    MoveLR = false;
                                }
                                left = rollBack;
                                //this.Margin = new Thickness(rollBack, 0, 0, 0);//设置新margin
                            }
                        }
                }
                else
                {
                    this.Margin = new Thickness(0, top, 0, 0); return;
                }
            }
            if (_CanUpAndDown)
            {
                if ((double)this.GetValue(ActualHeightProperty) > (double)this.Parent.GetValue(ActualHeightProperty))
                {
                    if (MoveUD)
                        if (moveTypeUpOrDown == MoveTypeUpOrDown.Up)
                        {
                            if (CheckMove)
                                DvalueY = DvalueY * _SliderSpeed;
                            else
                                DvalueY = -3;
                            if (this.Margin.Top + this.ActualHeight > (double)this.Parent.GetValue(ActualHeightProperty))
                            {
                                if (Math.Abs(DvalueY) < 0.1)
                                {
                                    MoveUD = false;
                                }
                                top = this.Margin.Top + DvalueY;
                                //this.Margin = new Thickness(0, this.Margin.Top + DvalueY, 0, 0);//设置新Margin
                            }
                            else
                            {

                                rollBack = this.Margin.Top - ((double)this.Parent.GetValue(ActualHeightProperty) - this.ActualHeight);
                                rollBack *= 0.07;


                                if (this.Margin.Top - rollBack > (double)this.Parent.GetValue(ActualHeightProperty) - this.ActualHeight)
                                {
                                    top = (double)this.Parent.GetValue(ActualHeightProperty) - this.ActualHeight;
                                    //this.Margin = new Thickness(left, (double)this.Parent.GetValue(HeightProperty) - 5 - this.ActualHeight, 0, 0);
                                    MoveUD = false;
                                }
                                top = this.Margin.Top - rollBack;
                                //this.Margin = new Thickness(0, this.Margin.Top - rollBack, 0, 0);//设置新Margin，递减缓慢向右边缘靠拢
                            }
                        }
                        else if (moveTypeUpOrDown == MoveTypeUpOrDown.Down)
                        {
                            if (CheckMove)
                                DvalueY = DvalueY * _SliderSpeed;
                            else
                                DvalueY = 3;
                            if (this.Margin.Top < 0)
                            {
                                if (Math.Abs(DvalueY) < 0.1)
                                {
                                    MoveUD = false;
                                }
                                top = this.Margin.Top + DvalueY;
                                // this.Margin = new Thickness(0, this.Margin.Top + DvalueY, 0, 0);//设置新margin
                            }
                            else
                            {

                                rollBack = this.Margin.Top;
                                rollBack *= 0.95;
                                if (Math.Abs(rollBack) < 0)
                                {
                                    top = 0;
                                    //this.Margin = new Thickness(left, 5, 0, 0);
                                    MoveUD = false;
                                }
                                top = rollBack;
                                //this.Margin = new Thickness(0, rollBack, 0, 0);//设置新margin
                            }
                        }
                }
            }

            this.Margin = new Thickness(left, top, 0, 0);
        }


        void moveStatus()
        {
            if (_CanLeftAndRight && _CanUpAndDown)
            {
                if (!MoveLR && !MoveUD)
                    isMove = false;
                else
                    isMove = true;
            }
            else if (_CanLeftAndRight && !_CanUpAndDown)
            {
                if (!MoveLR)
                    isMove = false;
                else
                    isMove = true;
            }
            else if (!_CanLeftAndRight && _CanUpAndDown)
            {
                if (!MoveUD)
                    isMove = false;
                else
                    isMove = true;
            }
            else
                isMove = false;

        }

        public void UnSelected()
        {
            if (ChangeColorFrameElement != null)
            {
                (ChangeColorFrameElement as Border).BorderBrush = Brushes.Transparent;
                (ChangeColorFrameElement as Border).Background = Brushes.Transparent;
                (ChangeColorFrameElement as Border).CornerRadius = new CornerRadius(0);

                ChangeColorFrameElement = null;
            }
            SelectedItem = null;
        }
        #endregion

        #region Event

        void Slider_UniFormGrid_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            //this.ReleaseMouseCapture();
        }

        void Slider_UniFormGrid_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            newX = e.GetPosition(null).X;
            newY = e.GetPosition(null).Y;
            Point p = e.GetPosition(null);
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                if (newX > oldX)
                {
                    moveTypeLeftOrRight = MoveTypeLeftOrRight.Right;
                    MoveLR = true;
                    isMove = true;
                }
                else if (newX < oldX)
                {
                    moveTypeLeftOrRight = MoveTypeLeftOrRight.Left;
                    MoveLR = true;
                    isMove = true;
                }

                if (newY < oldY)
                {
                    moveTypeUpOrDown = MoveTypeUpOrDown.Up;
                    MoveUD = true;
                    isMove = true;
                }
                else if (newY > oldY)
                {
                    moveTypeUpOrDown = MoveTypeUpOrDown.Down;
                    MoveUD = true;
                    isMove = true;
                }
                //this.CaptureMouse();
                DvalueX = newX - oldX;
                DvalueY = newY - oldY;
                oldX = newX;
                oldY = newY;
            }
        }

        void Slider_UniFormGrid_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            oldX = e.GetPosition(null).X;
            oldY = e.GetPosition(null).Y;
        }

        #endregion

        #region Enum
        /// 存放枚举类 左右滑动状态
        public enum MoveTypeLeftOrRight
        {
            None,
            Left,
            Right,
            Max
        }

        ///存放枚举类 上下滑动状态
        public enum MoveTypeUpOrDown
        {
            None,
            Up,
            Down,
            Max
        }
        #endregion
    }
}
