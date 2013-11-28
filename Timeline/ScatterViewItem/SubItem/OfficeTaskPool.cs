using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Timeline.ToolClasses;

namespace Timeline.ScatterViewItem.SubItem
{
    public static class OfficeTaskPool
    {
        internal static List<TaskInfo> m_InternalTaskStack = new List<TaskInfo>();
        internal static Task m_TaskFactory = null;
        private static bool m_IsCanceled = false;

        static OfficeTaskPool()
        {
            Application.Current.Exit += delegate
            {
                m_IsCanceled = true;
            };
            m_TaskFactory = Task.Factory.StartNew(delegate
            {
                while (true)
                {
                    if (m_InternalTaskStack.Count != 0)
                    {
                        TaskInfo task = m_InternalTaskStack[0];
                        if (!task.IsRequestCancel)
                        {
                            task.Start();
                            try
                            {
                                Task.WaitAny(new Task[] { task.Task }, 15000);
                            }
                            catch (AggregateException ex)
                            { 
                                //ShiningMeeting.Util.LogRecord.Instance.Log(ex.ToString());
                            }
                            m_InternalTaskStack.RemoveAt(0);
                        }
                        else { m_InternalTaskStack.RemoveAt(0); continue; }
                    }
                    if (m_IsCanceled)
                        break;

                    System.Threading.Thread.Sleep(100);
                }
            });
        }
        public static void Abord() { m_IsCanceled = true; }
        public static void Add(TaskInfo task)
        {
            m_InternalTaskStack.Add(task);
        }
        public static void Insert(TaskInfo task)
        {
            if (m_InternalTaskStack.Count > 0)
                m_InternalTaskStack.Insert(1, task);
            else m_InternalTaskStack.Insert(0, task);
        }
    }

    public static class OfficeOpenTaskPool
    {
        internal static Queue<TaskInfo> m_InternalTaskStack = new Queue<TaskInfo>();
        internal static Task m_TaskFactory = null;
        private static bool m_IsCanceled = false;

        static OfficeOpenTaskPool()
        {
            Application.Current.Exit += delegate
            {
                m_IsCanceled = true;
            };
            m_TaskFactory = Task.Factory.StartNew(delegate
            {
                while (true)
                {
                    if (m_InternalTaskStack.Count != 0)
                    {
                        TaskInfo task = m_InternalTaskStack.Dequeue();
                        if (!task.IsRequestCancel)
                        {
                            task.Start();
                            try
                            {
                                Task.WaitAny(new Task[] { task.Task }, 15000);
                            }
                            catch (AggregateException ex) { //ShiningMeeting.Util.LogRecord.Instance.Log(ex.ToString());
                            }
                        }
                        else { continue; }
                    }

                    if (m_IsCanceled)
                        break;
                    System.Threading.Thread.Sleep(300);
                }
            });
        }
        public static void Abord() { m_IsCanceled = true; }
        public static void Add(TaskInfo task)
        {
            m_InternalTaskStack.Enqueue(task);
        }
    }
}
