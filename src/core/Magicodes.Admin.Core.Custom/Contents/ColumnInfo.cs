using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Abp.Domain.Entities;

namespace Magicodes.Admin.Core.Custom.Contents
{
    /// <summary>
    /// 栏目
    /// </summary>
    [Display(Name = "栏目")]
    public class ColumnInfo : EntityBase<long>, ISortNo, IPassivable
    {
        /// <summary>
        /// 标题
        /// </summary>
        [Display(Name = "标题")]
        [Required]
        [MaxLength(50)]
        public string Title { get; set; }

        /// <summary>
        /// 排序号
        /// </summary>
        [Display(Name = "排序号")]
        public long? SortNo { get; set; }

        /// <summary>
        /// 是否启用
        /// </summary>
        [Display(Name = "是否启用")]
        [DefaultValue(true)]
        public bool IsActive { get; set; }

        /// <summary>
        /// 授权访问
        /// </summary>
        [Display(Name = "授权访问")]
        public bool IsNeedAuthorizeAccess { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        [Display(Name = "描述")]
        [MaxLength(500)]
        [DataType(DataType.MultilineText)]
        public string Description { get; set; }

        /// <summary>
        /// 简介
        /// </summary>
        [Display(Name = "简介")]
        [MaxLength(200)]
        [DataType(DataType.MultilineText)]
        public string Introduction { get; set; }

        /// <summary>
        /// 父级Id
        /// </summary>
        [Display(Name = "父级Id")]
        public long ParentId { get; set; }

        /// <summary>
        /// 小图标
        /// </summary>
        [Display(Name = "小图标")]
        [MaxLength(20)]
        public string IconCls { get; set; }

        /// <summary>
        /// 封面
        /// </summary>
        [DataType(DataType.ImageUrl)]
        [Display(Name = "封面")]
        public long? Cover { get; set; }

        /// <summary>
        /// 链接
        /// </summary>
        [Display(Name = "链接")]
        [DataType(DataType.Url)]
        [MaxLength(255)]
        public string Url { get; set; }
    }
}
