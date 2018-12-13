using System.Collections.Generic;

namespace Magicodes.Admin.Dto
{
    /// <summary>
    /// Tree Table行数据
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public class TreeTableRowDto<TEntity> where TEntity : class
    {
        /// <summary>
        /// 数据
        /// </summary>
        public TEntity Data { get; set; }

        /// <summary>
        /// 子集
        /// </summary>
        public ICollection<TreeTableRowDto<TEntity>> Children { get; set; }
    }
}