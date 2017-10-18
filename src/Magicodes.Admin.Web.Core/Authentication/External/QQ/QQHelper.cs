using System;
using Newtonsoft.Json.Linq;

namespace Magicodes.Admin.Web.Authentication.External.QQ
{
    public static class QQHelper
    {
        /// <summary>
        ///     获取Id
        /// </summary>
        public static string GetId(JObject user)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            return user.Value<string>("id");
        }

        /// <summary>
        ///     获取用户昵称
        /// </summary>
        public static string GetNickName(JObject user)
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
            return user.Value<string>("gender");
        }

        /// <summary>
        /// 获取用户头像
        /// </summary>
        public static string GetFigureUrl(JObject user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            return user.Value<string>("figureurl");
        }

        /// <summary>
        /// 获取用户头像（中）
        /// </summary>
        public static string GetFigureUrlMedium(JObject user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            return user.Value<string>("figureurl_1");
        }

        /// <summary>
        /// 获取用户头像（全）
        /// </summary>
        public static string GetFigureUrlFull(JObject user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            return user.Value<string>("figureurl_2");
        }

        /// <summary>
        /// 获取用户avatar
        /// </summary>
        public static string GetAvatar(JObject user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            return user.Value<string>("figureurl_qq_1");
        }

        /// <summary>
        /// 获取用户avatar_full
        /// </summary>
        public static string GetAvatarFull(JObject user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            return user.Value<string>("figureurl_qq_2");
        }
    }
}