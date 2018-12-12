using System.Collections.Generic;
using Abp.Application.Services;
using Magicodes.Admin.Common.Dto;

namespace Magicodes.Admin.Common
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
