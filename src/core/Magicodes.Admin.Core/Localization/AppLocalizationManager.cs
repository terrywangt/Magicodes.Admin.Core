using System.Globalization;
using Abp.Dependency;
using Abp.Localization;
using Abp.Localization.Sources;

namespace Magicodes.Admin.Localization
{
    /// <summary>
    /// APP本地化管理器
    /// </summary>
    public class AppLocalizationManager : IAppLocalizationManager, ITransientDependency
    {
        public AppLocalizationManager()
        {
            LocalizationManager = NullLocalizationManager.Instance;
        }
        

        private ILocalizationSource _localizationSource;
        public ILocalizationManager LocalizationManager { get; set; }

        protected ILocalizationSource LocalizationSource
        {
            get
            {
                if (_localizationSource == null || _localizationSource.Name != AdminConsts.AppLocalizationSourceName)
                {
                    _localizationSource = LocalizationManager.GetSource(AdminConsts.AppLocalizationSourceName);
                }

                return _localizationSource;
            }
        }

        /// <summary>
        /// 获取本地化字符串
        /// </summary>
        /// <param name="name">Key</param>
        /// <returns>本地化字符串</returns>
        public virtual string L(string name)
        {
            return LocalizationSource.GetString(name);
        }

        /// <summary>
        /// 获取本地化字符串
        /// </summary>
        /// <param name="name">Key</param>
        /// <param name="args">参数</param>
        /// <returns>本地化字符串</returns>
        public string L(string name, params object[] args)
        {
            return LocalizationSource.GetString(name, args);
        }

        /// <summary>
        /// 获取本地化字符串
        /// </summary>
        /// <param name="name">Key</param>
        /// <param name="culture">语言</param>
        /// <returns>本地化字符串</returns>
        public string L(string name, CultureInfo culture)
        {
            return LocalizationSource.GetString(name, culture);
        }

        /// <summary>
        /// 获取本地化字符串
        /// </summary>
        /// <param name="name">Key</param>
        /// <param name="culture">语言</param>
        /// <param name="args"></param>
        /// <returns>本地化字符串</returns>
        public string L(string name, CultureInfo culture, params object[] args)
        {
            return LocalizationSource.GetString(name, culture, args);
        }
    }
}