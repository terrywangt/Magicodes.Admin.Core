using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Magicodes.Admin.Tests.Currency
{
    public class TestCurrency: AppTestBase
    {
        [Fact]
        public void TestCurrencyToString()
        {
            Core.Custom.LogInfos.Currency currency = new Core.Custom.LogInfos.Currency("zh-CN", 300);
            var cn = currency.ToString();

            Core.Custom.LogInfos.Currency currency1 = new Core.Custom.LogInfos.Currency("en-us", 300);
            var us = currency1.ToString();
        }
    }
}
