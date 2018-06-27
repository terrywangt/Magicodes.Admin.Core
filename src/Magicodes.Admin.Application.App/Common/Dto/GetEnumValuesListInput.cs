using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Magicodes.Admin.Application.App.Common.Dto
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
