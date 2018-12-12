namespace Magicodes.Admin.Common.Dto
{
    /// <summary>
    /// 获取对象图片列表
    /// </summary>
    public class GetObjectImagesInput
    {
        /// <summary>
        /// 对象类型
        /// </summary>
        public string ObjectType { get; set; }

        /// <summary>
        /// 对象Id
        /// </summary>
        public long ObjectId { get; set; }
    }
}
