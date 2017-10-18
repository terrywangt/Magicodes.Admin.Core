using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using JoyMoe.AspNetCore.Authentication.Weibo;
using Newtonsoft.Json.Linq;

namespace Magicodes.Admin.Web.Authentication.External.Weibo
{
    public class WeiboAuthProviderApi: ExternalAuthProviderApiBase
    {
        public override async Task<ExternalAuthUserInfo> GetUserInfo(string accessCode)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.UserAgent.ParseAdd("Microsoft ASP.NET Core OAuth middleware");
                client.DefaultRequestHeaders.Accept.ParseAdd("application/json");
                client.Timeout = TimeSpan.FromSeconds(30);
                client.MaxResponseContentBufferSize = 1024 * 1024 * 10; // 10 MB

                var request = new HttpRequestMessage(HttpMethod.Get, WeiboDefaults.UserInformationEndpoint);
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessCode);

                var response = await client.SendAsync(request);

                response.EnsureSuccessStatusCode();

                var payload = JObject.Parse(await response.Content.ReadAsStringAsync());

                return new ExternalAuthUserInfo
                {
                    Name = WeiboHelper.GetNikeName(payload),
                    EmailAddress = "",
                    Surname = WeiboHelper.GetName(payload),
                    ProviderKey = WeiboHelper.GetId(payload),
                    Provider = WeiboDefaults.DisplayName
                };
            }
        }
    }
}
