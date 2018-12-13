using System;
using System.Collections.Generic;
using System.Text;

namespace Magicodes.Admin.Dto
{
    /// <summary>
    /// Tree输出结果
    /// </summary>
    public class TreeOutputDto
    {
        /// <summary>
        /// 数据集合
        /// </summary>
        public ICollection<TreeItemDto> Data { get; set; }
    }
}
