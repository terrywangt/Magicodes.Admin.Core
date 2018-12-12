using System.ComponentModel.DataAnnotations;

namespace Magicodes.Admin.Contents.Dto
{
    /// <summary>
    ///  创建或者编辑Dto
    /// </summary>
    public partial class CreateOrUpdateArticleInfoArticleTagInfoDto
    {
        [Required]
        public ArticleTagInfoEditDto ArticleTagInfo { get; set; }
    }
}