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
    /// 获取栏目列表接口 输入参数
    /// </summary>
    public class GetColumnListInput    
    {

		/// <summary>
		/// 位置
		/// </summary>
		public PositionEnum Position { get; set; }

		/// <summary>
		/// 栏目类型
		/// </summary>
		public ColumnTypeEnum ColumnType { get; set; }


        public enum PositionEnum
        {
            /// <summary>
            /// 查询所有(默认)
            /// </summary>
            All = 0, 

            /// <summary>
            /// 首页
            /// </summary>
            Default = 1, 

        }

        public enum ColumnTypeEnum
        {
            /// <summary>
            /// 查询所有(默认)
            /// </summary>
            All = 2, 

            /// <summary>
            /// Html文本
            /// </summary>
            Html = 3, 

            /// <summary>
            /// 图片
            /// </summary>
            Image = 4, 

        }

    }
}