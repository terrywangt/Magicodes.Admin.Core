using Shouldly;
using Xunit;

namespace Magicodes.Admin.Tests.Currency
{
    public class TestCurrency : AppTestBase
    {
        [Fact]
        public void TestCurrencyToString()
        {
            var currency = new LogInfos.Currency(300);
            currency.ToString().ShouldBe("300 CNY");

            var currency1 = new LogInfos.Currency(300, "USD");
            currency1.ToString().ShouldBe("300 USD");

        }
    }
}
