using Acr.UserDialogs;
using Magicodes.Admin.Localization;

namespace Magicodes.Admin.UI
{
    public static class UserDialogHelper
    {
        public static void Warn(string localizationKeyOrMessage,
            LocalizationSource localizationSource = LocalizationSource.RemoteTranslation)
        {
            UserDialogs.Instance.Alert(Localize(localizationKeyOrMessage, localizationSource), L.Localize("Warning"));
        }

        public static void Error(string localizationKeyOrMessage,
            LocalizationSource localizationSource = LocalizationSource.RemoteTranslation)
        {
            UserDialogs.Instance.Alert(Localize(localizationKeyOrMessage, localizationSource), L.Localize("Error"));
        }

        public static void Success(string localizationKeyOrMessage,
            LocalizationSource localizationSource = LocalizationSource.RemoteTranslation)
        {
            UserDialogs.Instance.Alert(Localize(localizationKeyOrMessage, localizationSource), L.Localize("Success"));
        }

        private static string Localize(string localizationKeyOrMessage,
            LocalizationSource localizationSource = LocalizationSource.RemoteTranslation)
        {
            switch (localizationSource)
            {
                case LocalizationSource.RemoteTranslation:
                    return L.Localize(localizationKeyOrMessage);
                case LocalizationSource.LocalTranslation:
                    return Localization.LocalTranslationHelper.Localize(localizationKeyOrMessage);
                case LocalizationSource.NoTranslation:
                    return localizationKeyOrMessage;
                default:
                    return localizationKeyOrMessage;
            }
        }
    }
}