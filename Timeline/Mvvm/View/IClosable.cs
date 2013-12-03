using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ShiningMeeting.MEF.WPFAttribute;

namespace ShiningMeeting.Mvvm.View
{
    public interface IClosable : IView
    {
        bool? Result { get; set; }
    }
}
