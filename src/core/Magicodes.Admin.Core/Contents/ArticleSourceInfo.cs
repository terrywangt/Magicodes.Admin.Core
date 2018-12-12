using System.ComponentModel.DataAnnotations;

namespace Magicodes.Admin.Contents
{
    /// <summary>
    /// 文章来源（原创、转载）
    /// </summary>
    [Display(Name = "文章来源")]
    public class ArticleSourceInfo : EntityBase<long>
    {
        /// <summary>
        /// 名称
        /// </summary>
        [Display(Name = "名称")]
        [Required]
        [MaxLength(30)]
        public string Name { get; set; }
    }
}