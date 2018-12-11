using System;
using Abp.AutoMapper;
using Abp.Domain.Entities.Auditing;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Magicodes.Admin.Dto;
using Abp.Runtime.Validation;
using Abp.Extensions;

namespace Magicodes.App.Application.Contents.Dto
{
    /// <summary>
    /// 文章列表接口 输入参数
    /// </summary>
    public class GetArticleListInput : PagedAndSortedInputDto, IShouldNormalize    
    {

		/// <summary>
		/// 栏目Id
		/// </summary>
		public long? ColumnInfoId { get; set; }

		/// <summary>
		/// 栏目编码
		/// </summary>
		public string ColumnInfoCode { get; set; }

		/// <summary>
		/// 关键字(模糊查询用)
		/// </summary>
		public string KeyWord { get; set; }

		/// <summary>
		/// 推荐类型
		/// </summary>
		public long? ArticleSourceId { get; set; }

		/// <summary>
		/// 栏目类型
		/// </summary>
		public RecommendedTypesEnum RecommendedTypes { get; set; }

		public void Normalize()
		{
		    Sorting = Sorting.IsNullOrWhiteSpace() ? "ReleaseTime": Sorting;
		}


        public enum RecommendedTypesEnum
        {
            /// <summary>
            /// 所有
            /// </summary>
            All = 0, 

            /// <summary>
            /// 置顶
            /// </summary>
            Top = 1, 

            /// <summary>
            /// 热门
            /// </summary>
            Hot = 2, 

            /// <summary>
            /// 推荐
            /// </summary>
            Recommend = 3, 

            /// <summary>
            /// 普通
            /// </summary>
            Common = 4, 

        }

    }
}