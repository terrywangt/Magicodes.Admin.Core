namespace Magicodes.Admin.ApiClient
{
    public static class DebugServerIpAdresses
    {
        //You can use Emulators.Android, Emulators.Gennymotion or LocalhostIp based on your needs.
        public static string Current => LocalhostIp;

        //You can configure your computer's IP adress for external access (if Current = LocalhostIp)
        private const string LocalhostIp = "192.168.1.39";

        //Loopback addresses by emulators.
        private static class Emulators
        {
            public const string Android = "10.0.2.2";
            public const string Gennymotion = "10.0.3.2";
        }
    }
}