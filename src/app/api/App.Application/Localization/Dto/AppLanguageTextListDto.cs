namespace Magicodes.App.Application.Localization.Dto
{
    /// <summary>
    /// APP语言列表对象
    /// </summary>
    public class AppLanguageTextListDto
    {
        /// <summary>
        /// 语言key
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// 当前语言值（默认使用用户当前定义的语言）
        /// </summary>
        public string Value { get; set; }
    }
}
