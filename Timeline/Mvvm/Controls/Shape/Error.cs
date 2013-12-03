using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Shapes;
using System.Windows.Controls;
using System.Windows;

namespace ShiningMeeting.Mvvm.Controls.Shape
{
    public class Error : UserControl
    {
        static Error() 
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Error), new FrameworkPropertyMetadata(typeof(Error)));
        }
    }
}
