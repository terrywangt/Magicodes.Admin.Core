using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using JoyMoe.AspNetCore.Authentication.QQ;
using Newtonsoft.Json.Linq;

namespace Magicodes.Admin.Web.Authentication.External.QQ
{
    public class QQAuthProviderApi: ExternalAuthProviderApiBase
    {
        public override async Task<ExternalAuthUserInfo> GetUserInfo(string accessCode)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.UserAgent.ParseAdd("Microsoft ASP.NET Core OAuth middleware");
                client.DefaultRequestHeaders.Accept.ParseAdd("application/json");
                client.Timeout = TimeSpan.FromSeconds(30);
                client.MaxResponseContentBufferSize = 1024 * 1024 * 10; // 10 MB

                var request = new HttpRequestMessage(HttpMethod.Get, QQDefaults.UserInformationEndpoint);
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessCode);

                var response = await client.SendAsync(request);

                response.EnsureSuccessStatusCode();

                var payload = JObject.Parse(await response.Content.ReadAsStringAsync());

                return new ExternalAuthUserInfo
                {
                    Name = QQHelper.GetNickName(payload),
                    EmailAddress = "",
                    Surname = QQHelper.GetNickName(payload),
                    ProviderKey = QQHelper.GetId(payload),
                    Provider = QQDefaults.DisplayName
                };
            }
        }
    }
}
