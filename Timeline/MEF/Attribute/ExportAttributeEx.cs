using System;
using System.ComponentModel.Composition;

namespace ShiningMeeting.MEF.Attribute
{
    [MetadataAttribute, AttributeUsage(AttributeTargets.All, AllowMultiple = false)]
    public abstract class ExportAttributeEx : ExportAttribute
    {
        public Type DataType { get; private set; }

        public ExportAttributeEx(Type dataType)
            : base(dataType)
        {
            if (dataType == null)
            {
                throw new ArgumentNullException("DataType");
            }
            this.DataType = dataType;
        }
    }
}
