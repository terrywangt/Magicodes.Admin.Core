using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Magicodes.Admin.Contents
{
    /// <summary>
    /// 栏目位置
    /// </summary>
    public enum PositionEnum
    {
        /// <summary>
        /// 默认首页
        /// </summary>
        [Display(Name= "默认首页")]
        [Description("默认首页")]
        Default = 0,
    }
}