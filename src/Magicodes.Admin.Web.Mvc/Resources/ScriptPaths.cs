using System.Collections.Concurrent;
using System.IO;
using System.Threading;
using Abp.Dependency;
using Abp.Extensions;
using Microsoft.AspNetCore.Hosting;

namespace Magicodes.Admin.Web.Resources
{
    public class ScriptPaths : ISingletonDependency
    {
        private readonly IHostingEnvironment _hostingEnvironment;

        private readonly ConcurrentDictionary<string, string> _scriptPaths;

        public ScriptPaths(IHostingEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
            _scriptPaths = new ConcurrentDictionary<string, string>();
        }

        #region jQuery Validation

        public string JQuery_Validation_Localization
        {
            get
            {
                return _scriptPaths.GetOrAdd("jquery-validation#" + Thread.CurrentThread.CurrentUICulture.Name, k =>
                {
                    var path = GetLocalizationFileForjQueryValidationOrNull(Thread.CurrentThread.CurrentUICulture.Name.ToLower().Replace("-", "_"))
                           ?? GetLocalizationFileForjQueryValidationOrNull(Thread.CurrentThread.CurrentUICulture.Name.Left(2).ToLower())
                           ?? @"Common/Scripts/_empty.js";

                    return "~/" + path;
                });
            }
        }

        private string GetLocalizationFileForjQueryValidationOrNull(string cultureCode)
        {
            try
            {
                var relativeFilePath = @"lib/jquery-validation/src/localization/messages_" + cultureCode + ".js";
                var physicalFilePath = GetPhysicalPath(relativeFilePath);
                if (File.Exists(physicalFilePath))
                {
                    return relativeFilePath;
                }
            }
            catch { }

            return null;
        }

        #endregion

        #region jQuery jTable

        public string JQuery_JTable_Localization
        {
            get
            {
                return _scriptPaths.GetOrAdd("jtable#" + Thread.CurrentThread.CurrentUICulture.Name, k =>
                {
                    var path = GetLocalizationFileForJTableOrNull(Thread.CurrentThread.CurrentUICulture.Name.ToLower())
                               ?? GetLocalizationFileForJTableOrNull(Thread.CurrentThread.CurrentUICulture.Name.Left(2).ToLower())
                               ?? "Common/Scripts/_empty.js";

                    return "~/" + path;
                });
            }
        }

        private string GetLocalizationFileForJTableOrNull(string cultureCode)
        {
            try
            {
                var relativeFilePath = "lib/jtable/lib/localization/jquery.jtable." + cultureCode + ".js";
                var physicalFilePath = GetPhysicalPath(relativeFilePath);
                if (File.Exists(physicalFilePath))
                {
                    return relativeFilePath;
                }
            }
            catch { }

            return null;
        }

        #endregion

        #region Bootstrap Select

        private static readonly string[] BootstrapSelectLocalizationFileCultureCodeList =
        {
            "ar_AR",
            "bg_BG",
            "cro_CRO",
            "cs_CZ",
            "da_DK",
            "de_DE",
            "en_US",
            "es_CL",
            "eu",
            "fa_IR",
            "fi_FI",
            "fr_FR",
            "hu_HU",
            "id_ID",
            "it_IT",
            "ko_KR",
            "lt_LT",
            "nb_NO",
            "nl_NL",
            "pl_PL",
            "pt_BR",
            "pt_PT",
            "ro_RO",
            "ru_RU",
            "sk_SK",
            "sl_SL",
            "sv_SE",
            "tr_TR",
            "ua_UA",
            "zh_CN",
            "zh_TW"
        };

        public string Bootstrap_Select_Localization
        {
            get
            {
                return _scriptPaths.GetOrAdd("bootstrap-select#" + Thread.CurrentThread.CurrentUICulture.Name, k =>
                {
                    var path = GetLocalizationFileForBootstrapSelect(Thread.CurrentThread.CurrentUICulture.Name.ToLower())
                               ?? GetLocalizationFileForBootstrapSelect(Thread.CurrentThread.CurrentUICulture.Name.Left(2).ToLower())
                               ?? "lib/bootstrap-select/js/i18n/defaults-en_US.js";
                    return "~/" + path;
                });
            }
        }

        private static string GetLocalizationFileForBootstrapSelect(string cultureCode)
        {
            try
            {
                cultureCode = cultureCode.Replace("-", "_");
                foreach (var localizationFile in BootstrapSelectLocalizationFileCultureCodeList)
                {
                    if (localizationFile.StartsWith(cultureCode))
                    {
                        return "lib/bootstrap-select/js/i18n/defaults-" + localizationFile + ".js";
                    }
                }
            }
            catch { }

            return null;
        }

        #endregion

        #region jQuery Timeago

        public string JQuery_Timeago_Localization
        {
            get
            {
                return _scriptPaths.GetOrAdd("jquery-timeago#" + Thread.CurrentThread.CurrentUICulture.Name, k =>
                {
                    var path = GetLocalizationFileForjQueryTimeagoOrNull(Thread.CurrentThread.CurrentUICulture.Name.ToLower().Replace("-", "_"))
                       ?? GetLocalizationFileForjQueryTimeagoOrNull(Thread.CurrentThread.CurrentUICulture.Name.Left(2).ToLower())
                       ?? "lib/jquery-timeago/locales/jquery.timeago.en.js";
                    return "~/" + path;
                });
            }
        }

        private string GetLocalizationFileForjQueryTimeagoOrNull(string cultureCode)
        {
            try
            {
                var relativeFilePath = "lib/jquery-timeago/locales/jquery.timeago." + cultureCode + ".js";
                var physicalFilePath = GetPhysicalPath(relativeFilePath);
                if (File.Exists(physicalFilePath))
                {
                    return relativeFilePath;
                }
            }
            catch { }

            return null;
        }

        #endregion

        #region Helper methods

        private string GetPhysicalPath(string relativePath)
        {
            return Path.Combine(_hostingEnvironment.WebRootPath, relativePath);
        }

        #endregion
    }
}