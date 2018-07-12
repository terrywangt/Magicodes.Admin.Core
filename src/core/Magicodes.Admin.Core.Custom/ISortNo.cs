using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Magicodes.App
{
    /// <summary>
    /// 自定义排序接口
    /// </summary>
    public interface ISortNo
    {
        /// <summary>
        /// 排序号
        /// </summary>
        long? SortNo { get; set; }
    }
}
