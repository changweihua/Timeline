using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.IO;

namespace Timeline
{
    class ProjectHelper
    {
        /// <summary>
        /// 读取 time 文件夹下的文件
        /// </summary>
        /// <param name="path">文件路径</param>
        /// <param name="splits">分割符</param>
        /// <returns></returns>
        public static IList<Project> Read(string path, char[] splits)
        {
            IList<Project> projects = null;

            if (string.IsNullOrEmpty(path) || !System.IO.Directory.Exists(path))
            {
                return null;
            }

            List<string> files = System.IO.Directory.GetFiles(path).ToList();

#if DEBUG

            Debug.WriteLine("子文件数量 {0}", files.Count);

#endif

            //保证只有一个文件
            if (files.Count == 1)
            {
                projects = new List<Project>();

                using (TextReader reader = new StreamReader(files[0], Encoding.Default))
                {
                    string line = string.Empty;
                    while ((line = reader.ReadLine()) != null)
                    {
                        string[] arr = line.Split(splits, 2, StringSplitOptions.RemoveEmptyEntries);
                        //判断每一行的信息是否符合标准
                        if (arr.Length != 2)
                        {
                            continue;
                        }

                        var project = new Project { Title = arr[0], ResourcePath = @arr[1] };
                        projects.Add(project);

                    }
                }

            }


#if DEBUG

            Debug.WriteLine("读取行数 {0}", projects.Count);

#endif

            return projects;
        }
    }
}
