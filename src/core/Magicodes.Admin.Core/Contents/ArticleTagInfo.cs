using System.ComponentModel.DataAnnotations;

namespace Magicodes.Admin.Contents
{
    /// <summary>
    /// 文章标签
    /// </summary>
    public class ArticleTagInfo : EntityBase<long>
    {
        /// <summary>
        /// 文章信息
        /// </summary>
        public virtual ArticleInfo ArticleInfo { get; set; }

        /// <summary>
        /// 文章Id
        /// </summary>
        public long ArticleInfoId { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        [Display(Name = "名称")]
        [MaxLength(50)]
        [Required]
        public string Name { get; set; }
    }
}