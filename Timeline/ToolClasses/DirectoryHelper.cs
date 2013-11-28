using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace ShiningMeeting.ToolClasses
{
    public static class DirectoryHelper
    {
        /// <summary>
        /// 赋值文件
        /// </summary>
        /// <param name="destFolder"></param>
        /// <param name="sourceFileName"></param>
        /// <returns></returns>
        public static string CopyFileWithAutoRename(string destFolder, string sourceFileName)
        {
            if (!IsValidFolder(destFolder))
                Directory.CreateDirectory(destFolder);
            string fileName = Path.GetFileName(sourceFileName);
            string ext = Path.GetExtension(sourceFileName);
            int i = 1;
            while (File.Exists(Path.Combine(destFolder, fileName)))
            {
                fileName = Path.GetFileNameWithoutExtension(sourceFileName) + "(" + i.ToString() + ")" + ext;
                i++;
            }
            string fullPath = Path.Combine(destFolder, fileName);
            File.Copy(sourceFileName, fullPath);
            return fullPath;
        }


        /// <summary>
        /// 创建文件夹
        /// </summary>
        /// <param name="destFolder">要创建文件夹的所在目录</param>
        /// <param name="dirName">名称</param>
        /// <param name="createDirFullPath">创建的文件夹的路径</param>
        /// <returns></returns>
        public static void CreateDirectory(string destFolder, string dirName, out string createDirFullPath)
        {
            string path = System.IO.Path.Combine(destFolder, dirName);
            int i = 1;
            while (System.IO.Directory.Exists(path))
            {
                path = System.IO.Path.Combine(destFolder, dirName + "(" + i.ToString() + ")");
                i++;
            }

            System.IO.Directory.CreateDirectory(path);

            createDirFullPath = path;
        }

        /// <summary>
        /// 复制文件到指定目录，
        /// </summary>
        /// <param name="destFolder"></param>
        /// <param name="sourceFileName"></param>
        /// <param name="overwrite"></param>
        /// <returns></returns>
        public static string CopFile(string destFolder, string sourceFileName, bool overwrite = false)
        {
            if (!IsValidFolder(destFolder))
                Directory.CreateDirectory(destFolder);
            string fileName = Path.GetFileName(sourceFileName);
            string fullPath = Path.Combine(destFolder, fileName);
            File.Copy(sourceFileName, fullPath, overwrite);
            return fullPath;
        }

        /// <summary>
        /// 复制文件夹
        /// </summary>
        /// <param name="destFolder"></param>
        /// <param name="sourceFolder"></param>
        /// <param name="copyAction"></param>
        public static void CopyFolder(string destFolder, string sourceFolder, Action copyAction = null)
        {
            CopyFolder(destFolder, sourceFolder, null, false, copyAction);
        }

        /// <summary>
        /// 复制文件夹
        /// </summary>
        public static void CopyFolder(string destFolder, string sourceFolder, string[] filter, Action copyAction = null)
        {
            CopyFolder(destFolder, sourceFolder, filter, false, copyAction);
        }

        /// <summary>
        /// 复制文件夹
        /// </summary>
        public static void CopyFolder(string destFolder, string sourceFolder, string[] filter, bool allDirectories, Action copyAction = null)
        {
            if (!IsValidFolder(destFolder))
                Directory.CreateDirectory(destFolder);
            string[] files = Directory.GetFiles(sourceFolder);
            if (filter != null && filter.Length > 0)
            {
                files = (from a in files
                         from b in filter
                         where Path.GetExtension(a).ToLower() == b.ToLower()
                         select a).ToArray();
            }
            foreach (string file in files)
            {
                CopFile(destFolder, file, true);
                if (copyAction != null)
                    copyAction.Invoke();
            }
            if (allDirectories)
            {
                string[] folders = Directory.GetDirectories(sourceFolder);
                foreach (var folder in folders)
                {
                    string path = Path.Combine(destFolder, new DirectoryInfo(folder).Name);

                    CopyFolder(path, folder, filter, allDirectories, copyAction);
                }
            }
        }

        /// <summary>
        /// 删除文件夹
        /// </summary>
        /// <param name="folder"></param>
        /// <param name="deletedAction"></param>
        public static void DeleteFolder(string folder, Action deletedAction = null)
        {
            if (!IsValidFolder(folder))
                return;
            string[] files = Directory.GetFiles(folder);
            foreach (var file in files)
            {
                File.Delete(file);
                if (deletedAction != null)
                    deletedAction.Invoke();
            }
            string[] folders = Directory.GetDirectories(folder);
            foreach (var itemfolder in folders)
                DeleteFolder(itemfolder, deletedAction);

            Directory.Delete(folder, true);

        }

        /// <summary>
        /// 获取文件夹中的所有文件
        /// </summary>
        /// <param name="folder"></param>
        /// <returns></returns>
        public static List<string> GetFiles(string folder)
        {
            return GetFiles(folder, null, false);
        }

        /// <summary>
        /// 获取文件夹中的所有文件
        /// </summary>
        public static List<string> GetFiles(string folder, string[] filter)
        {
            return GetFiles(folder, filter, false);
        }

        /// <summary>
        /// 获取文件夹中的所有文件
        /// </summary>
        public static List<string> GetFiles(string folder, string[] filter, bool allDirectories)
        {
            List<string> listFiles = new List<string>();

            if (!IsValidFolder(folder))
                return listFiles;

            try
            {
                string[] files = Directory.GetFiles(folder);

                if (filter != null && filter.Length > 0)
                {
                    files = (from a in files
                             from b in filter
                             where File.Exists(a) && Path.GetExtension(a).ToLower() == b.ToLower()
                             select a).ToArray();
                }
                listFiles.AddRange(files);

                if (allDirectories)
                {
                    string[] folders = Directory.GetDirectories(folder);
                    foreach (var item in folders)
                        listFiles.AddRange(GetFiles(item, filter, allDirectories));
                }

            }

            catch (IOException io)
            {
                ToolClasses.LogRecord.Instance.Log(Exceptions.ExceptionManage.GetExceptionMsg(2));
            }
            catch (UnauthorizedAccessException ex)
            {
                ToolClasses.LogRecord.Instance.Log(Exceptions.ExceptionManage.GetExceptionMsg(1, folder));
            }
            catch (Exception)
            {
            }

            return listFiles;
        }

        /// <summary>
        /// 计算文件夹中的文件数量
        /// </summary>
        /// <param name="folder"></param>
        /// <returns></returns>
        public static int ComputeFileCount(string folder)
        {
            return ComputeFileCount(folder, null, false);
        }

        /// <summary>
        /// 计算文件夹中的文件数量
        /// </summary>
        /// <param name="folder"></param>
        /// <returns></returns>
        public static int ComputeFileCount(string folder, string[] filter)
        {
            return ComputeFileCount(folder, filter, false);
        }

        /// <summary>
        /// 计算文件夹中的文件数量
        /// </summary>
        /// <param name="folder"></param>
        /// <returns></returns>
        public static int ComputeFileCount(string folder, string[] filter, bool allDirectories)
        {
            int count = 0;
            if (!IsValidFolder(folder))
                return count;
            try
            {
                string[] files = Directory.GetFiles(folder);
                if (filter != null && filter.Length > 0)
                {
                    files = (from a in files
                             from b in filter
                             where File.Exists(a) && Path.GetExtension(a).ToLower() == b.ToLower()
                             select a).ToArray();
                }

                count += files.Length;

                if (allDirectories)
                {
                    string[] folders = Directory.GetDirectories(folder);
                    foreach (var item in folders)
                        count += ComputeFileCount(item, filter, allDirectories);
                }
            }
            catch (IOException io)
            {
                ToolClasses.LogRecord.Instance.Log(Exceptions.ExceptionManage.GetExceptionMsg(2));
            }
            catch (UnauthorizedAccessException ex)
            {
                ToolClasses.LogRecord.Instance.Log(Exceptions.ExceptionManage.GetExceptionMsg(1, folder));
            }
            catch (Exception e)
            {
                ToolClasses.LogRecord.Instance.Log( Exceptions.ExceptionManage.GetExceptionMsg(0, e.Message));
            }

            return count;
        }

        public static bool IsValidFolder(string folder)
        {
            if (string.IsNullOrEmpty(folder) || (!Directory.GetLogicalDrives().Contains(folder) && !Directory.Exists(folder)))
            {
                return false;
            }
            return true;
        }
    }
}
