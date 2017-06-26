using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Abp.Json;
using Abp.UI;
using Castle.Core.Logging;
using Newtonsoft.Json;

namespace Magicodes.Api.ClientApi
{
    public class JsonClientApi : IJsonClientApi
    {
        public JsonClientApi()
        {
            InitHttpClient(Client);
        }

        public ILogger Logger { get; set; }
        public string RootUrl { get; set; }
        public Dictionary<HttpStatusCode, Action<string>> HandlerDictionary { get; set; }
        public HttpClient Client { get; set; } = new HttpClient();

        public async Task<TResult> PostAsync<TResult>(string url = null, object data = null)
        {
            var apiUrl = url ?? RootUrl;
            if (Client.BaseAddress == null && !string.IsNullOrWhiteSpace(RootUrl))
                Client.BaseAddress = new Uri(RootUrl);
            var json = JsonConvert.SerializeObject(data, Formatting.Indented);
            //记录调试日志
            Logger.DebugFormat("【POST】API请求{2}Url：{0}{2}Data:{1}", apiUrl, json, Environment.NewLine);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var apiResult = await Client.PostAsync(apiUrl, content);
            var strResult = await apiResult.Content.ReadAsStringAsync();
            //记录调试日志
            Logger.DebugFormat("API请求{3}Url：{0}{3}StatusCode:{1}{3}Result:{2}", apiUrl, apiResult.StatusCode, strResult,
                Environment.NewLine);
            if (HandlerDictionary != null && HandlerDictionary.ContainsKey(apiResult.StatusCode))
            {
                HandlerDictionary[apiResult.StatusCode].Invoke(strResult);
                return default(TResult);
            }
            if (apiResult.StatusCode == HttpStatusCode.OK)
                return JsonConvert.DeserializeObject<TResult>(strResult);
            throw new UserFriendlyException((int) apiResult.StatusCode, "出现未处理的错误！", strResult);
        }

        public async Task<TResult> GetAsync<TResult>(string url = null,
            Dictionary<string, object> paramDictionary = null)
        {
            var apiUrl = url ?? RootUrl;
            if (Client.BaseAddress == null && !string.IsNullOrWhiteSpace(RootUrl))
                Client.BaseAddress = new Uri(RootUrl);
            if (paramDictionary != null)
                foreach (var item in paramDictionary)
                {
                    apiUrl += apiUrl.Contains("?") ? "&" : "?" + item.Key;
                    //基元类型为 Boolean、Byte、SByte、Int16、UInt16、Int32、UInt32、Int64、UInt64、IntPtr、UIntPtr、Char、Double 和 Single。
                    if (item.Value is string || item.Value.GetType().IsPrimitive)
                        apiUrl += WebUtility.UrlEncode(item.Value.ToString());
                    else
                        apiUrl += WebUtility.UrlEncode(item.Value.ToJsonString());
                }
            //记录调试日志
            Logger.DebugFormat("【GET】API请求 GET Url：{0}", apiUrl);
            var apiResult = await Client.GetAsync(apiUrl);
            var strResult = await apiResult.Content.ReadAsStringAsync();
            //记录调试日志
            Logger.DebugFormat("API请求{3}Url：{0}{3}StatusCode:{1}{3}Result:{2}", apiUrl, apiResult.StatusCode, strResult,
                Environment.NewLine);
            if (HandlerDictionary != null && HandlerDictionary.ContainsKey(apiResult.StatusCode))
            {
                HandlerDictionary[apiResult.StatusCode].Invoke(strResult);
                return default(TResult);
            }
            if (apiResult.StatusCode == HttpStatusCode.OK)
                return JsonConvert.DeserializeObject<TResult>(strResult);
            throw new UserFriendlyException((int) apiResult.StatusCode, "出现未处理的错误！", strResult);
        }

        public async Task DeleteAsync(string url = null)
        {
            var apiUrl = url ?? RootUrl;
            if (Client.BaseAddress == null && !string.IsNullOrWhiteSpace(RootUrl))
                Client.BaseAddress = new Uri(RootUrl);
            //记录调试日志
            Logger.DebugFormat("【Delete】API请求{1}Url：{0}{1}", apiUrl, Environment.NewLine);
            var apiResult = await Client.DeleteAsync(apiUrl);
            var strResult = await apiResult.Content.ReadAsStringAsync();
            //记录调试日志
            Logger.DebugFormat("API请求{3}Url：{0}{3}StatusCode:{1}{3}Result:{2}", apiUrl, apiResult.StatusCode, strResult,
                Environment.NewLine);
            if (HandlerDictionary != null && HandlerDictionary.ContainsKey(apiResult.StatusCode))
                HandlerDictionary[apiResult.StatusCode].Invoke(strResult);
            if (apiResult.StatusCode != HttpStatusCode.OK)
                throw new UserFriendlyException((int) apiResult.StatusCode, "出现未处理的错误！", strResult);
        }

        public async Task<TResult> PutAsync<TResult>(string url = null, object data = null)
        {
            var apiUrl = url ?? RootUrl;
            if (Client.BaseAddress == null && !string.IsNullOrWhiteSpace(RootUrl))
                Client.BaseAddress = new Uri(RootUrl);
            var json = JsonConvert.SerializeObject(data, Formatting.Indented);
            //记录调试日志
            Logger.DebugFormat("【PUT】API请求{2}Url：{0}{2}Data:{1}", apiUrl, json, Environment.NewLine);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var apiResult = await Client.PutAsync(apiUrl, content);
            var strResult = await apiResult.Content.ReadAsStringAsync();
            //记录调试日志
            Logger.DebugFormat("API请求{3}Url：{0}{3}StatusCode:{1}{3}Result:{2}", apiUrl, apiResult.StatusCode, strResult,
                Environment.NewLine);
            if (HandlerDictionary != null && HandlerDictionary.ContainsKey(apiResult.StatusCode))
            {
                HandlerDictionary[apiResult.StatusCode].Invoke(strResult);
                return default(TResult);
            }
            if (apiResult.StatusCode == HttpStatusCode.OK)
                return JsonConvert.DeserializeObject<TResult>(strResult);
            throw new UserFriendlyException((int) apiResult.StatusCode, "出现未处理的错误！", strResult);
        }

        private void InitHttpClient(HttpClient client)
        {
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            if (client.BaseAddress == null && !string.IsNullOrWhiteSpace(RootUrl))
                client.BaseAddress = new Uri(RootUrl);
        }
    }
}