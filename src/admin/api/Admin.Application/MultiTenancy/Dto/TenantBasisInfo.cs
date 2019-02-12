using System;
using System.Collections.Generic;
using System.Text;

namespace Magicodes.Admin.MultiTenancy.Dto
{
    /// <summary>
    /// 租户基本信息
    /// </summary>
    public class TenantBasisInfo
    {
        /// <summary>
        /// 租户Id
        /// </summary>
        public int Id { get; set; }
        
        /// <summary>
        /// 租户名称
        /// </summary>
        public string Name { get; set; }
    }
}
