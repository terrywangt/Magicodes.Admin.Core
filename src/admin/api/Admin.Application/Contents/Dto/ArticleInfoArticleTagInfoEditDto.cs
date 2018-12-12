using System.ComponentModel.DataAnnotations;
using Abp.Application.Services.Dto;
using Abp.AutoMapper;

namespace Magicodes.Admin.Contents.Dto
{
    /// <summary>
    ///  编辑Dto
    /// </summary>
    [AutoMapFrom(typeof(ArticleTagInfo))]
    public class ArticleTagInfoEditDto : EntityDto<long?>
    {
        public long ArticleInfoId { get; set; }
		/// <summary>
		/// 名称
		/// </summary>
		[Required][MaxLength(50)]
        public string Name { get; set; }
    }
}