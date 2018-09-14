// ======================================================================
//   
//           Copyright (C) 2018-2020 湖南心莱信息科技有限公司    
//           All rights reserved
//   
//           filename : AppLanguageAppService.cs
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

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Abp.Authorization;
using Abp.Extensions;
using Abp.Localization;
using Magicodes.Admin;
using Magicodes.App.Application.Localization.Dto;
using Microsoft.AspNetCore.Mvc;

namespace Magicodes.App.Application.Localization
{
    /// <summary>
    ///     APP多语言服务
    /// </summary>
    [AbpAllowAnonymous]
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class AppLanguageAppService : AppServiceBase, IAppLanguageAppService
    {
        private readonly IApplicationLanguageManager _applicationLanguageManager;
        private readonly IApplicationLanguageTextManager _applicationLanguageTextManager;

        /// <summary>
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
        ///     获取所有语言文本定义
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
                    if (defaultLanguage == null) throw new Exception("No language found in the application!");
                }

                input.LanguageName = defaultLanguage.Name;
            }

            var source = LocalizationManager.GetSource(AdminConsts.AppLocalizationSourceName);
            var targetCulture = CultureInfo.GetCultureInfo(input.LanguageName);

            var languageTexts = source
                .GetAllStrings()
                .Select(localizedString => new AppLanguageTextListDto
                {
                    Key = localizedString.Name,
                    Value = _applicationLanguageTextManager.GetStringOrNull(AbpSession.TenantId, source.Name,
                        targetCulture, localizedString.Name, false)
                })
                .AsQueryable();

            return languageTexts.ToList();
        }
    }
}