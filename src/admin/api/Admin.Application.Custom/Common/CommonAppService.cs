using Abp.Reflection.Extensions;
using Magicodes.Admin.Application.App.Common.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using Magicodes.Admin.Core.Custom;

namespace Magicodes.Admin.Application.App.Common
{
    /// <summary>
    /// 通用服务
    /// </summary>
    public class CommonAppService : AppServiceBase, ICommonAppService
    {
        /// <summary>
        /// 获取枚举值列表
        /// </summary>
        /// <returns></returns>
        public List<GetEnumValuesListDto> GetEnumValuesList(GetEnumValuesListInput input)
        {
            var type = typeof(AppCoreModule).GetAssembly().GetType(input.FullName);
            if (!type.IsEnum) return null;
            var names = Enum.GetNames(type);
            var values = Enum.GetValues(type);
            var list = new List<GetEnumValuesListDto>();
            var index = 0;
            foreach (var value in values)
            {
                list.Add(new GetEnumValuesListDto()
                {
                    DisplayName = L(names[index]),
                    Value = Convert.ToInt32(value)
                });
                index++;
            }
            return list;
        }
    }
}
