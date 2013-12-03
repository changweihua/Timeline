using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Shapes;
using System.Windows.Controls;
using System.Windows;

namespace ShiningMeeting.Mvvm.Controls.Shape
{
    public class Tick : UserControl
    {
        static Tick() 
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Tick), new FrameworkPropertyMetadata(typeof(Tick)));
        }
    }
}
