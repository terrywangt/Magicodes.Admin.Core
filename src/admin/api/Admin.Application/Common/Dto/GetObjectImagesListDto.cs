using Abp.AutoMapper;
using Magicodes.Admin.Attachments;

namespace Magicodes.Admin.Common.Dto
{
    /// <summary>
    /// 图片显示Dto
    /// </summary>
    [AutoMapFrom(typeof(AttachmentInfo))]
    public class GetObjectImagesListDto : IImagesDto
    {
        public long Id { get; set; }
        /// <summary>
        ///名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 文件大小
        /// </summary>
        public long FileLength { get; set; }

        /// <summary>
        /// 网络路径
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// 是否封面
        /// </summary>
        public bool IsCover { get; set; }
    }
}
