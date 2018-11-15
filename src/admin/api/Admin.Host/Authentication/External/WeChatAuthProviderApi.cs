using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Abp.AspNetZeroCore.Web.Authentication.External;
using Abp.UI;
using Castle.Core.Logging;
using Magicodes.WeChat.SDK.Apis.Token;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Magicodes.Admin.Web.Authentication.External
{
    public class WeChatAuthProviderApi: ExternalAuthProviderApiBase
    {
        public const string Name = "WeChat";

        private readonly IExternalAuthConfiguration _externalAuthConfiguration;
        private readonly ILogger _logger;
        public WeChatAuthProviderApi(IExternalAuthConfiguration externalAuthConfiguration, ILogger logger)
        {
            _externalAuthConfiguration = externalAuthConfiguration;
            var provider = externalAuthConfiguration.Providers.First(p => p.Name == Name);
            _logger = logger;
        }

        /// <summary>
        /// Gets the user information.
        /// </summary>
        /// <param name="accessCode">The access code.</param>
        /// <returns></returns>
        public override async Task<ExternalAuthUserInfo> GetUserInfo(string accessCode)
        {
            var oauthApi = new OAuthApi();
            var oAuthTokenApiResult =  oauthApi.Get(accessCode);
            if (!oAuthTokenApiResult.IsSuccess())
            {
                return null;
            }
            var weChatUser = oauthApi.GetUserInfo(oAuthTokenApiResult.AccessToken, oAuthTokenApiResult.OpenId);
            var externalAuthUserInfo = new ExternalAuthUserInfo
            {
                EmailAddress = weChatUser.OpenId + "@test.cn",
                Surname = weChatUser.NickName,
                ProviderKey = weChatUser.OpenId,
                Provider = Name,
                Name = weChatUser.OpenId
            };
            return await Task.FromResult(externalAuthUserInfo);
        }
    }
}
