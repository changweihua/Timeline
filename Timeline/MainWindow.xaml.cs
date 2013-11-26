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
            averageDistance = this.ActualWidth / ((projects.Count >= 8 ? 9 : (projects.Count + 1)));

            for (int i = 0; i < projects.Count; i++)
            {
                this.canvas.Children.Add(CreateBorder(projects[i], i+1));
            }
        }

        private Border CreateBorder(Project project, int index)
        {
            Border border = new Border();
            border.Style = this.FindResource("BorderStyle") as Style;
            double left = averageDistance * index - 50;
#if DEBUG

            Debug.WriteLine("Canvas.LeftProperty = {0}", left);

#endif

            border.SetValue(Canvas.LeftProperty, left);

            Image image = new Image();
            image.Source = new BitmapImage(new Uri("pack://application:,,,/Icons;Component/wp/light/appbar.card.1.png"));

            border.Child = image;

            return border;
        }

        #endregion

        #region 公共事件



        #endregion

        #region CLR 属性

        const string ResourceFolder = @"D:\Meeting\time";

        public double averageDistance = 0.0d;

        #endregion
       

    }
}
