// ======================================================================
//   
//           Copyright (C) 2018-2020 湖南心莱信息科技有限公司    
//           All rights reserved
//   
//           filename : LanguageAppService.cs
//           description :
//   
//           created by 雪雁 at  2018-09-04 9:06
//           Mail: wenqiang.li@xin-lai.com
//           QQ群：85318032（技术交流）
//           Blog：http://www.cnblogs.com/codelove/
//           GitHub：https://github.com/xin-lai
//           Home：http://xin-lai.com
//   
// ======================================================================

using Abp.Authorization;
using Abp.Extensions;
using Abp.Localization;
using Magicodes.Admin.Localization.Dto;
using Magicodes.App.Application.Localization.Dto;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Magicodes.Admin;

namespace Magicodes.App.Application.Localization
{
    /// <summary>
    /// APP多语言服务
    /// </summary>
    [AbpAllowAnonymous]
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class AppLanguageAppService : AppServiceBase, IAppLanguageAppService
    {
        private readonly IApplicationLanguageManager _applicationLanguageManager;
        private readonly IApplicationLanguageTextManager _applicationLanguageTextManager;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="applicationLanguageManager"></param>
        /// <param name="applicationLanguageTextManager"></param>
        public AppLanguageAppService(
            IApplicationLanguageManager applicationLanguageManager,
            IApplicationLanguageTextManager applicationLanguageTextManager)
        {
            _applicationLanguageManager = applicationLanguageManager;
            _applicationLanguageTextManager = applicationLanguageTextManager;
        }

        /// <summary>
        /// 获取所有语言文本定义
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpGet("")]
        public async Task<List<AppLanguageTextListDto>> GetAllLanguageTexts(GetAllLanguageTextsInput input)
        {

            if (input.LanguageName.IsNullOrEmpty())
            {
                var defaultLanguage =
                    await _applicationLanguageManager.GetDefaultLanguageOrNullAsync(AbpSession.TenantId);
                if (defaultLanguage == null)
                {
                    defaultLanguage = (await _applicationLanguageManager.GetLanguagesAsync(AbpSession.TenantId))
                        .FirstOrDefault();
                    if (defaultLanguage == null)
                    {
                        throw new Exception("No language found in the application!");
                    }
                }

                input.LanguageName = defaultLanguage.Name;
            }

            var source = LocalizationManager.GetSource(AdminConsts.AppLocalizationSourceName);
            var baseCulture = CultureInfo.GetCultureInfo("en");
            var targetCulture = CultureInfo.GetCultureInfo(input.LanguageName);

            var languageTexts = source
                .GetAllStrings()
                .Select(localizedString => new AppLanguageTextListDto
                {
                    Key = localizedString.Name,
                    EnValue = _applicationLanguageTextManager.GetStringOrNull(AbpSession.TenantId, source.Name,
                        baseCulture, localizedString.Name),
                    TargetValue = _applicationLanguageTextManager.GetStringOrNull(AbpSession.TenantId, source.Name,
                        targetCulture, localizedString.Name, false)
                })
                .AsQueryable();

            return languageTexts.ToList();
        }
    }
}