using System;
using System.Collections.Generic;
using Magicodes.Admin.Tenants.Dashboard.Dto;

namespace Magicodes.Admin.Tenants.Dashboard
{
    public static class DashboardRandomDataGenerator
    {
        private const string DateFormat = "yyyy-MM-dd";
        private static readonly Random Random;
        public static string[] CountryCodes = { "ABW", "AFG", "AGO", "AIA", "ALA", "ALB", "AND", "ARE", "ARG", "ARM", "ASM", "ATA", "ATF", "ATG", "AUS", "AUT", "AZE", "BDI", "BEL", "BEN", "BES", "BFA", "BGD", "BGR", "BHR", "BHS", "BIH", "BLM", "BLR", "BLZ", "BMU", "BOL", "BRA", "BRB", "BRN", "BTN", "BVT", "BWA", "CAF", "CAN", "CCK", "CHE", "CHL", "CHN", "CIV", "CMR", "COD", "COG", "COK", "COL", "COM", "CPV", "CRI", "CUB", "CUW", "CXR", "CYM", "CYP", "CZE", "DEU", "DJI", "DMA", "DNK", "DOM", "DZA", "ECU", "EGY", "ERI", "ESH", "ESP", "EST", "ETH", "FIN", "FJI", "FLK", "FRA", "FRO", "FSM", "GAB", "GBR", "GEO", "GGY", "GHA", "GIB", "GIN", "GLP", "GMB", "GNB", "GNQ", "GRC", "GRD", "GRL", "GTM", "GUF", "GUM", "GUY", "HKG", "HMD", "HND", "HRV", "HTI", "HUN", "IDN", "IMN", "IND", "IOT", "IRL", "IRN", "IRQ", "ISL", "ISR", "ITA", "JAM", "JEY", "JOR", "JPN", "KAZ", "KEN", "KGZ", "KHM", "KIR", "KNA", "KOR", "KWT", "LAO", "LBN", "LBR", "LBY", "LCA", "LIE", "LKA", "LSO", "LTU", "LUX", "LVA", "MAC", "MAF", "MAR", "MCO", "MDA", "MDG", "MDV", "MEX", "MHL", "MKD", "MLI", "MLT", "MMR", "MNE", "MNG", "MNP", "MOZ", "MRT", "MSR", "MTQ", "MUS", "MWI", "MYS", "MYT", "NAM", "NCL", "NER", "NFK", "NGA", "NIC", "NIU", "NLD", "NOR", "NPL", "NRU", "NZL", "OMN", "PAK", "PAN", "PCN", "PER", "PHL", "PLW", "PNG", "POL", "PRI", "PRK", "PRT", "PRY", "PSE", "PYF", "QAT", "REU", "ROU", "RUS", "RWA", "SAU", "SDN", "SEN", "SGP", "SGS", "SHN", "SJM", "SLB", "SLE", "SLV", "SMR", "SOM", "SPM", "SRB", "SSD", "STP", "SUR", "SVK", "SVN", "SWE", "SWZ", "SXM", "SYC", "SYR", "TCA", "TCD", "TGO", "THA", "TJK", "TKL", "TKM", "TLS", "TON", "TTO", "TUN", "TUR", "TUV", "TWN", "TZA", "UGA", "UKR", "UMI", "URY", "USA", "UZB", "VAT", "VCT", "VEN", "VGB", "VIR", "VNM", "VUT", "WLF", "WSM", "YEM", "ZAF", "ZMB", "ZWE" };

        static DashboardRandomDataGenerator()
        {
            Random = new Random();
        }

        public static int GetRandomInt(int min, int max)
        {
            return Random.Next(min, max);
        }

        public static int[] GetRandomArray(int size, int min, int max)
        {
            var array = new int[size];
            for (var i = 0; i < size; i++)
            {
                array[i] = GetRandomInt(min, max);
            }

            return array;
        }

        public static List<SalesSummaryData> GenerateSalesSummaryData(SalesSummaryDatePeriod inputSalesSummaryDatePeriod)
        {
            List<SalesSummaryData> data = null;


            switch (inputSalesSummaryDatePeriod)
            {
                case SalesSummaryDatePeriod.Daily:
                    data = new List<SalesSummaryData>
                    {
                        new SalesSummaryData(DateTime.Now.AddDays(-5).ToString(DateFormat), Random.Next(1000, 2000),
                            Random.Next(100, 999)),
                        new SalesSummaryData(DateTime.Now.AddDays(-4).ToString(DateFormat), Random.Next(1000, 2000),
                            Random.Next(100, 999)),
                        new SalesSummaryData(DateTime.Now.AddDays(-3).ToString(DateFormat), Random.Next(1000, 2000),
                            Random.Next(100, 999)),
                        new SalesSummaryData(DateTime.Now.AddDays(-2).ToString(DateFormat), Random.Next(1000, 2000),
                            Random.Next(100, 999)),
                        new SalesSummaryData(DateTime.Now.AddDays(-1).ToString(DateFormat), Random.Next(1000, 2000),
                            Random.Next(100, 999)),
                    };

                    break;
                case SalesSummaryDatePeriod.Weekly:
                    var lastYear = DateTime.Now.AddYears(-1).Year;
                    data = new List<SalesSummaryData>
                    {
                        new SalesSummaryData(lastYear + " W4", Random.Next(1000, 2000),
                            Random.Next(100, 999)),
                        new SalesSummaryData(lastYear + " W3", Random.Next(1000, 2000),
                            Random.Next(100, 999)),
                        new SalesSummaryData(lastYear + " W2", Random.Next(1000, 2000),
                            Random.Next(100, 999)),
                        new SalesSummaryData(lastYear + " W1", Random.Next(1000, 2000),
                            Random.Next(100, 999))
                    };

                    break;
                case SalesSummaryDatePeriod.Monthly:
                    data = new List<SalesSummaryData>
                    {
                        new SalesSummaryData(DateTime.Now.AddMonths(-4).ToString("yyyy-MM"), Random.Next(1000, 2000),
                            Random.Next(100, 999)),
                        new SalesSummaryData(DateTime.Now.AddMonths(-3).ToString("yyyy-MM"), Random.Next(1000, 2000),
                            Random.Next(100, 999)),
                        new SalesSummaryData(DateTime.Now.AddMonths(-2).ToString("yyyy-MM"), Random.Next(1000, 2000),
                            Random.Next(100, 999)),
                        new SalesSummaryData(DateTime.Now.AddMonths(-1).ToString("yyyy-MM"), Random.Next(1000, 2000),
                            Random.Next(100, 999))
                    };

                    break;
            }

            return data;
        }

        public static List<MemberActivity> GenerateMemberActivities()
        {
            return new List<MemberActivity>
            {
                new MemberActivity("Brain", "$" + GetRandomInt(100, 500), GetRandomInt(10, 100), GetRandomInt(10, 150),
                    GetRandomInt(10, 99) + "%"),

                new MemberActivity("Nick", "$" + GetRandomInt(100, 500), GetRandomInt(10, 100), GetRandomInt(10, 150),
                    GetRandomInt(10, 99) + "%"),

                new MemberActivity("Tim", "$" + GetRandomInt(100, 500), GetRandomInt(10, 100), GetRandomInt(10, 150),
                    GetRandomInt(10, 99) + "%"),

                new MemberActivity("Tom", "$" + GetRandomInt(100, 500), GetRandomInt(10, 100), GetRandomInt(10, 150),
                    GetRandomInt(10, 99) + "%")
            };
        }

        public static List<WorldMapCountry> GenerateWorldMapCountries()
        {
            var countries = new List<WorldMapCountry>();
            for (var i = 0; i < 10; i++)
            {
                var countryIndex = GetRandomInt(0, CountryCodes.Length);
                countries.Add(new WorldMapCountry(CountryCodes[countryIndex], GetRandomInt(10, 100)));
            }

            return countries;
        }

        public static List<TimeLineItem> GenerateTimeLineItems()
        {
            return new List<TimeLineItem>
            {
                new TimeLineItem(DateTime.Now.AddDays(-60))
                {
                    AutherName = "Andres Iniesta",
                    Image = "avatar80_2.jpg",
                    Title = "New User",
                    Text = "Lorem ipsum dolor sit amet, consectetur adipiscing elit"
                },
                new TimeLineItem(DateTime.Now.AddDays(-50))
                {
                    AutherName = "Hugh Grant",
                    Image = "avatar80_3.jpg",
                    Title = "Sending Shipment",
                    Text = "Etiam euismod eleifend ipsum, at posuere augue."
                },
                new TimeLineItem(DateTime.Now.AddDays(-10))
                {
                    AutherName = "Rory Matthew",
                    Image = "avatar80_1.jpg",
                    Title = "Blue Chambray",
                    Text = "Volosoft software."
                },
                new TimeLineItem(DateTime.Now)
                {
                    AutherName = "Andres Iniesta",
                    Image = "avatar80_2.jpg",
                    Title = "Timeline Received",
                    Text = "Pellentesque mi felis, aliquam at iaculis eu, finibus eu ex."
                },
                new TimeLineItem(DateTime.Now.AddDays(20))
                {
                    AutherName = "Matt Goldman",
                    Image = "avatar80_7.jpg",
                    Title = "Event Success",
                    Text = "Integer efficitur leo eget dolor tincidunt, et dignissim risus lacinia."
                },
                new TimeLineItem(DateTime.Now.AddDays(30))
                {
                    AutherName = "Rory Matthew",
                    Image = "avatar80_1.jpg",
                    Title = "Conference Call",
                    Text = "Nam in egestas nunc. Suspendisse potenti."
                },
                new TimeLineItem(DateTime.Now.AddDays(50))
                {
                    AutherName = "Andres Iniesta",
                    Image = "avatar80_2.jpg",
                    Title = "Timeline Received",
                    Text = "Sed sit amet molestie elit, vel placerat ipsum. Ut consectetur odio non est rhoncus volutpat."
                },
                new TimeLineItem(DateTime.Now.AddDays(80))
                {
                    AutherName = "Andres Iniesta",
                    Image = "avatar80_2.jpg",
                    Title = "Code Review",
                    Text = "Praesent dignissim luctus risus sed sodales."
                }
            };
        }
    }
}