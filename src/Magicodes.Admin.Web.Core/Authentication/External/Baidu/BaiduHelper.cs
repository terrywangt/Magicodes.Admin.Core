using System;
using Newtonsoft.Json.Linq;

namespace Magicodes.Admin.Web.Authentication.External.Baidu
{
    public static class BaiduHelper
    {
        /// <summary>
        ///     获取用户统一标识
        /// </summary>
        public static string GetId(JObject user)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            return user.Value<string>("uid");
        }

        /// <summary>
        ///     用户名称
        /// </summary>
        public static string GetName(JObject user)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            return user.Value<string>("uname");
        }

        /// <summary>
        ///     用户头像
        /// </summary>
        public static string GetPortrait(JObject user)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            return user.Value<string>("portrait");
        }
    }
}