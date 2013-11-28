using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Imaging;
using System.IO;

namespace ShiningMeeting.ToolClasses
{
    public static class ImageHelper
    {
        public static void Save(BitmapSource bs, string fileName)
        {
            JpegBitmapEncoder encode = new JpegBitmapEncoder();
            encode.Frames.Add(BitmapFrame.Create(bs));
            using (FileStream fs=new FileStream(fileName, FileMode.Create))
            {
                encode.Save(fs);
            }
        }
    }
}
