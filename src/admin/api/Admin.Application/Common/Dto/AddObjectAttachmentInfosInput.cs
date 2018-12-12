namespace Magicodes.Admin.Common.Dto
{
    /// <summary>
    /// 更新附件绑定关系
    /// </summary>
    public class AddObjectAttachmentInfosInput
    {
        /// <summary>
        /// 对象类型
        /// </summary>
        public string ObjectType { get; set; }

        /// <summary>
        /// 对象Id
        /// </summary>
        public long ObjectId { get; set; }

        /// <summary>
        /// 附件Id
        /// </summary>
        public long[] AttachmentInfoIds { get; set; }
    }
}
