using System;
using System.ComponentModel.Composition;
using ShiningMeeting.MEF.Attribute;

namespace ShiningMeeting.MEF.WPFAttribute
{
    /// <summary>
    /// 视图模型导入特性
    /// </summary>
    [MetadataAttribute, AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public sealed class ViewModelExportAttribute : ExportAttributeEx
    {
        /// <summary>
        /// 视图模型类型
        /// </summary>
        public new Type DataType { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataType">视图模型类型</param>
        public ViewModelExportAttribute(Type dataType)
            : base(dataType) { }
    }
}
