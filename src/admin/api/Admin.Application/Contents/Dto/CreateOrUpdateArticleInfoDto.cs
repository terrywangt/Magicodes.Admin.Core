using System.ComponentModel.DataAnnotations;

namespace Magicodes.Admin.Contents.Dto
{
    /// <summary>
    ///  文章创建或者编辑Dto
    /// </summary>
    public partial class CreateOrUpdateArticleInfoDto
    {
        [Required]
        public ArticleInfoEditDto ArticleInfo { get; set; }
    }
}