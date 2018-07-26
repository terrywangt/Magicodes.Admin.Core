using System;
using System.Collections.Generic;
using System.Text;

namespace Magicodes.Admin.Dto
{
    /// <summary>
    /// Tree Table输出结果
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public class TreeTableOutputDto<TEntity> where TEntity : class
    {
        /// <summary>
        /// 数据集合
        /// </summary>
        public ICollection<TreeTableRowDto<TEntity>> Data { get; set; }
    }
}
