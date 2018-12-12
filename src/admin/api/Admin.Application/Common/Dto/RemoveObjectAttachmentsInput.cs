namespace Magicodes.Admin.Common.Dto
{
    public class RemoveObjectAttachmentsInput
    {
        /// <summary>
        /// 主键Id数组
        /// </summary>
        public long[] Ids { get; set; }

        /// <summary>
        /// 附件类型
        /// </summary>
        public string ObjectType { get; set; }
    }
}
