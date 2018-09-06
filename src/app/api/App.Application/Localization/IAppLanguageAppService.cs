using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Application.Services;
using Magicodes.Admin.Localization.Dto;
using Magicodes.App.Application.Localization.Dto;

namespace Magicodes.App.Application.Localization
{
    /// <summary>
    /// APP多语言服务
    /// </summary>
    public interface IAppLanguageAppService: IApplicationService
    {
        /// <summary>
        /// 获取所有语言文本定义
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<List<AppLanguageTextListDto>> GetAllLanguageTexts(GetAllLanguageTextsInput input);
    }
}