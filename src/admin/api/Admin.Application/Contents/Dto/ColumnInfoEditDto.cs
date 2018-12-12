using System.ComponentModel.DataAnnotations;
using Abp.Application.Services.Dto;
using Abp.AutoMapper;

namespace Magicodes.Admin.Contents.Dto
{
    /// <summary>
    ///  栏目编辑Dto
    /// </summary>
    [AutoMapFrom(typeof(ColumnInfo))]
    public class ColumnInfoEditDto : EntityDto<long?>
    {
        /// <summary>
        ///     父级Id
        /// </summary>
        public long ParentId { get; set; }
        /// <summary>
        /// 标题
        /// </summary>
        [Required][MaxLength(50)]
        public string Title { get; set; }
		/// <summary>
		/// 排序号
		/// </summary>
		
        public long? SortNo { get; set; }
		/// <summary>
		/// 是否启用
		/// </summary>
		
        public bool IsActive { get; set; } = true ;
		/// <summary>
		/// 授权访问
		/// </summary>
		
        public bool IsNeedAuthorizeAccess { get; set; }
		/// <summary>
		/// 描述
		/// </summary>
		[MaxLength(500)]
        public string Description { get; set; }
		/// <summary>
		/// 简介
		/// </summary>
		[MaxLength(200)]
        public string Introduction { get; set; }
		/// <summary>
		/// 小图标
		/// </summary>
		[MaxLength(20)]
        public string IconCls { get; set; }
		/// <summary>
		/// 链接
		/// </summary>
		[MaxLength(255)]
        public string Url { get; set; }

        /// <summary>
        ///     栏目类型
        /// </summary>
        [Required]
        public ColumnTypes ColumnType { get; set; }

        /// <summary>
        ///     最大子项数量
        /// </summary>
        public int? MaxItemCount { get; set; }

        /// <summary>
        ///     是否静态
        /// </summary>
        public bool IsStatic { get; set; }
    }
}