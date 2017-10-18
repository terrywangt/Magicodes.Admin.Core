using System;
using Newtonsoft.Json.Linq;

namespace Magicodes.Admin.Web.Authentication.External.Weibo
{
    public static class WeiboHelper
    {
        /// <summary>
        ///     获取用户统一标识
        /// </summary>
        public static string GetId(JObject user)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            return user.Value<string>("id");
        }

        /// <summary>
        ///     用户名称
        /// </summary>
        public static string GetName(JObject user)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            return user.Value<string>("name");
        }

        /// <summary>
        ///     用户昵称
        /// </summary>
        public static string GetNikeName(JObject user)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            return user.Value<string>("screen_name");
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
        /// 获取个人图片地址
        /// </summary>
        public static string GetProfileImageUrl(JObject user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            return user.Value<string>("profile_image_url");
        }

        /// <summary>
        /// 获取用户头像地址（大图）
        /// </summary>
        public static string GetAvatarLarge(JObject user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            return user.Value<string>("avatar_large");
        }

        /// <summary>
        /// 获取用户头像地址（高清）
        /// </summary>
        public static string GetAvatarHd(JObject user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            return user.Value<string>("avatar_hd");
        }

        /// <summary>
        /// 获取用户cover_image_phone
        /// </summary>
        public static string GetCoverImagePhone(JObject user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            return user.Value<string>("cover_image_phone");
        }

        /// <summary>
        /// 获取用户所在地
        /// </summary>
        public static string GetLocation(JObject user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            return user.Value<string>("location");
        }
    }
}