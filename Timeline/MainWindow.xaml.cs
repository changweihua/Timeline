using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Diagnostics;
using Microsoft.Surface.Presentation.Controls;

namespace Timeline
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {

        #region 窗体事件

        public MainWindow()
        {
            InitializeComponent();

            this.Loaded += new RoutedEventHandler(MainWindow_Loaded);
        }

        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(_resourceFolder) || !System.IO.Directory.Exists(_resourceFolder))
            {
                return;
            }

            List<string> folders = System.IO.Directory.GetDirectories(_resourceFolder).ToList();

            LoadButtonToCanvas(ProjectHelper.Read(folders[0]));

        }
        
        #endregion

        #region 公共方法

        /// <summary>
        /// 计算元素的 Canvas.Left 值
        /// </summary>
        private double CalauteLeft(UIElement element, double target)
        {

            double left = 0.0;

#if DEBUG

            var button = element as SurfaceButton;

            if (button != null)
            { 
                //获取按钮相对窗体的坐标
                //Point point = button.TransformToAncestor(this).Transform(new Point(0, 0));
                //获取按钮相对 SurfaceScrollViewer 的坐标
                Point point = button.TranslatePoint(new Point(), ssv);

                if (point.X < target)
                {
                    left = point.X;
                }

            }

#endif

            return left;

        }

        #endregion

        

        #region 将时间点作为 SurfaceButton 展示

        void LoadButtonToCanvas(IList<Project> projects)
        {
            int count = projects.Count;
            _projectCount = count;
            //减去最后一个元素的宽度
            _averageDistance = (this.ssv.ActualWidth - 104) / ((projects.Count >= 8 ? 7 : (projects.Count - 1)));
            //计算canvas的宽度
            this.canvas.Width = (count - 1) * _averageDistance + 104;
#if DEBUG

            Debug.WriteLine("容器宽度为 {0},Canvas 的宽度 {1}", (this.ssv.ActualWidth - 100), this.canvas.ActualWidth);

#endif

            for (int i = 0; i < projects.Count; i++)
            {
                this.canvas.Children.Add(CreateSurfaceButton(projects[i], i));
            }
        }

        private SurfaceButton CreateSurfaceButton(Project project, int index)
        {
            double left = _averageDistance * index;

            SurfaceButton button = new SurfaceButton();
            button.Width = button.Height = 100;
            button.BorderThickness = new Thickness(2.0);
            Image image = new Image();
            image.Source = new BitmapImage(new Uri("pack://application:,,,/Icons;Component/wp/light/appbar.card.1.png"));
            button.Content = image;
            button.SetValue(Canvas.LeftProperty, left);
            button.SetValue(Canvas.TopProperty, this.canvas.ActualHeight / 2 - 52);

            button.Click += (sender, e) =>
            {
                
                //MessageBox.Show("Touched");
                //var point = e.GetTouchPoint(button);
#if DEBUG

                //Debug.WriteLine("触摸位置 ({0}, {1})", point.Position.X, point.Position.Y);

#endif
                
                ProjectListUserControl pluc = new ProjectListUserControl();
                pluc.Background = Brushes.Transparent;
                pluc.listbox.ItemsSource = System.IO.Directory.GetFiles(project.ResourcePath);

                pluc.listbox.SelectionChanged += (source, evt) =>
                {
#if DEBUG

                    string path = (source as ListBox).SelectedItem.ToString();

                    Debug.WriteLine("当前点击资源路径为 {0}", path);

                    sv.Items.Add(new Image { Width = 350, Height = 550, Source = new BitmapImage(new Uri(path, UriKind.Absolute)) });

#endif
                };

                pluc.SetValue(Canvas.LeftProperty, left);
                pluc.SetValue(Canvas.TopProperty, this.canvas.ActualHeight / 2 + 52 + 25);
                this.canvas.Children.Add(pluc);

            };

            return button;
        }

        #endregion
        

        #region 将时间点作为 Border 展示

        void LoadBorderToCanvas(IList<Project> projects)
        {
            int count = projects.Count;
            _projectCount = count;
            //减去最后一个元素的宽度
            _averageDistance = (this.ssv.ActualWidth - 104) / ((projects.Count >= 8 ? 7 : (projects.Count - 1)));
            //计算canvas的宽度
            this.canvas.Width = (count - 1) * _averageDistance + 104;
#if DEBUG

            Debug.WriteLine("容器宽度为 {0},Canvas 的宽度 {1}", (this.ssv.ActualWidth - 100), this.canvas.ActualWidth);

#endif

            for (int i = 0; i < projects.Count; i++)
            {
                this.canvas.Children.Add(CreateSurfaceButton(projects[i], i));
            }
        }

        private Border CreateBorder(Project project, int index)
        {
            Border border = new Border();
            border.Style = this.FindResource("BorderStyle") as Style;

            double left = _averageDistance * index;

#if DEBUG

            Debug.WriteLine("Canvas.LeftProperty = {0}, Border.Width = {1}", left, border.ActualWidth);

#endif

            border.SetValue(Canvas.LeftProperty, left);

            Image image = new Image();
            image.Source = new BitmapImage(new Uri("pack://application:,,,/Icons;Component/wp/light/appbar.card.1.png"));

            border.Child = image;

            border.TouchUp += (sender, e) =>
            {

                var point = e.GetTouchPoint(border);
#if DEBUG

                Debug.WriteLine("触摸位置 ({0}, {1})", point.Position.X, point.Position.Y);

#endif
                ProjectListUserControl pluc = new ProjectListUserControl();
                pluc.Background = Brushes.Transparent;
                pluc.listbox.ItemsSource = System.IO.Directory.GetFiles(project.ResourcePath);

                pluc.listbox.SelectionChanged += (source, evt) =>
                {
#if DEBUG

                    string path = (source as ListBox).SelectedItem.ToString();

                    Debug.WriteLine("当前点击资源路径为 {0}", path);

                    sv.Items.Add(new Image { Width = 350, Height = 550, Source = new BitmapImage(new Uri(path, UriKind.Absolute)) });

#endif
                };

                this.canvas.Children.Add(pluc);

            };

            return border;
        }

        #endregion


        #region 公共事件



        #endregion

        #region CLR 属性

        const string _resourceFolder = @"D:\Meeting\time";

        public double _averageDistance = 0.0d;

        int _projectCount = 0;

        #endregion
       

    }
}
