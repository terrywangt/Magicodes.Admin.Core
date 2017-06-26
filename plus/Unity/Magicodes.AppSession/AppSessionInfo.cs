namespace Magicodes.AppSession
{
    public class AppSessionInfo : IAppSessionInfo
    {
        public string AppUserId { get; set; }
        public string AppType { get; set; }
        public RequestTypes RequestType { get; set; }
    }
}