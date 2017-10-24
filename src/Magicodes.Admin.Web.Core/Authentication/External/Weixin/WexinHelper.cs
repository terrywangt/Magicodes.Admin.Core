using System;
using Newtonsoft.Json.Linq;

namespace Magicodes.Admin.Web.Authentication.External.Weixin
{
    public static class WexinHelper
    {
        /// <summary>
        ///     获取用户统一标识
        /// </summary>
        public static string GetId(JObject user)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            return user.Value<string>("unionid");
        }

        /// <summary>
        ///     用户昵称
        /// </summary>
        public static string GetNikeName(JObject user)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            return user.Value<string>("nickname");
        }

        /// <summary>
        /// 获取用户性别
        /// </summary>
        public static string GetGender(JObject user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            return user.Value<string>("sex");
        }

        /// <summary>
        /// 获取用户国家
        /// </summary>
        public static string GetProvince(JObject user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            return user.Value<string>("country");
        }

        /// <summary>
        /// 获取用户省
        /// </summary>
        public static string GetCountry(JObject user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            return user.Value<string>("province");
        }

        /// <summary>
        /// 获取用户城市
        /// </summary>
        public static string GetCity(JObject user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            return user.Value<string>("city");
        }

        /// <summary>
        /// 获取用户OpenId
        /// </summary>
        public static string GetOpenId(JObject user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            return user.Value<string>("openid");
        }

        /// <summary>
        /// 获取用户头像
        /// </summary>
        public static string GetHeadimgurl(JObject user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            return user.Value<string>("headimgurl");
        }

        /// <summary>
        /// 获取用户特权
        /// </summary>
        public static string GetPrivilege(JObject user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            return user.Value<string>("privilege");
        }
    }
}