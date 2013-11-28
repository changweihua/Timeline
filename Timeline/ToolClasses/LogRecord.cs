using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace ShiningMeeting.ToolClasses
{
    public class LogRecord
    {
        private string m_Path = string.Empty;
        private static LogRecord m_Instance = null;

        readonly object _syncRoot = new object();

        public static LogRecord Instance
        {
            get
            {
                if (m_Instance == null)
                {
                    m_Instance = new LogRecord();
                    m_Instance.Path = Environment.CurrentDirectory;
                }
                return m_Instance;
            }
        }

        public string Path
        {
            get { return m_Path; }
            set { m_Path = value; }
        }

        public void Log(string message)
        {
            if (!Directory.Exists(Path))
            {
                Directory.CreateDirectory(Path);
            }
            FileStream fileStream = null;
            string filePath = Path + @"\Error.log";
            lock (_syncRoot)
            {
                if (System.IO.File.Exists(filePath))
                {
                    try
                    {
                        fileStream = File.Open(filePath, FileMode.Append);
                    }
                    catch { return; }
                }
                else
                {
                    fileStream = File.Create(filePath);
                }

                using (fileStream)
                {
                    byte[] buffer = Encoding.UTF8.GetBytes(DateTime.Now.ToString() + Environment.NewLine + message + Environment.NewLine + Environment.NewLine);
                    fileStream.Write(buffer, 0, buffer.Length);
                }
            }

        }

    }
}
