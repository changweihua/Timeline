using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Office.Interop.Word;
using Microsoft.Office.Interop.PowerPoint;
using Microsoft.Office.Interop.Excel;

namespace Timeline.ScatterViewItem.SubItem
{
    class OfficeDocumentToXps
    {

        /// <summary>
        /// This method takes a Word document full path and new XPS document full path and name
        /// and returns the new XpsDocument
        /// </summary>
        /// <param name="wordDocName"></param>
        /// <param name="xpsDocName"></param>
        /// <returns></returns>
        public static bool ConvertWordDocToXPSDoc(string wordDocName, string xpsDocName)
        {
            bool result = false;
            Microsoft.Office.Interop.Word.Application wordApplication = null;
            Document doc = null;
            try
            {
                // Create a WordApplication and add Document to it
                wordApplication = new Microsoft.Office.Interop.Word.Application();
                wordApplication.Documents.Add(wordDocName);


                doc = wordApplication.ActiveDocument;
                // You must make sure you have Microsoft.Office.Interop.Word.Dll version 12.
                // Version 11 or previous versions do not have WdSaveFormat.wdFormatXPS option

                doc.ExportAsFixedFormat(xpsDocName, WdExportFormat.wdExportFormatXPS);
                result = true;
            }
            catch (Exception ex)
            {
                //ShiningMeeting.Util.LogRecord.Instance.Log(ex.ToString());
#if !DEBUG
                System.Windows.MessageBox.Show("Word文档转换不成功,请确定安装了Office Word","提示");
#endif
            }
            finally
            {
                if (doc != null)
                {
                    try
                    {
                        object saveOption = WdSaveOptions.wdDoNotSaveChanges;
                        doc.Close(ref saveOption);
                    }
                    catch { }
                    doc = null;
                }
                if (wordApplication != null)
                {
                    object saveOption = WdSaveOptions.wdDoNotSaveChanges;
                    wordApplication.Quit(ref saveOption);
                    wordApplication = null;
                }
            }
            return result;
        }

        /// <summary>
        /// 将PPT文档转化为PPT的XPS格式
        /// </summary>
        /// <param name="sourcePath">源文件路径。</param>
        /// <param name="targetPath">转化为xps文档的保存目录。</param>
        /// <returns>是否转化成功。</returns>
        public static bool ConvertPptToXPSPpt(string sourcePath, string targetPath)
        {
            bool result = false;
            Microsoft.Office.Interop.PowerPoint.Application application = null;
            
            Presentation persentation = null;
            try
            {
                application = new Microsoft.Office.Interop.PowerPoint.Application();
                persentation = application.Presentations.Open(sourcePath, Microsoft.Office.Core.MsoTriState.msoTrue, Microsoft.Office.Core.MsoTriState.msoTrue, Microsoft.Office.Core.MsoTriState.msoFalse);

                persentation.ExportAsFixedFormat(targetPath, PpFixedFormatType.ppFixedFormatTypeXPS);
                result = true;
            }
            catch (Exception ex)
            {
                //ShiningMeeting.Util.LogRecord.Instance.Log(ex.ToString());
#if !DEBUG
                System.Windows.MessageBox.Show("PPT文档转换不成功,请确定安装了Office PPT","提示");
#endif
            }
            finally
            {
                if (persentation != null)
                {
                    try
                    {
                        persentation.Close();
                    }
                    catch { }
                    persentation = null;
                }
                if (application != null)
                {
                    try
                    {
                        application.Quit();
                    }
                    catch { }
                    application = null;
                }
                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
            return result;
        }

        /// <summary>
        /// 将PPT文档转化为EXCEL的XPS格式
        /// </summary>
        /// <param name="sourcePath">源文件路径。</param>
        /// <param name="targetPath">转化为xps文档的保存目录。</param>
        /// <returns>是否转化成功。</returns>
        public static bool ConvertExcelToXPSExcel(string sourcePath, string targetPath)
        {
            bool result = false;
            object missing = Type.Missing;
            Microsoft.Office.Interop.Excel.Application application = null;
            Workbook workBook = null;
            try
            {
                application = new Microsoft.Office.Interop.Excel.Application();
                workBook = application.Workbooks.Open(sourcePath, missing, missing, missing, missing, missing,
                        missing, missing, missing, missing, missing, missing, missing, missing, missing);

                workBook.ExportAsFixedFormat(XlFixedFormatType.xlTypeXPS, targetPath, XlFixedFormatQuality.xlQualityStandard, true, false, missing, missing, missing, missing);
                result = true;
            }
            catch (Exception ex)
            {
                //ShiningMeeting.Util.LogRecord.Instance.Log(ex.ToString());
#if !DEBUG
                System.Windows.MessageBox.Show("Excel文档转换不成功,请确定安装了Office Excel","提示");
#endif
            }
            finally
            {
                if (workBook != null)
                {
                    object saveOption = WdSaveOptions.wdDoNotSaveChanges;
                    workBook.Close(saveOption, missing, missing);
                    workBook = null;
                }
                if (application != null)
                {
                    application.Quit();
                    application = null;
                }
                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
            return result;

        }
    }
}
