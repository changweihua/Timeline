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
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;

namespace Timeline
{
    /// <summary>
    /// TimelineUserControl.xaml 的交互逻辑
    /// </summary>
    public partial class TimelineUserControl : UserControl
    {

        public TimelineUserControl()
        {
            InitializeComponent();

            this.Loaded += (sender, e) => {
                InitResource();
            };
        }

        #region 公共方法

        /// <summary>
        /// 初始化
        /// </summary>
        private void InitResource()
        {
            if (string.IsNullOrEmpty(ResourceFolder) || !System.IO.Directory.Exists(ResourceFolder))
            {
                return;
            }

            List<string> folders = System.IO.Directory.GetFiles(ResourceFolder).ToList();

#if DEBUG

            Debug.WriteLine("子文件夹数量 {0}",folders.Count);

#endif

            if (folders.Count == 1)
            {
                Projects = new ObservableCollection<Project>();
                
                using (TextReader reader = new StreamReader(folders[0]))
                {
                    string line = string.Empty;
                    while ((line = reader.ReadLine()) != null)
                    {
                        string[] arr = line.Split(new char[] { '|' }, 2);
                        var project = new Project { Title = arr[0], ResourcePath = @arr[1] };
                        Projects.Add(project);

                    }
                }

            }

            averageDistance = this.ActualWidth / ((Projects.Count >= 8 ? 9 : (Projects.Count + 1)));

            for (int i = 0; i < Projects.Count; i++)
            {
                this.canvas.Children.Add(CreateBorder(Projects[i], i + 1));
            }

#if DEBUG

            Debug.WriteLine("真实宽度 {2}, {0} 等分, 值为 {1}", (Projects.Count >= 8 ? 9 : (Projects.Count + 1)), averageDistance, this.ActualWidth);
            Debug.WriteLine("读取行数 {0}", Projects.Count);

#endif

        }

        void LoadBorderToCanvas()
        { 
            
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

        public string timelineFolder = @"D:\Meeting\time";

        public double averageDistance = 0.0d;

        #endregion

        #region 依赖属性

        /// <summary>
        /// 时间轴元素集合
        /// </summary>
        public ObservableCollection<Project> Projects
        {
            get { return (ObservableCollection<Project>)GetValue(ProjectsProperty); }
            set { SetValue(ProjectsProperty, value); }
        }

        public static readonly DependencyProperty ProjectsProperty =
            DependencyProperty.Register("Projects", typeof(ObservableCollection<Project>), typeof(TimelineUserControl), new UIPropertyMetadata(null));

        /// <summary>
        /// 文件夹路径
        /// </summary>
        public string ResourceFolder
        {
            get { return (string)GetValue(ResourceFolderProperty); }
            set { SetValue(ResourceFolderProperty, value); }
        }

        public static readonly DependencyProperty ResourceFolderProperty =
            DependencyProperty.Register("ResourceFolder", typeof(string), typeof(TimelineUserControl), new UIPropertyMetadata(""));

        #endregion


    }
}
