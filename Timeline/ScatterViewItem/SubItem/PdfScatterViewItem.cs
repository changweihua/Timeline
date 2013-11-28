using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Imaging;
using System.Windows;
using System.IO;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Windows.Media;
using System.Windows.Controls;
using ShiningMeeting.ToolClasses;
using System.Threading;
using System.Windows.Threading;

namespace ShiningMeeting.ScatterViewItem.SubItem
{
    public class PdfScatterViewItem : BaseScatterViewItem
    {
        private int m_PageCount = 0;
        private int m_CurrentPageNumber = 0;
        private string m_SourcePath;

        public BitmapFrame BitmapSource
        {
            get { return (BitmapFrame)GetValue(BitmapSourceProperty); }
            set { SetValue(BitmapSourceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for BitmapSource.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BitmapSourceProperty =
            DependencyProperty.Register("BitmapSource", typeof(BitmapFrame), typeof(PdfScatterViewItem), new UIPropertyMetadata(null));



        static PdfScatterViewItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(PdfScatterViewItem), new FrameworkPropertyMetadata(typeof(PdfScatterViewItem)));
        }
        public PdfScatterViewItem()
        {
            InitialCompleted += new RoutedEventHandler(PdfScatterViewItem_InitialComplated);
            Tapped += new TappedRoutedEventHandler(PdfScatterViewItem_Tapped);
        }

        void PdfScatterViewItem_InitialComplated(object sender, RoutedEventArgs e)
        {
            if (!double.IsNaN(Height))
                this.MaxHeight = this.Height * 20;
            if (!double.IsNaN(Width))
                this.MaxWidth = this.Width * 20;
        }
        protected override void InitailScatterViewItem()
        {

            string a = ConstClass.GetPath(ConstClass.SpecialFolder.TempPDFFolder);
            string b = new FileInfo(SourceModel.FileFullPath).LastWriteTime.ToFileTime() + "_" + new FileInfo(SourceModel.FileFullPath).Name.Substring(0, new FileInfo(SourceModel.FileFullPath).Name.LastIndexOf('.'));
            m_SourcePath = Path.Combine(a, b);
            Width = 500;
            Height = 500;
            new System.Threading.Thread(new System.Threading.ThreadStart(delegate
            {

                if (!Directory.Exists(m_SourcePath) || Directory.GetFiles(m_SourcePath).Length == 0)
                {
                    PdfToImages(SourceModel.FileFullPath, m_SourcePath, Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "gs", "gs9.07", "bin"));
                }
                else
                {
                    m_PageCount = Directory.GetFiles(m_SourcePath).Count();
                }

                this.Dispatcher.BeginInvoke((Action)delegate
                {
                    base.BaseScatterViewItem_InitialComplated(null, null);
                    RoutedEventArgs args = new RoutedEventArgs(InitialCompletedRouteEvent);
                    RaiseEvent(args);
                }, null);
                RenderCurrentPage();

            })) { IsBackground = true }.Start();
        }

        void PdfScatterViewItem_Tapped(object sender, TappedEventArgs e)
        {
            GetPageChange(e.Point);
        }

        void RenderCurrentPage()
        {
            this.Dispatcher.BeginInvoke((Action)delegate
                            {
                                try
                                {
                                    if (Directory.GetFiles(m_SourcePath).ToArray().Count() > 0)
                                        BitmapSource = BitmapFrame.Create(new BitmapImage(new Uri(Directory.GetFiles(m_SourcePath).ToArray()[m_CurrentPageNumber], UriKind.RelativeOrAbsolute)));
                                }
                                catch { }

                            }, null);
        }

        void GetPageChange(Point point)
        {

            // 在item总宽度的三分之一左方点击显示上一页
            if (point.X < ActualWidth / 3)
            {
                if (m_CurrentPageNumber > 0)
                {
                    m_CurrentPageNumber--;
                    RenderCurrentPage();
                }
            }
            else if (point.X > 2 * ActualWidth / 3) // 在item宽度的三分之二右方点击显示下一页
            {
                if (m_CurrentPageNumber < m_PageCount - 1)
                {
                    m_CurrentPageNumber++;
                    RenderCurrentPage();
                }
            }


        }

        public override void CleanScatterViewItem()
        {
            InitialCompleted -= new RoutedEventHandler(PdfScatterViewItem_InitialComplated);
            Tapped -= new TappedRoutedEventHandler(PdfScatterViewItem_Tapped);
        }

        public void PdfToImages(string pdfFile, string imgPath, string gsPath)
        {

            //PDDocument doc = PDDocument.load(pdfFile);

            if (!Directory.Exists(imgPath))
                Directory.CreateDirectory(imgPath);

            //int pageCount = doc.getDocumentCatalog().getAllPages().size();//计算pdf文档的总页数
            iTextSharp.text.pdf.PdfReader pdf = new iTextSharp.text.pdf.PdfReader(pdfFile);

            //imgPath = "\"" + imgPath + "\"";

            m_PageCount = pdf.NumberOfPages;
            pdf.Close();

            string pdfFileName = System.IO.Path.GetFileName(pdfFile);

            int index = pdfFileName.LastIndexOf('.');

            if (index != -1)

                pdfFileName = pdfFileName.Substring(0, index);

            string imgFile = System.IO.Path.Combine(imgPath, pdfFileName);//转换成的图片文件

            if (m_PageCount == 0) return;


            imgFile = "\"" + System.IO.Path.Combine(imgPath, "%03d.png") + "\"";
            pdfFile = "\"" + pdfFile + "\"";

            ProcessStartInfo info = new ProcessStartInfo();

            info.CreateNoWindow = true;

            info.WindowStyle = ProcessWindowStyle.Hidden;

            info.WorkingDirectory = gsPath;// @"E:\Program Files (x86)\gs\gs9.07\bin";// System.Configuration.ConfigurationManager.AppSettings["GhostScriptView"];

            info.Arguments = "-dSAFER -dBATCH -dNOPAUSE -r150 -sDEVICE=jpeg -dGraphicsAlphaBits=4" + @" -sOutputFile=" + imgFile + "  " + pdfFile;

            info.FileName = @"gswin32c.exe";

            Process subProcess = new Process();

            subProcess.StartInfo = info;
            subProcess.Start();

            while (true)
            {
                if (Directory.GetFiles(imgPath).ToArray().Count() > 0)
                {
                    Thread.Sleep(1000);
                    break;
                }
            }
        }

    }
}
