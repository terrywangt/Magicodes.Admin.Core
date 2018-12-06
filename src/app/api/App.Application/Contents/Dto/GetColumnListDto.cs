using System;
using Abp.AutoMapper;
using Abp.Domain.Entities.Auditing;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Magicodes.App.Application.Contents.Contents.Dto
{
    /// <summary>
    /// 获取栏目列表接口 输出参数
    /// </summary>
    public class GetColumnListDto    
    {
        /// <summary>
        /// 栏目Id
        /// </summary>
        public long ColumnInfoId { get; set; }

        /// <summary>
        /// 标题
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 简介
        /// </summary>
        public string Introduction { get; set; }

        /// <summary>
        /// 栏目编码
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 位置
        /// </summary>
        public PositionEnum Position { get; set; }

        /// <summary>
        /// 栏目类型
        /// </summary>
        public ColumnTypeEnum ColumnType { get; set; }

        /// <summary>
        /// 小图标
        /// </summary>
        public string IconCls { get; set; }


        public enum PositionEnum
        {
            /// <summary>
            /// 首页
            /// </summary>
            Default = 0, 

        }

        public enum ColumnTypeEnum
        {
            /// <summary>
            /// Html文本
            /// </summary>
            Html = 1, 
            /// <summary>
            /// 图片
            /// </summary>
            Image = 2, 

        }

    }
}