using System.Globalization;
using System.Threading.Tasks;
using Abp;

namespace Magicodes.Admin.Localization
{
    /// <summary>
    /// APP本地化管理器
    /// </summary>
    public interface IAppLocalizationManager
    {
        /// <summary>
        /// 获取本地化字符串
        /// </summary>
        /// <param name="name">Key</param>
        /// <returns>本地化字符串</returns>
        string L(string name);

        /// <summary>
        /// 获取本地化字符串
        /// </summary>
        /// <param name="name">Key</param>
        /// <param name="args">参数</param>
        /// <returns>本地化字符串</returns>
        string L(string name, params object[] args);

        /// <summary>
        /// 获取本地化字符串
        /// </summary>
        /// <param name="name">Key</param>
        /// <param name="culture">语言</param>
        /// <returns>本地化字符串</returns>
        string L(string name, CultureInfo culture);

        /// <summary>
        /// 获取本地化字符串
        /// </summary>
        /// <param name="name">Key</param>
        /// <param name="culture">语言</param>
        /// <param name="args"></param>
        /// <returns>本地化字符串</returns>
        string L(string name, CultureInfo culture, params object[] args);
    }
}
