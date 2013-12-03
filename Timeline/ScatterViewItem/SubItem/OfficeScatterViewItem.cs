using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.IO;
using System.Windows.Xps.Packaging;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Documents;
using Timeline.ScatterViewItem;
using Timeline.ToolClasses;
using Timeline.ScatterViewItem.SubItem;

namespace Timeline.ScatterViewItem.SubItem
{
    public class OfficeScatterViewItem : BaseScatterViewItem
    {
        // Using a DependencyProperty as the backing store for Document.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DocumentProperty =
            DependencyProperty.Register("Document", typeof(System.Windows.Documents.IDocumentPaginatorSource), typeof(OfficeScatterViewItem), new UIPropertyMetadata(null));

        public System.Windows.Documents.IDocumentPaginatorSource Document
        {
            get { return (System.Windows.Documents.IDocumentPaginatorSource)GetValue(DocumentProperty); }
            set { SetValue(DocumentProperty, value); }
        }


        public BitmapSource Thumbnail
        {
            get { return (BitmapSource)GetValue(ThumbnailProperty); }
            set { SetValue(ThumbnailProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Thumbnail.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ThumbnailProperty =
            DependencyProperty.Register("Thumbnail", typeof(BitmapSource), typeof(OfficeScatterViewItem), new UIPropertyMetadata(null));


        public bool IsUsingOfficeThumbnail { get; set; }
        public bool IsPptFile { get; set; }
        public bool IsInsertTask { get; set; }

        private XpsDocument m_XpsDocument = null;
        private DocumentViewer m_DocumentViewer = null;
        private int m_CurrentPage = 1;
        private static IShellFolder pDestop = null;
        private static IShellFolder pSub = null;
        private static IExtractImage pExtractImage = null;
        private static IntPtr pDesktopPtr = IntPtr.Zero;
        private static IntPtr pIdList = IntPtr.Zero;
        private static IntPtr hBmp = IntPtr.Zero;
        private TaskInfo m_TaskInfo = null;

        public static event Action Error;

        static OfficeScatterViewItem()
        {
            //DefaultStyleKeyProperty.OverrideMetadata(typeof(OfficeScatterViewItem), new FrameworkPropertyMetadata(typeof(OfficeScatterViewItem)));
            //DocumentViewer.DefaultStyleKeyProperty.OverrideMetadata(typeof(DocumentViewer), new FrameworkPropertyMetadata(typeof(DocumentViewer)));
            pDestop = API.GetDesktopFolder(out pDesktopPtr);
        }
        public OfficeScatterViewItem()
        {
            InitialCompleted += new RoutedEventHandler(OfficeScatterViewItem_Initialized);
            Tapped += new TappedRoutedEventHandler(OfficeScatterViewItem_Tapped);            
        }


        void OfficeScatterViewItem_Initialized(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!IsUsingOfficeThumbnail)
                {
                    Document = m_XpsDocument.GetFixedDocumentSequence();

                    Height = Document.DocumentPaginator.GetPage(0).Size.Height;
                    Width = Document.DocumentPaginator.GetPage(0).Size.Width;

                    m_DocumentViewer = new DocumentViewer();
                    m_DocumentViewer.Width = Width;
                    m_DocumentViewer.Height = Height;
                    m_DocumentViewer.Document = Document;
                    m_DocumentViewer.ShowPageBorders = false;
                    m_DocumentViewer.IsEnabled = false;
                    ScrollViewer content = m_DocumentViewer.Template.FindName("PART_ContentHost", m_DocumentViewer) as ScrollViewer;
                    content.HorizontalScrollBarVisibility = ScrollBarVisibility.Hidden;
                    content.VerticalScrollBarVisibility = ScrollBarVisibility.Hidden;

                    this.AddChild(new Viewbox() { Child = m_DocumentViewer });
                }
                else
                {
                    this.AddChild(new Image() { Source = Thumbnail });
                    Height = Thumbnail.Height;
                    Width = Thumbnail.Width;
                }
                base.BaseScatterViewItem_InitialComplated(sender, e);
            }
            catch
            { 
                
            }
        }

        protected override void InitailScatterViewItem()
        {

            #region 生成XPS文件并呈现
            string tmpXpsDirectory = ConstClass.m_TempXPSPath;
            if (!Directory.Exists(tmpXpsDirectory))
            {
                Directory.CreateDirectory(tmpXpsDirectory);
            }
            string parentDirectory = new DirectoryInfo(SourceModel.FileFullPath.Replace(SourceModel.FileFullPath.FileName(), string.Empty)).Name; ///父文件夹名称
            string modifyDataTime = new FileInfo(SourceModel.FileFullPath).LastWriteTime.ToFileTime().ToString(); ///修改时间
            string xpsFilePath = string.Empty;

            if (!parentDirectory.Contains(":"))
            {
                xpsFilePath = tmpXpsDirectory + "\\" + SourceModel.FileFullPath.FileName(false) + "_" + parentDirectory + "_" + modifyDataTime + "_" + SourceModel.FileFullPath.Extension() + ".xps";
            }
            else
            {
                xpsFilePath = tmpXpsDirectory + "\\" + SourceModel.FileFullPath.FileName(false) + "_" + modifyDataTime + "_" + SourceModel.FileFullPath.Extension() + ".xps";
            }
            if (File.Exists(xpsFilePath))
            {
                if (!IsUsingOfficeThumbnail)
                {
                    m_TaskInfo = new TaskInfo(new Task((Action)delegate
                    {
                        this.Dispatcher.BeginInvoke((Action)delegate
                        {
                            m_XpsDocument = new XpsDocument(xpsFilePath, FileAccess.Read);

                            RoutedEventArgs args = new RoutedEventArgs(InitialCompletedRouteEvent);
                            RaiseEvent(args);
                        }, null);
                    }));
                    OfficeOpenTaskPool.Add(m_TaskInfo);
                }
                else
                {
                    string entension = Path.GetExtension(SourceModel.FileFullPath);
                    if ((entension.Equals(".ppt") || entension.Equals(".pptx")) && IsUsingOfficeThumbnail) ///ppt直接使用预览 其他OFFICE生成XPS后预览
                    {
                        UsingThumbnail(SourceModel.FileFullPath);
                        return;
                    }
                    UsingThumbnail(xpsFilePath);
                }
            }
            else
            {
                string entension = Path.GetExtension(SourceModel.FileFullPath);
                if ((entension.Equals(".ppt") || entension.Equals(".pptx")) && IsUsingOfficeThumbnail) ///ppt直接使用预览 其他OFFICE生成XPS后预览
                {
                    UsingThumbnail(SourceModel.FileFullPath);
                }
                else
                {
                    m_TaskInfo = new TaskInfo(new Task((Action)delegate
                    {
                        switch (SourceModel.FileFullPath.Extension().ToLower())
                        {
                            case "xps":
                                System.IO.File.Copy(SourceModel.FileFullPath, xpsFilePath, true);
                                xpsFilePath = SourceModel.FileFullPath;
                                break;
                            case "doc":
                            case "docx":
                                OfficeDocumentToXps.ConvertWordDocToXPSDoc(SourceModel.FileFullPath, xpsFilePath);
                                break;
                            case "xls":
                            case "xlsx":
                                OfficeDocumentToXps.ConvertExcelToXPSExcel(SourceModel.FileFullPath, xpsFilePath);
                                break;
                            case "ppt":
                            case "pptx":
                                OfficeDocumentToXps.ConvertPptToXPSPpt(SourceModel.FileFullPath, xpsFilePath);
                                break;
                        }
                        this.Dispatcher.BeginInvoke((Action)delegate
                        {
                            try
                            {
                                if (File.Exists(xpsFilePath))
                                {
                                    if (!IsUsingOfficeThumbnail) ///生成完XPS 开始选择使用哪种方式呈现
                                    {
                                        m_XpsDocument = new XpsDocument(xpsFilePath, FileAccess.Read);
                                        RoutedEventArgs args = new RoutedEventArgs(InitialCompletedRouteEvent);
                                        RaiseEvent(args);
                                    }
                                    else { UsingThumbnail(xpsFilePath); }
                                }
                            }
                            catch { }
                        }, null);
                    }));
                    if (!IsInsertTask)
                    {
                        OfficeTaskPool.Add(m_TaskInfo);
                    }
                    else
                    {
                        OfficeTaskPool.Insert(m_TaskInfo);
                    }
                }
            }
            #endregion

        }
        public override void OnApplyTemplate()
        {
            //m_DocumentViewer = (this.ContentTemplate.LoadContent() as Viewbox).Child as DocumentViewer;
            //m_DocumentViewer.Width = Width;
            //m_DocumentViewer.Height = Height;
            base.OnApplyTemplate();
        }
        void OfficeScatterViewItem_Tapped(object sender, TappedEventArgs e)
        {
            GetPageChange(e.Point);
        }
        void GetPageChange(Point point)
        {
            // 在item总宽度的三分之一左方点击显示上一页
            if (point.X < ActualWidth / 3)
            {
                if (m_CurrentPage > 1)
                {
                    m_CurrentPage--;
                    m_DocumentViewer.GoToPage(m_CurrentPage);
                }
            }
            else if (point.X > 2 * ActualWidth / 3) // 在item宽度的三分之二右方点击显示下一页
            {
                if (m_CurrentPage < m_DocumentViewer.PageCount)
                {
                    m_CurrentPage++;
                    m_DocumentViewer.GoToPage(m_CurrentPage);
                }
            }
        }
        void UsingThumbnail(string fileName)
        {
            uint pdwAttributes = 0;
            uint pchEaten;
            int prgf = 0;

            m_TaskInfo = new TaskInfo(new Task((Action)delegate
            {
                this.Dispatcher.Invoke((Action)delegate
                {
                    switch (SourceModel.FileFullPath.Extension().ToLower())
                    {
                        case "xps":
                            Thumbnail = Timeline.ToolClasses.WPFUtil.StringToBitmapImage(@"pack://application:,,,/Icons;Component/wp/light/appbar.page.question.png");
                            break;
                        case "doc":
                        case "docx":
                            Thumbnail = Timeline.ToolClasses.WPFUtil.StringToBitmapImage(@"pack://application:,,,/Icons;Component/wp/light/appbar.office.word.png");
                            break;
                        case "xls":
                        case "xlsx":
                            Thumbnail = Timeline.ToolClasses.WPFUtil.StringToBitmapImage(@"pack://application:,,,/Icons;component/Resource/appbar.office.excel.png");
                            break;
                        case "ppt":
                        case "pptx":
                            Thumbnail = Timeline.ToolClasses.WPFUtil.StringToBitmapImage(@"pack://application:,,,/Icons;component/Resource/appbar.office.powerpoint.png");
                            break;
                    }
                }, null);

                try
                {
                    pDestop.ParseDisplayName(IntPtr.Zero, IntPtr.Zero, Path.GetDirectoryName(fileName), out pchEaten, out pIdList, ref pdwAttributes);
                    pDestop.BindToObject(pIdList, IntPtr.Zero, ref Guids.IID_IShellFolder, out pSub);
                    pSub.ParseDisplayName(IntPtr.Zero, IntPtr.Zero, Path.GetFileName(fileName), out pchEaten, out pIdList, ref pdwAttributes);
                    pSub.GetUIObjectOf(IntPtr.Zero, 1, ref  pIdList, ref Guids.IID_IExtractImage, ref prgf, ref pExtractImage);
                }
                catch
                {
                    if (pSub != null)
                    {
                        Marshal.ReleaseComObject(pSub);
                        pSub = null;
                    }
                    this.Dispatcher.Invoke((Action)delegate
                    {
                        try
                        {
                            BitmapImage bit = new BitmapImage();
                            bit.BeginInit();
                            bit.StreamSource = new XpsDocument(fileName, FileAccess.Read).Thumbnail.GetStream();
                            bit.EndInit();
                            Thumbnail = bit;
                        }
                        catch { }
                        DeleteObject(hBmp);

                        RoutedEventArgs args = new RoutedEventArgs(InitialCompletedRouteEvent);
                        RaiseEvent(args);
                    });
                    return;
                }

                SIZE sz = new SIZE();
                sz.cx = 500;
                sz.cy = 500;
                StringBuilder location = new StringBuilder(260, 260);
                int priority = 0; int requestedColourDepth = 32;
                EIEIFLAG flags = EIEIFLAG.QUALITY;
                int uFlags = (int)flags;
                try
                {
                    pExtractImage.GetLocation(location, location.Capacity, ref priority, ref sz, requestedColourDepth, ref uFlags);
                    pExtractImage.Extract(ref hBmp);
                }
                catch
                {
                    if (pSub != null)
                    {
                        Marshal.ReleaseComObject(pSub);
                        pSub = null;
                    }
                    if (pExtractImage != null)
                    {
                        Marshal.ReleaseComObject(pExtractImage);
                        pExtractImage = null;
                    }
                    this.Dispatcher.Invoke((Action)delegate
                    {                       
                        RoutedEventArgs args = new RoutedEventArgs(InitialCompletedRouteEvent);
                        RaiseEvent(args);
                    });
                    return;
                }

                this.Dispatcher.BeginInvoke((Action)delegate
                {
                    try
                    {
                        Thumbnail = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(hBmp, IntPtr.Zero, Int32Rect.Empty,
System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions());
                    }
                    catch { }

                    RoutedEventArgs args = new RoutedEventArgs(InitialCompletedRouteEvent);
                    RaiseEvent(args);

                    DeleteObject(hBmp);
                }, null);

                Marshal.ReleaseComObject(pSub);
                pSub = null;
                Marshal.ReleaseComObject(pExtractImage);
                pExtractImage = null;

                //this.Dispatcher.BeginInvoke((Action)delegate
                //{
                //    try
                //    {
                //        BitmapImage bit = new BitmapImage();
                //        bit.BeginInit();
                //        bit.StreamSource = new XpsDocument(fileName, FileAccess.Read).Thumbnail.GetStream();
                //        bit.EndInit();
                //        Thumbnail = bit;
                //    }
                //    catch { }

                //    RoutedEventArgs args = new RoutedEventArgs(InitialCompletedRouteEvent);
                //    RaiseEvent(args);

                //    DeleteObject(hBmp);
                //}, null);
            }));
            OfficeOpenTaskPool.Add(m_TaskInfo);
        }

        public override void CleanScatterViewItem()
        {
            Document = null;
            new Thread(new ParameterizedThreadStart(delegate
            {
                Thread.Sleep(5000);
                this.Dispatcher.BeginInvoke((Action)delegate
                {
                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                    GC.Collect();
                }, null);
            })).Start();

            if (m_XpsDocument != null) { m_XpsDocument.Close(); }
            if (m_TaskInfo != null) m_TaskInfo.IsRequestCancel = true;
            InitialCompleted -= new RoutedEventHandler(OfficeScatterViewItem_Initialized);
            Tapped -= new TappedRoutedEventHandler(OfficeScatterViewItem_Tapped);
        }

        [DllImport("gdi32")]
        static extern int DeleteObject(IntPtr o);
    }
}
