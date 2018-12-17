using Magicodes.App.Application.Localization;
using Magicodes.App.Application.Localization.Dto;
using Shouldly;
using System.Threading.Tasks;
using Xunit;
namespace App.Tests.Localization
{
    public class Localization_Test : AppTestBase
    {
        private readonly IAppLanguageAppService _languageAppService;

        public Localization_Test()
        {
            _languageAppService = Resolve<IAppLanguageAppService>();
        }

        [Theory(DisplayName = "获得语言文本")]
        [InlineData(null)]
        [InlineData("zh-CN")]
        [InlineData("en")]
        public async Task GetLanguageTexts(string languageName)
        {
            //初始化对象
            var input = new GetAllLanguageTextsInput() { LanguageName = languageName };

            //获取所有语言文本
            var result = await _languageAppService.GetAllLanguageTexts(input);
            result.ShouldNotBeNull();
            result.Count.ShouldBeGreaterThan(0);
        }
    }
}
