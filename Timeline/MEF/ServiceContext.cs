using System;

namespace ShiningMeeting.MEF
{
    [Flags]
    public enum ServiceContext
    {
        None = 0,
        Runtime = 1 << 0,
        DesignTime = 1 << 1,
        TestTime = 1 << 2,

        All = Runtime | DesignTime | TestTime,
    }

}
