using System;
using Abp.AutoMapper;
using Abp.Domain.Entities.Auditing;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Magicodes.App.Application.Contents.Dto
{
    /// <summary>
    /// 文章详情接口 输出参数
    /// </summary>
    public class GetArticleDetailInfoOutput
    {
        public GetArticleDetailInfoOutput()
        {
            ArticleTagInfos = new List<ArticleTagInfosDto>();
        }
        /// <summary>
        /// 文章Id
        /// </summary>
        public long ArticleInfoId { get; set; }

        /// <summary>
        /// 标题
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 内容
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// 发布人（机构）
        /// </summary>
        public string Publisher { get; set; }

        /// <summary>
        /// 栏目名称
        /// </summary>
        public string ColumnInfoTitle { get; set; }

        /// <summary>
        /// 文章来源
        /// </summary>
        public string ArticleSourceInfoName { get; set; }

        /// <summary>
        /// 推荐类型
        /// </summary>
        public RecommendedTypesEnum RecommendedTypes { get; set; }

        /// <summary>
        /// 标签
        /// </summary>
        public List<ArticleTagInfosDto> ArticleTagInfos { get; set; }

        /// <summary>
        /// 栏目名称
        /// </summary>
        public DateTime? ReleaseTime { get; set; }

        #region 子类定义
        public class ArticleTagInfosDto
        {
            /// <summary>
            /// 文章信息Id
            /// </summary>
            public long ArticleTagInfoId { get; set; }
            /// <summary>
            /// 文章信息名称
            /// </summary>
            public string ArticleTagInfoName { get; set; }
        }

        #endregion

        public enum RecommendedTypesEnum
        {
            /// <summary>
            /// 置顶
            /// </summary>
            Top = 0,
            /// <summary>
            /// 热门
            /// </summary>
            Hot = 1,

            /// <summary>
            /// 推荐
            /// </summary>
            Recommend = 2,

            /// <summary>
            /// 普通
            /// </summary>
            Common = 3,

        }

    }
}