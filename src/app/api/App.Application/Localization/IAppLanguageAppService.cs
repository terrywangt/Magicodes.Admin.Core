// ======================================================================
//   
//           Copyright (C) 2018-2020 湖南心莱信息科技有限公司    
//           All rights reserved
//   
//           filename : IAppLanguageAppService.cs
//           description :
//   
//           created by 雪雁 at  2018-09-04 10:16
//           Mail: wenqiang.li@xin-lai.com
//           QQ群：85318032（技术交流）
//           Blog：http://www.cnblogs.com/codelove/
//           GitHub：https://github.com/xin-lai
//           Home：http://xin-lai.com
//   
// ======================================================================

using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Application.Services;
using Magicodes.App.Application.Localization.Dto;

namespace Magicodes.App.Application.Localization
{
    /// <summary>
    ///     APP多语言服务
    /// </summary>
    public interface IAppLanguageAppService : IApplicationService
    {
        /// <summary>
        ///     获取所有语言文本定义
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<List<AppLanguageTextListDto>> GetAllLanguageTexts(GetAllLanguageTextsInput input);
    }
}