using System;
using System.IO;
using System.Threading.Tasks;
using Abp.Dependency;
using Abp.MultiTenancy;
using Abp.UI;
using Abp.Web.Models;
using Flurl.Http;
using Magicodes.Admin.Extensions;

namespace Magicodes.Admin.ApiClient
{
    public class AbpApiClient : ISingletonDependency, IDisposable
    {
        private readonly IAccessTokenManager _accessTokenManager;
        private readonly IApplicationContext _applicationContext;
        private static FlurlClient _client;

        public const string UserAgent = "AbpApiClient";
        public static int? TimeoutSeconds { get; set; } = 30;

        public AbpApiClient(IAccessTokenManager accessTokenManager, IApplicationContext applicationContext)
        {
            _accessTokenManager = accessTokenManager;
            _applicationContext = applicationContext;
        }

        #region PostAsync<T>

        public async Task<T> PostAync<T>(string endpoint)
        {
            return await PostAync<T>(endpoint, null, null, await _accessTokenManager.GetAccessTokenAsync(), true);
        }

        public async Task<T> PostAnonymousAync<T>(string endpoint)
        {
            return await PostAync<T>(endpoint, null, null, null, true);
        }

        public async Task<T> PostAync<T>(string endpoint, object inputDto)
        {
            return await PostAync<T>(endpoint, inputDto, null, await _accessTokenManager.GetAccessTokenAsync(), true);
        }

        public async Task<T> PostAync<T>(string endpoint, object inputDto, object queryParameters)
        {
            return await PostAync<T>(endpoint, inputDto, queryParameters, await _accessTokenManager.GetAccessTokenAsync(), true);
        }

        /// <summary>
        /// Makes POST request without authentication token.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="endpoint"></param>
        /// <param name="inputDto"></param>
        /// <returns></returns>
        public async Task<T> PostAnonymousAync<T>(string endpoint, object inputDto)
        {
            return await PostAync<T>(endpoint, inputDto, null, null, true);
        }

        public async Task<T> PostAync<T>(string endpoint, object inputDto, object queryParameters, string accessToken, bool stripAjaxResponseWrapper)
        {
            var httpResponse = GetClient(accessToken)
                .Request(endpoint)
                .SetQueryParams(queryParameters)
                .PostJsonAsync(inputDto);

            if (stripAjaxResponseWrapper)
            {
                var response = await httpResponse.ReceiveJson<AjaxResponse<T>>();
                ValidateAbpResponse(response);
                return response.Result;
            }

            return await httpResponse.ReceiveJson<T>();
        }

        public async Task<T> PostMultipartAsync<T>(string endpoint, Stream stream, string fileName, bool stripAjaxResponseWrapper = true)
        {
            var httpResponse = GetClient(await _accessTokenManager.GetAccessTokenAsync())
                    .Request(endpoint)
                    .PostMultipartAsync(multiPartContent => multiPartContent.AddFile("file", stream, fileName));

            if (stripAjaxResponseWrapper)
            {
                var response = await httpResponse.ReceiveJson<AjaxResponse<T>>();
                ValidateAbpResponse(response);
                return response.Result;
            }

            return await httpResponse.ReceiveJson<T>();
        }

        public async Task<T> PostMultipartAsync<T>(string endpoint, string filePath, bool stripAjaxResponseWrapper = true)
        {
            var httpResponse = GetClient(await _accessTokenManager.GetAccessTokenAsync())
                .Request(endpoint)
                .PostMultipartAsync(multiPartContent => multiPartContent.AddFile("file", filePath));

            if (stripAjaxResponseWrapper)
            {
                var response = await httpResponse.ReceiveJson<AjaxResponse<T>>();
                ValidateAbpResponse(response);
                return response.Result;
            }

            return await httpResponse.ReceiveJson<T>();
        }

        #endregion

        #region  PostAsync

        public async Task PostAync(string endpoint)
        {
            await PostAync(endpoint, null, null, await _accessTokenManager.GetAccessTokenAsync(), true);
        }

        public async Task PostAync(string endpoint, object inputDto)
        {
            await PostAync(endpoint, inputDto, null, await _accessTokenManager.GetAccessTokenAsync(), true);
        }

        /// <summary>
        /// Makes POST request without authentication token.
        /// </summary>
        /// <param name="endpoint"></param>
        /// <param name="inputDto"></param>
        /// <returns></returns>
        public async Task PostAnonymousAync(string endpoint, object inputDto)
        {
            await PostAync(endpoint, inputDto, null, null, true);
        }

        public async Task PostAync(string endpoint, object inputDto, object queryParameters)
        {
            await PostAync(endpoint, inputDto, queryParameters, await _accessTokenManager.GetAccessTokenAsync(), true);
        }

        public async Task PostAync(string endpoint, object inputDto, object queryParameters, string accessToken,
            bool stripAjaxResponseWrapper)
        {
            await GetClient(accessToken)
                  .Request(endpoint)
                  .SetQueryParams(queryParameters)
                  .PostJsonAsync(inputDto);
        }

        #endregion

        #region  GetAsync<T>

        public async Task<T> GetAsync<T>(string endpoint)
        {
            return await GetAsync<T>(endpoint, null);
        }

        /// <summary>
        /// Makes GET request without authentication token.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="endpoint"></param>
        /// <returns></returns>
        public async Task<T> GetAnonymousAsync<T>(string endpoint)
        {
            return await GetAsync<T>(endpoint, null, null, true);
        }

        public async Task<T> GetAsync<T>(string endpoint, object queryParameters)
        {
            return await GetAsync<T>(endpoint, queryParameters, await _accessTokenManager.GetAccessTokenAsync(), true);
        }

        public async Task<T> GetAsync<T>(string endpoint, object queryParameters, string accessToken, bool stripAjaxResponseWrapper)
        {
            var httpResponse = GetClient(accessToken)
                .Request(endpoint)
                .SetQueryParams(queryParameters);

            if (stripAjaxResponseWrapper)
            {
                var response = await httpResponse.GetJsonAsync<AjaxResponse<T>>();
                ValidateAbpResponse(response);
                return response.Result;
            }

            return await httpResponse.GetJsonAsync<T>();
        }

        #endregion

        #region  GetAsync

        public async Task GetAsync(string endpoint)
        {
            await GetAsync(endpoint, null);
        }

        public async Task GetAsync(string endpoint, object queryParameters)
        {
            await GetAsync(endpoint, queryParameters, await _accessTokenManager.GetAccessTokenAsync(), true);
        }

        public async Task GetAsync(string endpoint, object queryParameters, string accessToken, bool stripAjaxResponseWrapper)
        {
            await GetClient(accessToken)
             .Request(endpoint)
             .SetQueryParams(queryParameters)
             .GetAsync();
        }

        #endregion

        #region  GetStringAsync

        public async Task GetStringAsync(string endpoint)
        {
            await GetStringAsync(endpoint, null);
        }

        public async Task GetStringAsync(string endpoint, object queryParameters)
        {
            await GetStringAsync(endpoint, queryParameters, await _accessTokenManager.GetAccessTokenAsync());
        }

        public async Task<string> GetStringAsync(string endpoint, object queryParameters, string accessToken)
        {
            return await GetClient(accessToken)
                    .Request(endpoint)
                    .SetQueryParams(queryParameters)
                    .GetStringAsync();

        }

        #endregion

        #region DeleteAsync

        public async Task DeleteAsync(string endpoint)
        {
            await DeleteAsync(endpoint, null, await _accessTokenManager.GetAccessTokenAsync());
        }

        public async Task DeleteAsync(string endpoint, object queryParameters)
        {
            await DeleteAsync(endpoint, queryParameters, await _accessTokenManager.GetAccessTokenAsync());
        }

        public async Task DeleteAsync(string endpoint, object queryParameters, string accessToken)
        {
            await GetClient(accessToken)
                    .Request(endpoint)
                    .SetQueryParams(queryParameters)
                    .DeleteAsync();
        }

        #endregion

        #region DeleteAsync<T>

        public async Task<T> DeleteAsync<T>(string endpoint)
        {
            return await DeleteAsync<T>(endpoint, null);
        }

        public async Task<T> DeleteAsync<T>(string endpoint, object queryParameters)
        {
            return await DeleteAsync<T>(endpoint, queryParameters, await _accessTokenManager.GetAccessTokenAsync(), true);
        }

        public async Task<T> DeleteAsync<T>(string endpoint, object queryParameters, string accessToken, bool stripAjaxResponseWrapper)
        {
            var httpResponse = GetClient(accessToken)
                .Request(endpoint)
                .SetQueryParams(queryParameters)
                .DeleteAsync();

            if (stripAjaxResponseWrapper)
            {
                var response = await httpResponse.ReceiveJson<AjaxResponse<T>>();
                ValidateAbpResponse(response);
                return response.Result;
            }

            return await httpResponse.ReceiveJson<T>();
        }

        #endregion

        #region PutAsync<T>

        public async Task<T> PutAsync<T>(string endpoint)
        {
            return await PutAsync<T>(endpoint, null, null, await _accessTokenManager.GetAccessTokenAsync(), true);
        }

        public async Task<T> PutAsync<T>(string endpoint, object inputDto)
        {
            return await PutAsync<T>(endpoint, inputDto, null, await _accessTokenManager.GetAccessTokenAsync(), true);
        }

        public async Task<T> PutAsync<T>(string endpoint, object inputDto, object queryParameters)
        {
            return await PutAsync<T>(endpoint, inputDto, queryParameters, await _accessTokenManager.GetAccessTokenAsync(), true);
        }

        public async Task<T> PutAsync<T>(string endpoint, object inputDto, object queryParameters, string accessToken, bool stripAjaxResponseWrapper)
        {
            var httpResponse = GetClient(accessToken)
                .Request(endpoint)
                .SetQueryParams(queryParameters)
                .PutJsonAsync(inputDto);

            if (stripAjaxResponseWrapper)
            {
                var response = await httpResponse.ReceiveJson<AjaxResponse<T>>();
                ValidateAbpResponse(response);
                return response.Result;
            }

            return await httpResponse.ReceiveJson<T>();
        }
        #endregion

        #region  PutAsync

        public async Task PutAsync(string endpoint)
        {
            await PutAsync(endpoint, null, null, await _accessTokenManager.GetAccessTokenAsync(), true);
        }

        public async Task PutAsync(string endpoint, object inputDto)
        {
            await PutAsync(endpoint, inputDto, null, await _accessTokenManager.GetAccessTokenAsync(), true);
        }

        public async Task PutAsync(string endpoint, object inputDto, object queryParameters)
        {
            await PutAsync(endpoint, inputDto, queryParameters, await _accessTokenManager.GetAccessTokenAsync(), true);
        }

        public async Task PutAsync(string endpoint, object inputDto, object queryParameters, string accessToken,
            bool stripAjaxResponseWrapper)
        {
            await GetClient(accessToken)
                  .Request(endpoint)
                  .SetQueryParams(queryParameters)
                  .PutJsonAsync(inputDto);
        }

        #endregion

        public FlurlClient GetClient(string accessToken)
        {
            if (_client != null)
            {
                AddHeaders(accessToken);
                return _client;
            }

            _client = new FlurlClient(ApiUrlConfig.BaseUrl);
            if (TimeoutSeconds.HasValue)
            {
                _client.WithTimeout(TimeoutSeconds.Value);
            }

            return _client;
        }

        private void AddHeaders(string accessToken)
        {
            _client.HttpClient.DefaultRequestHeaders.Clear();

            _client.WithHeader("Accept", "application/json");
            _client.WithHeader("User-Agent", UserAgent);
            _client.WithHeader("Accept-Encoding", "gzip,deflate");

            if (_applicationContext.CurrentTenant != null)
            {
                _client.WithHeader(MultiTenancyConsts.TenantIdResolveKey, _applicationContext.CurrentTenant.TenantId);
            }

            if (accessToken != null)
            {
                _client.WithOAuthBearerToken(accessToken);
            }
        }

        private static void ValidateAbpResponse(AjaxResponseBase response)
        {
            if (response == null)
            {
                return;
            }

            if (response.Success)
            {
                return;
            }

            if (response.Error == null)
            {
                return;
            }

            throw new UserFriendlyException(response.Error.GetConsolidatedMessage());
        }

        public void Dispose()
        {
            _client?.Dispose();
        }
    }
}