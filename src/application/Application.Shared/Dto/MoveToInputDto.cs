using System;
using System.Collections.Generic;
using System.Text;

namespace Magicodes.Admin.Dto
{
    /// <summary>
    /// 排序迁移
    /// </summary>
    public class MoveToInputDto<TPrimaryKey>
    {
        /// <summary>
        /// 源Id
        /// </summary>
        public TPrimaryKey SourceId { get; set; }

        /// <summary>
        /// 目标Id
        /// </summary>
        public TPrimaryKey TargetId { get; set; }

        /// <summary>
        /// 移动位置
        /// </summary>
        public MoveToPositions MoveToPosition { get; set; }
    }
}
