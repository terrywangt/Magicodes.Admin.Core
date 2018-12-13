namespace Magicodes.Admin.Tenants.Dashboard.Dto
{
    public class WorldMapCountry
    {
        public WorldMapCountry(string countryName, long color)
        {
            CountryName = countryName;
            Color = color;
        }

        public string CountryName { get; set; }
        public long Color { get; set; }
    }
}