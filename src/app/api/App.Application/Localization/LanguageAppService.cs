using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Extensions;
using Abp.Localization;
using Abp.MultiTenancy;
using Abp.UI;
using Magicodes.Admin.Authorization;
using Magicodes.Admin.Localization;
using Magicodes.Admin.Localization.Dto;

namespace Magicodes.App.Application.Localization
{
    [AbpAllowAnonymous]
    public class LanguageAppService : AppServiceBase, ILanguageAppService
    {
        private readonly IApplicationLanguageManager _applicationLanguageManager;
        private readonly IApplicationLanguageTextManager _applicationLanguageTextManager;
        private readonly IRepository<ApplicationLanguage> _languageRepository;

        public LanguageAppService(
            IApplicationLanguageManager applicationLanguageManager,
            IApplicationLanguageTextManager applicationLanguageTextManager,
            IRepository<ApplicationLanguage> languageRepository)
        {
            _applicationLanguageManager = applicationLanguageManager;
            _languageRepository = languageRepository;
            _applicationLanguageTextManager = applicationLanguageTextManager;
        }

        public Task CreateOrUpdateLanguage(CreateOrUpdateLanguageInput input)
        {
            throw new NotImplementedException();
        }

        public Task DeleteLanguage(EntityDto input)
        {
            throw new NotImplementedException();
        }

        public Task<GetLanguageForEditOutput> GetLanguageForEdit(NullableIdDto input)
        {
            throw new NotImplementedException();
        }

        public Task<GetLanguagesOutput> GetLanguages()
        {
            throw new NotImplementedException();
        }

        public async Task<PagedResultDto<LanguageTextListDto>> GetLanguageTexts(GetLanguageTextsInput input)
        {
            /* Note: This method is used by SPA without paging, MPA with paging.
             * So, it can both usable with paging or not */

            //Normalize base language name
            if (input.BaseLanguageName.IsNullOrEmpty())
            {
                var defaultLanguage = await _applicationLanguageManager.GetDefaultLanguageOrNullAsync(AbpSession.TenantId);
                if (defaultLanguage == null)
                {
                    defaultLanguage = (await _applicationLanguageManager.GetLanguagesAsync(AbpSession.TenantId)).FirstOrDefault();
                    if (defaultLanguage == null)
                    {
                        throw new Exception("No language found in the application!");
                    }
                }

                input.BaseLanguageName = defaultLanguage.Name;
            }

            var source = LocalizationManager.GetSource(input.SourceName);
            var baseCulture = CultureInfo.GetCultureInfo(input.BaseLanguageName);
            var targetCulture = CultureInfo.GetCultureInfo(input.TargetLanguageName);

            var languageTexts = source
                .GetAllStrings()
                .Select(localizedString => new LanguageTextListDto
                {
                    Key = localizedString.Name,
                    BaseValue = _applicationLanguageTextManager.GetStringOrNull(AbpSession.TenantId, source.Name, baseCulture, localizedString.Name),
                    TargetValue = _applicationLanguageTextManager.GetStringOrNull(AbpSession.TenantId, source.Name, targetCulture, localizedString.Name, false)
                })
                .AsQueryable();

            //Filters
            if (input.TargetValueFilter == "EMPTY")
            {
                languageTexts = languageTexts.Where(s => s.TargetValue.IsNullOrEmpty());
            }

            if (!input.FilterText.IsNullOrEmpty())
            {
                languageTexts = languageTexts.Where(
                    l => (l.Key != null && l.Key.IndexOf(input.FilterText, StringComparison.CurrentCultureIgnoreCase) >= 0) ||
                         (l.BaseValue != null && l.BaseValue.IndexOf(input.FilterText, StringComparison.CurrentCultureIgnoreCase) >= 0) ||
                         (l.TargetValue != null && l.TargetValue.IndexOf(input.FilterText, StringComparison.CurrentCultureIgnoreCase) >= 0)
                    );
            }

            var totalCount = languageTexts.Count();

            //Ordering
            if (!input.Sorting.IsNullOrEmpty())
            {
                languageTexts = languageTexts.OrderBy(input.Sorting);
            }

            ////Paging
            //if (input.SkipCount > 0)
            //{
            //    languageTexts = languageTexts.Skip(input.SkipCount);
            //}

            //if (input.MaxResultCount > 0)
            //{
            //    languageTexts = languageTexts.Take(input.MaxResultCount);
            //}

            return new PagedResultDto<LanguageTextListDto>(
                totalCount,
                languageTexts.ToList()
                );
        }

        public Task SetDefaultLanguage(SetDefaultLanguageInput input)
        {
            throw new NotImplementedException();
        }

        public Task UpdateLanguageText(UpdateLanguageTextInput input)
        {
            throw new NotImplementedException();
        }
    }
}