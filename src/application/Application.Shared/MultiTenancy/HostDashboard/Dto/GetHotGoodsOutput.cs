using System;
using System.Collections.Generic;
using System.Text;

namespace Magicodes.Admin.MultiTenancy.HostDashboard.Dto
{
    /// <summary>
    /// 热销商品输出Dto
    /// </summary>
  public  class GetHotGoodsOutput
    {
        /// <summary>
        /// 商品名称
        /// </summary>
        public string goodname { get; set; }
        /// <summary>
        /// 商品类型
        /// </summary>
        public string goodtype { get; set; }
        /// <summary>
        /// 销量
        /// </summary>
        public int sales { get; set; }
    }
}
