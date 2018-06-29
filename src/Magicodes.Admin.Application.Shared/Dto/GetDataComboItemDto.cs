using System;
using System.Collections.Generic;
using System.Text;

namespace Magicodes.Admin.Dto
{
    /// <summary>
    /// 获取下拉列表
    /// </summary>
    /// <typeparam name="TPrimaryKey"></typeparam>
    public class GetDataComboItemDto<TPrimaryKey>
    {
        /// <summary>
        /// 显示名（会进行语言处理）
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// 值
        /// </summary>
        public TPrimaryKey Value { get; set; }
    }
}
