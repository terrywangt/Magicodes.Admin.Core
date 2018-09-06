using System;
using System.Collections.Generic;
using System.Text;

namespace Magicodes.Admin.MultiTenancy.HostDashboard.Dto
{
    /// <summary>
    /// 待处理Dto
    /// </summary>
  public class GetPendsOutput
    {
        /// <summary>
        /// 待发货
        /// </summary>
        public int Delivery { get; set; }
        /// <summary>
        /// 待付款
        /// </summary>
        public int Obligation { get; set; }
        /// <summary>
        /// 待退货
        /// </summary>
        public int Rejectedgoods { get; set; }
        /// <summary>
        /// 库存预警
        /// </summary>
        public int Stock { get; set; }
        /// <summary>
        /// 待回复咨询
        /// </summary>
        public int Consult { get; set; }
       
    }
}
