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
        public static IList<Project> Read(string path)
        {
            IList<Project> projects = null;

            if (string.IsNullOrEmpty(path) || !System.IO.Directory.Exists(path))
            {
                return null;
            }

            List<string> folders = System.IO.Directory.GetFiles(path).ToList();

#if DEBUG

            Debug.WriteLine("子文件夹数量 {0}", folders.Count);

#endif

            if (folders.Count == 1)
            {
                projects = new List<Project>();
                using (TextReader reader = new StreamReader(folders[0], Encoding.Default))
                {
                    string line = string.Empty;
                    while ((line = reader.ReadLine()) != null)
                    {
                        string[] arr = line.Split(new char[] { '|' }, 2);
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
