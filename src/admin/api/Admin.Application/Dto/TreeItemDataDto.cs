namespace Magicodes.Admin.Dto
{
    /// <summary>
    /// Tree每一项数据类型定义
    /// </summary>
    public class TreeItemDataDto
    {
        /// <summary>
        /// 标题
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 主键Id
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// 图标
        /// </summary>
        public string Icon { get; set; }
    }
}