using System;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Abp.Dependency;
using Abp.MultiTenancy;
using Abp.UI;
using Abp.Web.Models;
using Flurl.Http;
using JetBrains.Annotations;
using Magicodes.Admin.ApiClient.Models;

namespace Magicodes.Admin.ApiClient
{
    public class AccessTokenManager : IAccessTokenManager, ISingletonDependency
    {
        private const string LoginUrlSegment = "api/TokenAuth/Authenticate";

        private readonly AbpAuthenticateModel _authenticateModel;
        private readonly IApplicationContext _applicationContext;
        private int _authenticationRetrySeconds = 2;

        [CanBeNull]
        private AbpAuthenticateResultModel _authenticateResult;

        private Timer _refreshTimer;

        public bool IsUserLoggedIn => _authenticateResult?.AccessToken != null;

        public AccessTokenManager(
            AbpAuthenticateModel authenticateModel,
            IApplicationContext applicationContext)
        {
            _authenticateModel = authenticateModel;
            _applicationContext = applicationContext;
        }

        public async Task<string> GetAccessTokenAsync()
        {
            EnsureUserNameAndPasswordProvided();

            if (_authenticateResult != null)
            {
                return _authenticateResult.AccessToken;
            }

            var response = await LoginAsync();
            return response.AccessToken;
        }

        public async Task<AbpAuthenticateResultModel> LoginAsync()
        {
            EnsureUserNameAndPasswordProvided();

            using (IFlurlClient client = new FlurlClient(ApiUrlConfig.BaseUrl))
            {
                client.WithHeader("Accept", new MediaTypeWithQualityHeaderValue("application/json"));
                client.WithHeader("User-Agent", AbpApiClient.UserAgent);
                if (AbpApiClient.TimeoutSeconds.HasValue)
                {
                    client.WithTimeout(AbpApiClient.TimeoutSeconds.Value);
                }

                if (_applicationContext.CurrentTenant != null)
                {
                    client.WithHeader(MultiTenancyConsts.TenantIdResolveKey, _applicationContext.CurrentTenant.TenantId);
                }

                var response = await client
                    .Request(LoginUrlSegment)
                    .PostJsonAsync(_authenticateModel)
                    .ReceiveJson<AjaxResponse<AbpAuthenticateResultModel>>();

                if (!response.Success)
                {
                    _authenticateResult = null;
                    throw new UserFriendlyException(response.Error.Message + ": " + response.Error.Details);
                }

                _authenticateResult = response.Result;

                ConfigureTokenRefresh(response.Result);

                return _authenticateResult;
            }
        }

        public void Logout()
        {
            _authenticateResult = null;
        }

        private void ConfigureTokenRefresh(AbpAuthenticateResultModel authenticateResult)
        {
            if (authenticateResult.ExpireInSeconds <= 1)
            {
                //Normally, ExpireInSeconds should never be that small. However, we wanted to handle the case.
                _refreshTimer?.Dispose();
                _refreshTimer = null;
            }
            else
            {
                var reAuthenticationWaitTime = (authenticateResult.ExpireInSeconds - 1) * 1000;

                if (_refreshTimer == null)
                {
                    _refreshTimer = new Timer(RefreshTimerElapsed, _authenticateModel, reAuthenticationWaitTime, reAuthenticationWaitTime);
                }
                else
                {
                    _refreshTimer.Change(reAuthenticationWaitTime, reAuthenticationWaitTime);
                }
            }
        }

        private void EnsureUserNameAndPasswordProvided()
        {
            if (_authenticateModel.UserNameOrEmailAddress == null || _authenticateModel.Password == null)
            {
                throw new Exception("Username or password fields cannot be empty!");
            }
        }

        /// <summary>
        /// Renew token periodically
        /// </summary>
        private async void RefreshTimerElapsed(object authenticateModel)
        {
            try
            {
                await LoginAsync();
                _authenticationRetrySeconds = 2; //reset retry duration when no exception in login process.
            }
            catch
            {
                _authenticationRetrySeconds = GetExponentiallyIncreasingNumber(_authenticationRetrySeconds);
                _refreshTimer.Change(_authenticationRetrySeconds, _authenticationRetrySeconds);
            }
        }

        private static int GetExponentiallyIncreasingNumber(int currentValue, int maxValue = 300)
        {
            // This gives an increasing number 1 > 2 > 3 > 6 > 10 > 19 > 34 > 61 > 110 > 198 ...
            if (currentValue > maxValue)
            {
                return currentValue;
            }

            return Convert.ToInt32(Math.Pow(1.8, currentValue));
        }
    }
}