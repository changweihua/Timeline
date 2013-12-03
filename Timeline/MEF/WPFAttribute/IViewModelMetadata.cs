using System;

namespace ShiningMeeting.MEF.WPFAttribute
{
    /// <summary>
    /// 视图模型元数据接口
    /// </summary>
    public interface IViewModelMetadata : ShiningMeeting.MEF.Attribute.IMetadata
    {
        /// <summary>
        /// 视图模型绑定的视图
        /// </summary>
        Type View { get; }

        /// <summary>
        /// 关键词
        /// </summary>
        string KeyWord { get; }
    }
}
