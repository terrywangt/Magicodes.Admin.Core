using System.ComponentModel.DataAnnotations;

namespace Magicodes.Admin.Contents
{
    /// <summary>
    /// 推荐类型
    /// </summary>
    public enum RecommendedTypes
    {
        /// <summary>
        /// 置顶
        /// </summary>
        [Display(Name = "置顶")]
        Top = 0,

        /// <summary>
        /// 热门
        /// </summary>
        [Display(Name = "热门")]
        Hot = 1,

        /// <summary>
        /// 推荐
        /// </summary>
        [Display(Name = "推荐")]
        Recommend = 2,

        /// <summary>
        /// 普通
        /// </summary>
        [Display(Name = "普通")]
        Common = 3
    }
}