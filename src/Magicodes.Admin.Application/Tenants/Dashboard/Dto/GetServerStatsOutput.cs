namespace Magicodes.Admin.Tenants.Dashboard.Dto
{
    public class GetServerStatsOutput
    {
        public int[] NetworkLoad { get; set; }
        public int[] CpuLoad { get; set; }
        public int[] LoadRate { get; set; }
    }
}