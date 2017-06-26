using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Abp.Dependency;
using Castle.Core.Logging;

namespace Magicodes.Api.ClientApi
{
    public interface IJsonClientApi : ITransientDependency
    {
        ILogger Logger { get; set; }
        /// <summary>
        /// 根路径
        /// </summary>
        string RootUrl { get; set; }

        HttpClient Client { get; set; }

        Dictionary<HttpStatusCode, Action<string>> HandlerDictionary { get; set; }
        /// <summary>
        /// Post请求
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="url">Api 路径</param>
        /// <param name="data"></param>
        /// <returns></returns>
        Task<TResult> PostAsync<TResult>(string url = null, object data = null);

        /// <summary>
        /// GET请求
        /// </summary>
        /// <typeparam name="TResult">返回结果</typeparam>
        /// <param name="url">请求地址</param>
        /// <param name="paramDictionary">参数字典</param>
        /// <returns></returns>
        Task<TResult> GetAsync<TResult>(string url = null, Dictionary<string, object> paramDictionary = null);

        Task DeleteAsync(string url = null);

        Task<TResult> PutAsync<TResult>(string url = null, object data = null);
    }
}
