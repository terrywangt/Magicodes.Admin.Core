using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace Magicodes.Admin.Dto
{
    /// <summary>
    /// 开关输入参数Dto  
    /// </summary>
    /// <typeparam name="TPrimaryKey"></typeparam>
    public class SwitchEntityInputDto<TPrimaryKey>: EntityDto<TPrimaryKey>
    {
        /// <summary>
        /// 开关值（bool类型）
        /// </summary>
        public bool SwitchValue { get; set; }
    }
}
