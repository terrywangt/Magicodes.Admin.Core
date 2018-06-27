using Abp.Application.Services;
using Magicodes.Admin.Application.App.Common.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace Magicodes.Admin.Application.App.Common
{
    /// <summary>
    /// 通用服务
    /// </summary>
    public interface ICommonAppService : IApplicationService
    {
        /// <summary>
        /// 获取枚举值列表
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        List<GetEnumValuesListDto> GetEnumValuesList(GetEnumValuesListInput input);
    }
}
