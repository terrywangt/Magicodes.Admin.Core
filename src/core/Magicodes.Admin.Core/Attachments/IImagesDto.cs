namespace Magicodes.Admin.Attachments
{
    /// <summary>
    /// 图片Dto定义
    /// </summary>
    public interface IImagesDto
    {
        /// <summary>
        /// 文件大小
        /// </summary>
        long FileLength { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// 链接
        /// </summary>
        string Url { get; set; }
    }
}