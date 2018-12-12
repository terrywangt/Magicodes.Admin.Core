using System.ComponentModel.DataAnnotations;
using Abp.Application.Services.Dto;
using Abp.AutoMapper;

namespace Magicodes.Admin.Contents.Dto
{
    /// <summary>
    ///  文章来源编辑Dto
    /// </summary>
    [AutoMapFrom(typeof(ArticleSourceInfo))]
    public class ArticleSourceInfoEditDto : EntityDto<long?>
    {
		/// <summary>
		/// 名称
		/// </summary>
		[Required][MaxLength(30)]
        public string Name { get; set; }
    }
}