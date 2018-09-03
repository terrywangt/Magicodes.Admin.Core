using Magicodes.Admin.Localization;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Abp.Timing;
using Shouldly;
using Xunit;
namespace App.Tests.Localization
{
    public class Localization_Test : AppTestBase
    {
        private readonly ILanguageAppService languageAppService;

        public Localization_Test()
        {
            languageAppService = Resolve<ILanguageAppService>();
        }

        [Theory(DisplayName = "获得语言文本")]
        [InlineData("zh-CN", "Admin", "zh-CN", "ALL")]
        [InlineData("zh-CN", "App", "zh-CN", "ALL")]
        public async Task GetLanguageTexts(string baseLanguageName, string sourceName, string targetLanguageName, string targetValueFilter)
        {
            //初始化对象
            GetLanguageTextsInput input = new GetLanguageTextsInput() { BaseLanguageName = baseLanguageName, SourceName = "Admin", TargetLanguageName = targetLanguageName, TargetValueFilter = targetValueFilter };

            //断言
            var result = await languageAppService.GetLanguageTexts(input);
            result.ShouldNotBeNull();
        }
    }
}
