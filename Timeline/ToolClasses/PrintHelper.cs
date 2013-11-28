using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Printing;

namespace ShiningMeeting.ToolClasses
{
    public static class PrintHelper
    {
        public static bool HasPrint()   
        {
            LocalPrintServer localPrintServer = new LocalPrintServer();
            return localPrintServer.GetPrintQueues().Count() > 0;
        }

        public static bool IsXpsPrint(this PrintQueue printQueue)
        {
            return printQueue.Name == "Microsoft XPS Document Writer";
        }

        public static bool IsFaxPrint(this PrintQueue printQueue)
        {
            return printQueue.Name == "Fax";
        }
    }
}
