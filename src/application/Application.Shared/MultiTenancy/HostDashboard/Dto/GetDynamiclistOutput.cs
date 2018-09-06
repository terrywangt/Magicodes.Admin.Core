using System;
using System.Collections.Generic;
using System.Text;

namespace Magicodes.Admin.MultiTenancy.HostDashboard.Dto
{
  public  class GetDynamiclistOutput
    {
        /// <summary>
        /// 动态创建时间
        /// </summary>
        public string CreationTime { get; set; }
        /// <summary>
        /// 动态内容
        /// </summary>
        public string Context { get; set; }

        /// <summary>
        /// 创建的时间日
        /// </summary>
        public string Days { get; set; }
        /// <summary>
        /// 创建时间年月
        /// </summary>
        public  string Mothdate { get; set; }
    }
}
