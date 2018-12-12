using System.ComponentModel.DataAnnotations;

namespace Magicodes.Admin.Contents
{
    /// <summary>
    ///     栏目类型
    /// </summary>
    public enum ColumnTypes
    {
        /// <summary>
        ///     Html文本
        /// </summary>
        [Display(Name = "Html文本")] Html = 0,

        /// <summary>
        ///     图片
        /// </summary>
        [Display(Name = "图片")] Image = 1
    }
}