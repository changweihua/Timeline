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
            if (string.IsNullOrEmpty(ResourceFolder) || !System.IO.Directory.Exists(ResourceFolder))
            {
                return;
            }

            List<string> folders = System.IO.Directory.GetDirectories(ResourceFolder).ToList();

            LoadBorderToCanvas(ProjectHelper.Read(folders[0]));

        }
        
        #endregion

        #region 公共方法

        /// <summary>
        /// 计算元素的 Canvas.Left 值
        /// </summary>
        private void CalauteLeft(int index)
        {

#if DEBUG



#endif

        }

        void LoadBorderToCanvas(IList<Project> projects)
        {
            int count = projects.Count;
            ProjectCount = count;
            //减去最后一个元素的宽度
            averageDistance = (this.ssv.ActualWidth - 104) / ((projects.Count >= 8 ? 7 : (projects.Count - 1)));
            //计算canvas的宽度
            this.canvas.Width = (count - 1) * averageDistance + 104;
#if DEBUG

            Debug.WriteLine("容器宽度为 {0},Canvas 的宽度 {1}", (this.ssv.ActualWidth - 100), this.canvas.ActualWidth);

#endif

            for (int i = 0; i < projects.Count; i++)
            {
                this.canvas.Children.Add(CreateBorder(projects[i], i));
            }
        }

        private Border CreateBorder(Project project, int index)
        {
            Border border = new Border();
            border.Style = this.FindResource("BorderStyle") as Style;
            
            double left = averageDistance * index;

#if DEBUG

            Debug.WriteLine("Canvas.LeftProperty = {0}, Border.Width = {1}", left, border.ActualWidth);

#endif

            border.SetValue(Canvas.LeftProperty, left);

            Image image = new Image();
            image.Source = new BitmapImage(new Uri("pack://application:,,,/Icons;Component/wp/light/appbar.card.1.png"));

            border.Child = image;

            border.TouchDown += (sender, e) => 
            {

                var point = e.GetTouchPoint(border);
#if DEBUG

                Debug.WriteLine("触摸位置 ({0}, {1})", point.Position.X, point.Position.Y);

#endif
            };

            return border;
        }

        #endregion

        #region 公共事件



        #endregion

        #region CLR 属性

        const string ResourceFolder = @"D:\Meeting\time";

        public double averageDistance = 0.0d;

        int ProjectCount = 0;

        #endregion
       

    }
}
