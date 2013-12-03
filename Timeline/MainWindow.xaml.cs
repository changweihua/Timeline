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
using Timeline.UC;
using System.Windows.Media.Animation;
using Timeline.ScatterViewItem;
using Timeline.ToolClasses;
using Timeline.Model;
using System.IO;
using System.Collections.ObjectModel;
using Timeline.ScatterViewItem.SubItem;
using System.Threading;


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

            //LoadButtonToCanvas(ProjectHelper.Read(folders[0], new char[] { '|' }));
            LoadBorderToCanvas(ProjectHelper.Read(folders[0], new char[] { '|' }));

        }

        #endregion

        #region 公共方法

        /// <summary>
        /// 遍历，过滤文件
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        IList<ProjectItem> GetProjectItems(string path)
        {
            var query = from p in System.IO.Directory.GetFiles(path)
                        where p.Extension() == "doc" || p.Extension() == "docx" || p.Extension() == "ppt" || p.Extension() == "pptx" || p.Extension() == "xls" || p.Extension() == "xlsx" || p.Extension() == "jpg" || p.Extension() == "jpeg" || p.Extension() == "png" || p.Extension() == "bmp" || p.Extension() == "wmv" || p.Extension() == "mov" || p.Extension() == "3gp" || p.Extension() == "mp4" || p.Extension() == "avi"
                        select new ProjectItem { IsSelected = false, FileName = p, IsEnabled = true } ;
            return query.ToList();
        }

        /// <summary>
        /// 取得文件类型
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        string GetFileType(string path)
        {
            string ext = new FileInfo(path).Extension;
            string type = "more";

            if (ext == ".doc" || ext == ".docx")
            {
                type = "word";
            }
            if (ext == ".ppt" || ext == ".pptx")
            {
                type = "ppt";
            }
            if (ext == ".xls" || ext == ".xlsx")
            {
                type = "excel";
            }
            if (ext == ".jpg" || ext == ".jpeg" || ext == ".png" || ext == ".bmp")
            {
                type = "photo";
            }
            //*.wmv,*mp4,*.3gp,*.avi,*mov
            if (ext == ".wmv" || ext == ".mp4" || ext == ".3gp" || ext == ".mov" || ext ==".avi")
            {
                type = "video";
            }

            return type;
        }

        /// <summary>
        /// 计算元素的 Canvas.Left 值
        /// </summary>
        private double CalauteLeft(UIElement element, double distance, int index)
        {

            double left = 0.0;

#if DEBUG

            var button = element as SurfaceButton;

            if (button != null)
            {
                //获取按钮相对窗体的坐标
                //Point point = button.TransformToAncestor(this).Transform(new Point(0, 0));
                //获取按钮相对 SurfaceScrollViewer 的坐标
                //Point point = button.TranslatePoint(new Point(), ssv);

                //if (point.X < 125)
                //{
                //    left = distance;
                //}
                //else if (this.ssv.ActualWidth - point.X < 125)
                //{
                //    left = distance - (250 - 104);
                //}
                //else
                //{
                //    left = distance - (125 - 52);
                //}

                if (index == 0)
                {
                    left = 0;
                }
                else if (index == _projectCount - 1)
                {
                    left = distance - (250 - 104);
                }
                else
                {
                    left = distance - (125 - 52);
                }

            }

#endif

            return left;

        }

        /// <summary>
        /// 展开
        /// </summary>
        /// <param name="index"></param>
        private void OpenListBox(DependencyObject obj, Point point, int index)
        {

            DoubleAnimation heightAnimation = new DoubleAnimation { From = 0, To = 300, Duration = TimeSpan.FromSeconds(0.5) };
            DoubleAnimation widthAnimation = new DoubleAnimation { From = 0, To = 250, Duration = TimeSpan.FromSeconds(0.5) };
            DoubleAnimation opacityAnimation = new DoubleAnimation { From = 0, To = 1, Duration = TimeSpan.FromSeconds(0.5) };

            Storyboard story = new Storyboard();
            story.Children.Add(heightAnimation);
            story.Children.Add(widthAnimation);
            story.Children.Add(opacityAnimation);

            Storyboard.SetTarget(heightAnimation, obj);
            Storyboard.SetTarget(widthAnimation, obj);
            Storyboard.SetTarget(opacityAnimation, obj);

            Storyboard.SetTargetProperty(heightAnimation, new PropertyPath(ProjectSurfaceListBoxUserControl.HeightProperty));
            Storyboard.SetTargetProperty(widthAnimation, new PropertyPath(ProjectSurfaceListBoxUserControl.WidthProperty));
            Storyboard.SetTargetProperty(opacityAnimation, new PropertyPath(ProjectSurfaceListBoxUserControl.OpacityProperty));

            story.Completed += (sender, e) =>
            {
                Debug.WriteLine("显示完毕");
            };

            story.Begin(this);

        }

        /// <summary>
        /// 关闭
        /// </summary>
        private void CloseListBox(DependencyObject obj, Point point, int index)
        {
            DoubleAnimation heightAnimation = new DoubleAnimation { To = 0.0, Duration = TimeSpan.FromSeconds(0.5) };
            DoubleAnimation widthAnimation = new DoubleAnimation { To = 0.0, Duration = TimeSpan.FromSeconds(0.5) };
            DoubleAnimation opacityAnimation = new DoubleAnimation { To = 0, Duration = TimeSpan.FromSeconds(0.5) };

            Storyboard story = new Storyboard();
            story.Children.Add(heightAnimation);
            story.Children.Add(widthAnimation);
            story.Children.Add(opacityAnimation);

            Storyboard.SetTarget(heightAnimation, obj);
            Storyboard.SetTarget(widthAnimation, obj);
            Storyboard.SetTarget(opacityAnimation, obj);

            Storyboard.SetTargetProperty(heightAnimation, new PropertyPath(ProjectSurfaceListBoxUserControl.HeightProperty));
            Storyboard.SetTargetProperty(widthAnimation, new PropertyPath(ProjectSurfaceListBoxUserControl.WidthProperty));
            Storyboard.SetTargetProperty(opacityAnimation, new PropertyPath(ProjectSurfaceListBoxUserControl.OpacityProperty));

            story.Completed += (sender, e) =>
            {
                Debug.WriteLine("移除");

            };

            story.Begin();
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
            this.canvas.Width = (count - 1) * _averageDistance + 250;
#if DEBUG

            Debug.WriteLine("容器宽度为 {0},Canvas 的宽度 {1}", (this.ssv.ActualWidth - 100), this.canvas.ActualWidth);

#endif

            for (int i = 0; i < projects.Count; i++)
            {
                this.canvas.Children.Add(CreateSurfaceButton(projects[i], i));
            }
        }

        /// <summary>
        /// 创建 SurfaceButton
        /// </summary>
        /// <param name="project"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        private SurfaceButton CreateSurfaceButton(Project project, int index)
        {

            double left = _averageDistance * index;

            SurfaceButton button = new SurfaceButton();
            button.Width = button.Height = 100;
            button.BorderThickness = new Thickness(2.0);
            button.Background = new ImageBrush(new BitmapImage(new Uri("pack://application:,,,/Icons;Component/wp/light/appbar.base.select.png")));
            Image image = new Image();
            image.Source = new BitmapImage(new Uri("pack://application:,,,/Icons;Component/wp/light/appbar.card.3.png"));
            button.Content = image;
            button.SetValue(Canvas.LeftProperty, left);
            button.SetValue(Canvas.TopProperty, this.canvas.ActualHeight / 2 - 52);
            button.Tag = 0;

            button.Click += (sender, e) =>
            {
                //是否展开标志
                int flag = Convert.ToInt32(button.Tag);

                //未展开
                if (flag == 0)
                {
                    //关闭已经打开的
                    for (int i = 0; i < _projectCount; i++)
                    {
                        if (_listBoxList.ContainsKey(i))
                        {
                            this.canvas.Children.Remove(_listBoxList[i]);
                            (this.canvas.Children.OfType<SurfaceButton>()).ElementAt(i).Tag = 0;
                        }
                    }

                    if (!_listBoxList.ContainsKey(index))
                    {
                        ProjectSurfaceListBoxUserControl pluc = new ProjectSurfaceListBoxUserControl();
                        pluc.Background = Brushes.Transparent;

                        var list = GetProjectItems(project.ResourcePath);
                        _projectItems = new ObservableCollection<ProjectItem>(list);
                        pluc.listbox.ItemsSource = _projectItems;

                        pluc.listbox.SelectionChanged += (source, evt) =>
                        {
                            if (pluc.listbox.SelectedIndex != -1)
                            {
                                string path = ((source as ListBox).SelectedItem as ProjectItem).FileName;
                                string type = GetFileType(path);
                                FileListItemModel model = new FileListItemModel()
                                {
                                    FileIcon = @"pack://application:,,,/Icons;Component/wp/light/appbar.page.question.png",
                                    FileType = ConstClass.GetFileSimpleDeclaration(type, new FileInfo(path).Extension),
                                    FileName = new FileInfo(path).Name,
                                    FileFullPath = new FileInfo(path).FullName,
                                    FileSize = (new FileInfo(path).Length / 1024.0) > 1024 ? (new FileInfo(path).Length / 1024.0 / 1024.0).ToString(".00") + " MB" : (new FileInfo(path).Length / 1024.0).ToString(".00") + " KB"
                                };
                                OpenScatterViewItem(model, new Point(0, 0), pluc, pluc.listbox.SelectedIndex);

                                ((source as ListBox).SelectedItem as ProjectItem).IsEnabled = false;

                            }
                        };

                        //left = CalauteLeft(button, left, index);
                        pluc.SetValue(Canvas.LeftProperty, left);
                        pluc.SetValue(Canvas.TopProperty, this.canvas.ActualHeight / 2 + 52 + 25);

                        _listBoxList.Add(index, pluc);
                    }

                    this.canvas.Children.Add(_listBoxList[index]);
                    OpenListBox(_listBoxList[index], new Point(0, 0), index);

                    button.Tag = 1;
                }
                else
                {
                    Debug.WriteLine("关闭");
                    CloseListBox(_listBoxList[index], new Point(0, 0), index);
                    //this.canvas.Children.Remove(_listBoxList[index]);
                    button.Tag = 0;
                }


            };

            return button;
        }

        void SetUIZIndex(UIElement ui)
        {
            ui.SetValue(Panel.ZIndexProperty, ++ConstClass.ZIndex);
        }

        /// <summary>
        /// 打开文件
        /// </summary>
        /// <param name="model">素材模型</param>
        /// <param name="point">位置</param>
        /// <param name="psluc">素材属于时间轴的哪个点</param>
        /// <param name="index">具体时间点</param>
        void OpenScatterViewItem(FileListItemModel model, Point point, ProjectSurfaceListBoxUserControl psluc, int index)
        {
            BaseScatterViewItem item = BaseScatterViewItem.CreateScatterViewItem(model);
            item.Tag = new ScatterViewerTag { ProjectSurfaceListBoxUserControl = psluc, Index = index };
            item.InitialCompleted += delegate
            {

                //scatterItemContainer.SetValue(Panel.ZIndexProperty, 1);
                //m_InkFileListControl.SetValue(Panel.ZIndexProperty, 0);
                SetUIZIndex(sv);

                sv.Items.Add(item);
                item.DoubleTapped += new RoutedEventHandler(item_DoubleTapped);
                item.ObjectRemoved += new Action<BaseScatterViewItem>(item_ObjectRemoved);
                item.Loaded += delegate
                {
                    //maskGrid.Visibility = System.Windows.Visibility.Collapsed;
                    double dot;
                    if (item is OfficeScatterViewItem)
                    {
                        if (double.IsNaN(item.Width) || double.IsNaN(item.Height))
                        {
                            dot = item.Width / item.Height;
                        }
                        else
                        {
                            dot = item.ActualWidth / item.ActualHeight;
                        }

                    }
                    else
                    {
                        dot = item.ShadowVector.X / item.ShadowVector.Y;
                    }
                    Point newCenter = new Point(ConstClass.APPWIDTH / 2, (ConstClass.APPHEIGHT - (88.0 - TopHeight) + (88.0 - BottomHeight)) / 2);
                    item.Opacity = 1;
                    double newHeight = 0, newWidth = 0;
                    if (ConstClass.APPHEIGHT < ConstClass.APPWIDTH)
                    {
                        newHeight = ConstClass.APPHEIGHT - BottomHeight - TopHeight;
                        newWidth = newHeight * dot;
                    }
                    else
                    {
                        newWidth = ConstClass.APPWIDTH;
                        newHeight = newWidth / dot;
                    }
                    DoubleAnimation animationWidth = new DoubleAnimation() { From = 0, To = newWidth, Duration = TimeSpan.FromMilliseconds(250) };
                    DoubleAnimation animationHeight = new DoubleAnimation() { From = 0, To = newHeight, Duration = TimeSpan.FromMilliseconds(250) };
                    PointAnimation animationCenter = new PointAnimation() { From = point, To = newCenter, Duration = TimeSpan.FromMilliseconds(250) };
                    DoubleAnimation animationOrientation = new DoubleAnimation() { From = 0, To = 0, Duration = TimeSpan.FromMilliseconds(250) };

                    Storyboard.SetTarget(animationWidth, item);
                    Storyboard.SetTarget(animationHeight, item);
                    Storyboard.SetTarget(animationCenter, item);
                    Storyboard.SetTarget(animationOrientation, item);
                    Storyboard.SetTargetProperty(animationWidth, new PropertyPath(BaseScatterViewItem.WidthProperty));
                    Storyboard.SetTargetProperty(animationHeight, new PropertyPath(BaseScatterViewItem.HeightProperty));
                    Storyboard.SetTargetProperty(animationCenter, new PropertyPath(BaseScatterViewItem.CenterProperty));
                    Storyboard.SetTargetProperty(animationOrientation, new PropertyPath(BaseScatterViewItem.OrientationProperty));

                    Storyboard sb = new Storyboard();
                    sb.Children.Add(animationWidth);
                    sb.Children.Add(animationHeight);
                    sb.Children.Add(animationCenter);
                    sb.Children.Add(animationOrientation);

                    sb.Completed += delegate
                    {
                        item.Width = animationWidth.To.Value;
                        item.Height = animationHeight.To.Value;
                        item.Center = animationCenter.To.Value;
                        item.Orientation = animationOrientation.To.Value;
                        item.ApplyAnimationClock(BaseScatterViewItem.WidthProperty, null);
                        item.ApplyAnimationClock(BaseScatterViewItem.HeightProperty, null);
                        item.ApplyAnimationClock(BaseScatterViewItem.CenterProperty, null);
                        item.ApplyAnimationClock(BaseScatterViewItem.OrientationProperty, null);

                        (psluc.listbox.SelectedItem as ProjectItem).IsSelected = false;

                        item.CanRotate = true;
                    };
                    sb.Begin();
                };
            };
        }

        void item_ObjectRemoved(BaseScatterViewItem obj)
        {
            sv.Items.Remove(obj);
        }

        void item_DoubleTapped(object sender, RoutedEventArgs e)
        {
            BaseScatterViewItem item = sender as BaseScatterViewItem;
            ScatterViewerTag tag = item.Tag as ScatterViewerTag;
            if (tag != null)
            {
                var pluc = tag.ProjectSurfaceListBoxUserControl;
                ((pluc.listbox as SurfaceListBox).Items[tag.Index] as ProjectItem).IsEnabled = true;
            }
            double dot = item.Width / item.Height;
            Point newCenter = new Point(ConstClass.APPWIDTH / 2, (ConstClass.APPHEIGHT - (88.0 - TopHeight) + (88.0 - BottomHeight)) / 2);
            item.Opacity = 1;

            DoubleAnimation animationWidth = new DoubleAnimation() { From = item.ActualWidth, To = 0, Duration = TimeSpan.FromMilliseconds(250) };
            DoubleAnimation animationHeight = new DoubleAnimation() { From = item.ActualHeight, To = 0, Duration = TimeSpan.FromMilliseconds(250) };
            PointAnimation animationCenter = new PointAnimation() { From = item.Center, To = newCenter, Duration = TimeSpan.FromMilliseconds(250) };
            DoubleAnimation animationOrientation = new DoubleAnimation() { From = item.Orientation, To = 0, Duration = TimeSpan.FromMilliseconds(250) };

            Storyboard.SetTarget(animationWidth, item);
            Storyboard.SetTarget(animationHeight, item);
            Storyboard.SetTarget(animationCenter, item);
            Storyboard.SetTarget(animationOrientation, item);
            Storyboard.SetTargetProperty(animationWidth, new PropertyPath(BaseScatterViewItem.WidthProperty));
            Storyboard.SetTargetProperty(animationHeight, new PropertyPath(BaseScatterViewItem.HeightProperty));
            Storyboard.SetTargetProperty(animationCenter, new PropertyPath(BaseScatterViewItem.CenterProperty));
            Storyboard.SetTargetProperty(animationOrientation, new PropertyPath(BaseScatterViewItem.OrientationProperty));

            Storyboard sb = new Storyboard();
            sb.Children.Add(animationWidth);
            sb.Children.Add(animationHeight);
            sb.Children.Add(animationCenter);
            sb.Children.Add(animationOrientation);

            sb.Completed += delegate
            {
                item.Width = animationWidth.To.Value;
                item.Height = animationHeight.To.Value;
                item.Center = animationCenter.To.Value;
                item.Orientation = animationOrientation.To.Value;
                item.ApplyAnimationClock(BaseScatterViewItem.WidthProperty, null);
                item.ApplyAnimationClock(BaseScatterViewItem.HeightProperty, null);
                item.ApplyAnimationClock(BaseScatterViewItem.CenterProperty, null);
                item.ApplyAnimationClock(BaseScatterViewItem.OrientationProperty, null);

                sv.Items.Remove(item);

            };
            sb.Begin();

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
                this.canvas.Children.Add(CreateBorder(projects[i], i));
            }
        }

        private Border CreateBorder(Project project, int index)
        {
            double left = _averageDistance * index;

            Border border = new Border();
            border.Background = Brushes.Transparent;
            border.Style = this.FindResource("BorderStyle") as Style;

            Image image = new Image();
            image.Source = new BitmapImage(new Uri("pack://application:,,,/Icons;Component/wp/light/appbar.card.3.png"));
            border.Child = image;
            border.SetValue(Canvas.LeftProperty, left);
            border.SetValue(Canvas.TopProperty, this.canvas.ActualHeight / 2 - 52);
            border.Tag = 0;

            border.PreviewTouchDown += (sender, e) =>
            {
               
                //是否展开标志
                int flag = Convert.ToInt32(border.Tag);

                //未展开
                if (flag == 0)
                {
                    //关闭已经打开的
                    for (int i = 0; i < _projectCount; i++)
                    {
                        if (_listBoxList.ContainsKey(i))
                        {
                            this.canvas.Children.Remove(_listBoxList[i]);
                            (this.canvas.Children.OfType<Border>()).ElementAt(i).Tag = 0;
                        }
                    }

                    if (!_listBoxList.ContainsKey(index))
                    {
                        ProjectSurfaceListBoxUserControl pluc = new ProjectSurfaceListBoxUserControl();
                        pluc.Background = Brushes.Transparent;

                        var list = GetProjectItems(project.ResourcePath);
                        _projectItems = new ObservableCollection<ProjectItem>(list);
                        pluc.listbox.ItemsSource = _projectItems;

                        pluc.listbox.SelectionChanged += (source, evt) =>
                        {
                            if (pluc.listbox.SelectedIndex != -1)
                            {
                                string path = ((source as ListBox).SelectedItem as ProjectItem).FileName;
                                string type = GetFileType(path);
                                FileListItemModel model = new FileListItemModel()
                                {
                                    FileIcon = @"pack://application:,,,/Icons;Component/wp/light/appbar.page.question.png",
                                    FileType = ConstClass.GetFileSimpleDeclaration(type, new FileInfo(path).Extension),
                                    FileName = new FileInfo(path).Name,
                                    FileFullPath = new FileInfo(path).FullName,
                                    FileSize = (new FileInfo(path).Length / 1024.0) > 1024 ? (new FileInfo(path).Length / 1024.0 / 1024.0).ToString(".00") + " MB" : (new FileInfo(path).Length / 1024.0).ToString(".00") + " KB"
                                };
                                OpenScatterViewItem(model, new Point(0, 0), pluc, pluc.listbox.SelectedIndex);

                                ((source as ListBox).SelectedItem as ProjectItem).IsEnabled = false;

                            }
                        };

                        //left = CalauteLeft(button, left, index);
                        pluc.SetValue(Canvas.LeftProperty, left);
                        pluc.SetValue(Canvas.TopProperty, this.canvas.ActualHeight / 2 + 52 + 25);

                        _listBoxList.Add(index, pluc);
                    }

                    this.canvas.Children.Add(_listBoxList[index]);
                    OpenListBox(_listBoxList[index], new Point(0, 0), index);

                    border.Tag = 1;
                }
                else
                {
                    Debug.WriteLine("关闭");
                    CloseListBox(_listBoxList[index], new Point(0, 0), index);
                    //this.canvas.Children.Remove(_listBoxList[index]);
                    border.Tag = 0;
                }


            };

            image.MouseLeftButtonDown += (sender, e) => 
            {
                //不响应非单击事件
                if (e.ClickCount != 1)
                {
                    return;
                }

                //是否展开标志
                int flag = Convert.ToInt32(border.Tag);

                //未展开
                if (flag == 0)
                {
                    //关闭已经打开的
                    for (int i = 0; i < _projectCount; i++)
                    {
                        if (_listBoxList.ContainsKey(i))
                        {
                            this.canvas.Children.Remove(_listBoxList[i]);
                            (this.canvas.Children.OfType<Border>()).ElementAt(i).Tag = 0;
                        }
                    }

                    if (!_listBoxList.ContainsKey(index))
                    {
                        ProjectSurfaceListBoxUserControl pluc = new ProjectSurfaceListBoxUserControl();
                        pluc.Background = Brushes.Transparent;

                        var list = GetProjectItems(project.ResourcePath);
                        _projectItems = new ObservableCollection<ProjectItem>(list);
                        pluc.listbox.ItemsSource = _projectItems;

                        pluc.listbox.SelectionChanged += (source, evt) =>
                        {
                            if (pluc.listbox.SelectedIndex != -1)
                            {
                                string path = ((source as ListBox).SelectedItem as ProjectItem).FileName;
                                string type = GetFileType(path);
                                FileListItemModel model = new FileListItemModel()
                                {
                                    FileIcon = @"pack://application:,,,/Icons;Component/wp/light/appbar.page.question.png",
                                    FileType = ConstClass.GetFileSimpleDeclaration(type, new FileInfo(path).Extension),
                                    FileName = new FileInfo(path).Name,
                                    FileFullPath = new FileInfo(path).FullName,
                                    FileSize = (new FileInfo(path).Length / 1024.0) > 1024 ? (new FileInfo(path).Length / 1024.0 / 1024.0).ToString(".00") + " MB" : (new FileInfo(path).Length / 1024.0).ToString(".00") + " KB"
                                };
                                OpenScatterViewItem(model, new Point(0, 0), pluc, pluc.listbox.SelectedIndex);

                                ((source as ListBox).SelectedItem as ProjectItem).IsEnabled = false;

                            }
                        };

                        //left = CalauteLeft(button, left, index);
                        pluc.SetValue(Canvas.LeftProperty, left);
                        pluc.SetValue(Canvas.TopProperty, this.canvas.ActualHeight / 2 + 52 + 25);

                        _listBoxList.Add(index, pluc);
                    }

                    this.canvas.Children.Add(_listBoxList[index]);
                    OpenListBox(_listBoxList[index], new Point(0, 0), index);

                    border.Tag = 1;
                }
                else
                {
                    Debug.WriteLine("关闭");
                    CloseListBox(_listBoxList[index], new Point(0, 0), index);
                    //this.canvas.Children.Remove(_listBoxList[index]);
                    border.Tag = 0;
                }
            };

            return border;
        }

        #endregion

        #region 公共事件



        #endregion

        #region CLR 属性

        /// <summary>
        /// 顶级目录
        /// </summary>
        const string _resourceFolder = @"D:\Meeting\time";

        /// <summary>
        /// 平均间隔
        /// </summary>
        public double _averageDistance = 0.0d;

        /// <summary>
        /// 资源个数
        /// </summary>
        int _projectCount = 0;

        /// <summary>
        /// 展开菜单列表
        /// </summary>
        Dictionary<int, ProjectSurfaceListBoxUserControl> _listBoxList = new Dictionary<int, ProjectSurfaceListBoxUserControl>();

        /// <summary>
        /// 菜单选项
        /// </summary>
        ObservableCollection<ProjectItem> _projectItems = new ObservableCollection<ProjectItem>();

        /// <summary>
        /// 菜单控件
        /// </summary>
        ProjectSurfaceListBoxUserControl _listBox = new ProjectSurfaceListBoxUserControl();

        #endregion

        #region 依赖属性

        public double TopHeight
        {
            get { return (double)GetValue(TopHeightProperty); }
            set { SetValue(TopHeightProperty, value); }
        }

        public static readonly DependencyProperty TopHeightProperty =
            DependencyProperty.Register("TopHeight", typeof(double), typeof(MainWindow), new UIPropertyMetadata(0.0));

        public double BottomHeight
        {
            get { return (double)GetValue(BottomHeightProperty); }
            set { SetValue(BottomHeightProperty, value); }
        }

        public static readonly DependencyProperty BottomHeightProperty =
            DependencyProperty.Register("BottomHeight", typeof(double), typeof(MainWindow), new UIPropertyMetadata(88.0));

        #endregion

    }
}
