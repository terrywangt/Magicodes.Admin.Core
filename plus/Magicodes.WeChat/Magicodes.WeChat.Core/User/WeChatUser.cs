﻿using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp;
using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Abp.Events.Bus;
using Abp.Timing;

namespace Magicodes.WeChat.Core.User
{
    [Table("WeChatUser")]
    public class WeChatUser: Entity<string>, IHasCreationTime, IMayHaveTenant
    {
        public const int MaxNickNameLength = 50;
        public const int MaxLanguageLength = 10;

        public WeChatUser()
        {
            CreationTime = Clock.Now;
        }
        /// <summary>
        /// 用户是否订阅该公众号标识
        /// </summary>
        public bool Subscribe { get; set; }

        /// <summary>
        /// 用户的昵称
        /// </summary>
        [Required]
        [MaxLength(MaxNickNameLength)]
        public string NickName { get; set; }
        /// <summary>
        /// 用户所在城市
        /// </summary>
        public string City { get; set; }
        /// <summary>
        /// 用户所在国家
        /// </summary>
        public string Country { get; set; }
        /// <summary>
        /// 用户所在省份
        /// </summary>
        public string Province { get; set; }

        /// <summary>
        /// 用户的语言，简体中文为zh_CN
        /// </summary>
        [MaxLength(MaxLanguageLength)]
        public string Language { get; set; }
        /// <summary>
        /// 用户头像，最后一个数值代表正方形头像大小（有0、46、64、96、132数值可选，0代表640*640正方形头像），用户没有头像时该项为空。若用户更换头像，原有头像URL将失效。
        /// </summary>
        public string HeadImgUrl { get; set; }
        /// <summary>
        /// 用户关注时间，为时间戳。如果用户曾多次关注，则取最后关注时间
        /// </summary>
        public DateTime SubscribeTime { get; set; }
        /// <summary>
        /// 只有在用户将公众号绑定到微信开放平台帐号后，才会出现该字段。详见：获取用户个人信息（UnionID机制）
        /// </summary>
        public string UnionId { get; set; }
        /// <summary>
        /// 公众号运营者对粉丝的备注，公众号运营者可在微信公众平台用户管理界面对粉丝添加备注
        /// </summary>
        public string Remark { get; set; }
        /// <summary>
        /// 性别
        /// </summary>
        public int Sex { get; set; }
        /// <summary>
        /// 用户所在的分组ID
        /// </summary>
        public int? GroupId { get; set; }
        public DateTime CreationTime { get; set; }
        public int? TenantId { get; set; }
    }
}