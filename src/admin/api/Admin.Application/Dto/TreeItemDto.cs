using System.Collections.Generic;

namespace Magicodes.Admin.Dto
{
    /// <summary>
    /// Tree每一项类型定义
    /// </summary>
    public class TreeItemDto
    {
        public TreeItemDataDto Data { get; set; }

        /// <summary>
        /// 是否页节点
        /// </summary>
        public bool Leaf { get; set; }

        /// <summary>
        /// 是否展开
        /// </summary>
        public bool Expanded { get; set; }

        /// <summary>
        /// 子集
        /// </summary>
        public ICollection<TreeItemDto> Children { get; set; }
    }
}