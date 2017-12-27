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

        public static int[] GetRandomPercentageArray(int size)
        {
            if (size == 1)
            {
                return new int[100];
            }

            var array = new int[size];
            var total = 0;
            for (var i = 0; i < size - 1; i++)
            {
                array[i] = GetRandomInt(0, 100 - total);
                total += array[i];
            }

            array[size - 1] = 100 - total;

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

                new MemberActivity("Jane", "$" + GetRandomInt(100, 500), GetRandomInt(10, 100), GetRandomInt(10, 150),
                    GetRandomInt(10, 99) + "%"),

                new MemberActivity("Tim", "$" + GetRandomInt(100, 500), GetRandomInt(10, 100), GetRandomInt(10, 150),
                    GetRandomInt(10, 99) + "%"),

                new MemberActivity("Kate", "$" + GetRandomInt(100, 500), GetRandomInt(10, 100), GetRandomInt(10, 150),
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
    }
}