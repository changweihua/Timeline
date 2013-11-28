using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace ShiningMeeting.Exceptions
{
    [Serializable]
    public class VitouException : ApplicationException
    {

        static VitouException()
        {

        }

        public VitouException()
        {

        }

        // 参数:
        //   message:
        //     描述错误的消息。
        public VitouException(string message)
            : base(message)
        {

        }

        protected VitouException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {

        }
        // 参数:
        //   message:
        //     解释异常原因的错误信息。
        //
        //   inner:
        //     导致当前异常的异常。如果 inner 参数不是空引用（在 Visual Basic 中为 Nothing），则在处理内部异常的 catch 块中引发当前异常。
        public VitouException(string message, Exception inner)
            : base(message, inner)
        {

        }
    }

    public class ExceptionMsg
    {
        int _msgID;

        public int MsgID
        {
            get { return _msgID; }
            set { _msgID = value; }
        }
        string _msgTitle;

        public string MsgTitle
        {
            get { return _msgTitle; }
            set { _msgTitle = value; }
        }
        string _msgBody;

        public string MsgBody
        {
            get { return _msgBody; }
            set { _msgBody = value; }
        }

        public override string ToString()
        {
            return string.Format("Time:{0},ID:{1},Msg:{2}", DateTime.Now, MsgID, MsgTitle + ":" + Environment.NewLine + MsgBody);
        }
    }

    public static class ExceptionManage
    {
        static List<ExceptionMsg> _vitouExceptions = new List<ExceptionMsg>();

        static ExceptionManage()
        {
            Init();
        }

        static void Init()
        {
            _vitouExceptions.Add(new ExceptionMsg() { MsgID = 0, MsgTitle = "系统错误", MsgBody = "系统错误" });
            _vitouExceptions.Add(new ExceptionMsg() { MsgID = 1, MsgTitle = "系统权限错误", MsgBody = "您没有权限访问" });
            _vitouExceptions.Add(new ExceptionMsg() { MsgID = 2, MsgTitle = "系统错误", MsgBody = "系统IO错误" });
            _vitouExceptions.Add(new ExceptionMsg() { MsgID = 3, MsgTitle = "文件转换失败", MsgBody = "文件转换失败" });
            _vitouExceptions.Add(new ExceptionMsg() { MsgID = 4, MsgTitle = "文件打开失败", MsgBody = "文件格式未知" });
        }

        public static void AddExceptionMsg(int id, string msgTitle, string msgBoady)
        {
            ExceptionMsg eMsg = _vitouExceptions.FirstOrDefault(_ => _.MsgID == id);
            if (eMsg != null)
                return;
            _vitouExceptions.Add(new ExceptionMsg() { MsgID = id, MsgTitle = msgTitle, MsgBody = msgBoady });
        }

        public static void AddExceptionMsg(ExceptionMsg msg)
        {
            ExceptionMsg eMsg = _vitouExceptions.FirstOrDefault(_ => _.MsgID == msg.MsgID);
            if (eMsg != null)
                return;
            _vitouExceptions.Add(msg);
        }

        public static string GetExceptionMsg(int id)
        {
            ExceptionMsg eMsg = _vitouExceptions.FirstOrDefault(_ => _.MsgID == id);
            if (eMsg == null)
            {
                return string.Format("Time:{0},ID:{1},Msg:{2}", DateTime.Now, id, string.Empty);
            }
            return eMsg.ToString();
        }

        public static string GetExceptionMsg(int id, string msg)
        {
            ExceptionMsg eMsg = _vitouExceptions.FirstOrDefault(_ => _.MsgID == id);
            if (eMsg == null)
            {
                return string.Format("Time:{0},ID:{1},Msg:{2}", DateTime.Now, id, msg);
            }
            return string.Concat(eMsg.ToString(), msg);
        }
    }
}
