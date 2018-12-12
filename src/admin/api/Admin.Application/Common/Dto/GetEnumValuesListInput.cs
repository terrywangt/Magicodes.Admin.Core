using System.ComponentModel.DataAnnotations;

namespace Magicodes.Admin.Common.Dto
{
    /// <summary>
    /// 获取枚举值列表
    /// </summary>
    public class GetEnumValuesListInput
    {
        /// <summary>
        /// 类型全名
        /// </summary>
        [Required]
        public string FullName { get; set; }
    }
}
