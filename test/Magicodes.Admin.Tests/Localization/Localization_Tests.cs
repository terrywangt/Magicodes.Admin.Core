using System.Globalization;
using Abp.Localization;
using Abp.Zero;
using Shouldly;
using Xunit;

namespace Magicodes.Admin.Tests.Localization
{
    public class Localization_Tests : AppTestBase
    {
        [Theory]
        [InlineData("en")]
        [InlineData("en-US")]
        [InlineData("en-GB")]
        public void Simple_Localization_Test(string cultureName)
        {
            CultureInfo.CurrentUICulture = CultureInfo.GetCultureInfo(cultureName);

            Resolve<ILanguageManager>().CurrentLanguage.Name.ShouldBe("en");

            Resolve<ILocalizationManager>()
                .GetString(AbpZeroConsts.LocalizationSourceName, "Identity.UserNotInRole")
                .ShouldBe("User is not in role '{0}'.");
        }
    }
}
