using System.ComponentModel.DataAnnotations;

namespace Magicodes.Admin.Attachments
{
    /// <summary>
    /// 附件信息
    /// </summary>
    public class AttachmentInfo: EntityBase<long>, IImagesDto
    {
        /// <summary>
        ///名称
        /// </summary>
        [MaxLength(50)]
        [Required]
        [Display(Name = "名称")]
        public string Name { get; set; }

        /// <summary>
        /// 内容类型
        /// </summary>
        [MaxLength(120)]
        [Required]
        [Display(Name = "内容类型")]
        public string ContentType { get; set; }

        /// <summary>
        /// 文件大小
        /// </summary>
        [Display(Name = "文件大小")]
        public long FileLength { get; set; }

        /// <summary>
        /// 容器名称
        /// </summary>
        [MaxLength(50)]
        public string ContainerName { get; set; }

        /// <summary>
        /// 对象名称
        /// </summary>
        [MaxLength(50)]
        public string BlobName { get; set; }

        /// <summary>
        /// 网络路径
        /// </summary>
        [MaxLength(255)]
        public string Url { get; set; }

        /// <summary>
        /// 附件类型
        /// </summary>
        [Display(Name = "附件类型")]
        public AttachmentTypes AttachmentType { get; set; }

        /// <summary>
        /// 附件/素材分类
        /// </summary>
        [Display(Name = "附件/素材分类")]
        public AttachmentSorts AttachmentSorts { get; set; }

        /// <summary>
        /// 内容MD5编码
        /// </summary>
        [MaxLength(32)]
        public string ContentMD5 { get; set; }
    }
}
