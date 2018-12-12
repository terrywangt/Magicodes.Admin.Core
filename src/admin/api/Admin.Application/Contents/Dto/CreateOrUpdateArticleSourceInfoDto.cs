using System.ComponentModel.DataAnnotations;

namespace Magicodes.Admin.Contents.Dto
{
    /// <summary>
    ///  文章来源创建或者编辑Dto
    /// </summary>
    public partial class CreateOrUpdateArticleSourceInfoDto
    {
        [Required]
        public ArticleSourceInfoEditDto ArticleSourceInfo { get; set; }
    }
}