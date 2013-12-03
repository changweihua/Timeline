using System;
using System.ComponentModel.Composition;
using ShiningMeeting.MEF.Attribute;

namespace ShiningMeeting.MEF.WPFAttribute
{
    /// <summary>
    /// 视图导入特性
    /// </summary>
    [MetadataAttribute, AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public sealed class ViewExportAttribute : ExportAttributeEx
    {
        /// <summary>
        /// 视图类型
        /// </summary>
        public new Type DataType { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataType">视图类型</param>
        public ViewExportAttribute(Type dataType)
            : base(dataType) { }
    }
}
