using System.Net;
using System.Net.Http;
using Flurl.Http.Configuration;
using ModernHttpClient;

namespace Magicodes.Admin.ApiClient
{
    public class ModernHttpClientFactory : DefaultHttpClientFactory
    {
        public override HttpMessageHandler CreateMessageHandler()
        {
            return new NativeMessageHandler
            {
               AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
            };
        }
    }
}