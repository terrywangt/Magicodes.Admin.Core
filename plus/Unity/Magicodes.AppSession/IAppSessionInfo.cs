using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abp.Dependency;

namespace Magicodes.AppSession
{
    public interface IAppSessionInfo : ITransientDependency
    {
        /// <summary>
        /// APP UserId（微信OpenId或者会员Id）
        /// </summary>
        string AppUserId { get; set; }
        /// <summary>
        /// APP类型
        /// </summary>
        string AppType { get; set; }
        /// <summary>
        /// 客户端请求类型
        /// </summary>
        RequestTypes RequestType { get; set; }
    }
}
