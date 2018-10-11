import { AppConsts } from '@shared/AppConsts';
import * as _ from 'lodash';
import * as rtlDetect from 'rtl-detect';

import { StyleLoaderService } from '@shared/utils/style-loader.service';

export class LocalizedResourcesHelper {

    static loadResources(callback: () => void): void {
        Promise.all([LocalizedResourcesHelper.loadLocalizedStlyes()])
            .then(() => {
                callback();
            });
    }

    static loadLocalizedStlyes(): Promise<any> {
        const isRtl = rtlDetect.isRtlLang(abp.localization.currentLanguage.name);
        let theme = abp.setting.get('App.UiManagement.Theme').toLocaleLowerCase();

        if (isRtl) {
            document.documentElement.setAttribute('dir', 'rtl');
        }

        return LocalizedResourcesHelper.loadLocalizedStylesForTheme(theme, isRtl);
    }

    static loadLocalizedStylesForTheme(theme: string, isRtl: boolean): Promise<any> {
        let cssPostfix = isRtl ? '-rtl' : '';

        let styleLoaderService = new StyleLoaderService();

        styleLoaderService.load(
            AppConsts.appBaseUrl + '/assets/metronic/dist/html/' + theme + '/assets/demo/' + theme + '/base/style.bundle' + cssPostfix.replace('-', '.') + '.css',
            AppConsts.appBaseUrl + '/assets/primeng/datatable/css/primeng.datatable' + cssPostfix + '.css',
            AppConsts.appBaseUrl + '/assets/common/styles/themes/' + theme + '/primeng.datatable' + cssPostfix + '.css',
            AppConsts.appBaseUrl + '/assets/common/styles/metronic-customize.css',
            AppConsts.appBaseUrl + '/assets/common/styles/themes/' + theme + '/metronic-customize.css',
            AppConsts.appBaseUrl + '/assets/common/styles/metronic-customize-angular.css',
            AppConsts.appBaseUrl + '/assets/common/styles/themes/' + theme + '/metronic-customize-angular.css'
        );

        if (abp.setting.get('App.UiManagement.Left.Position') === 'top') {
            styleLoaderService.load(
                AppConsts.appBaseUrl + '/assets/common/styles/metronic-customize-top-menu.css',
                AppConsts.appBaseUrl + '/assets/common/styles/themes/' + theme + '/metronic-customize-top-menu.css'
            );
        }

        if (isRtl) {
            styleLoaderService.load(
                AppConsts.appBaseUrl + '/assets/common/styles/abp-zero-template-rtl.css'
            );
        }

        return Promise.resolve(true);
    }

    private static mapCultureForBootstrapSelect(currentCulture: string): string {
        const cultureMap = {
            'en': 'en_US',
            'pt-BR': 'pt_BR',
            'fr': 'fr_FR',
            'de': 'de_DE',
            'it': 'it_IT',
            'tr': 'tr_TR',
            'ru': 'ru_RU',
            'ar': 'ar_AR'
            // Add more here
        };

        if (cultureMap[currentCulture]) {
            return cultureMap[currentCulture];
        }

        return currentCulture.replace('-', '_');
    }

    private static mapCultureForTimeago(currentCulture: string): string {
        const cultureMap = {
            'sv-SE': 'sv',
            'pt-BR': 'pt-br'
            // Add more here
        };

        if (cultureMap[currentCulture]) {
            return cultureMap[currentCulture];
        }

        return currentCulture;
    }

    private static findBootstrapSelectLocalization(currentCulture: string): string {
        const supportedCultures = ['ar_AR',
            'bg_BG',
            'cs_CZ',
            'da_DK',
            'de_DE',
            'en_US',
            'es_CL',
            'eu',
            'fa_IR',
            'fi_FI',
            'fr_FR',
            'hu_HU',
            'id_ID',
            'it_IT',
            'ko_KR',
            'nb_NO',
            'nl_NL',
            'pl_PL',
            'pt_BR',
            'pt_PT',
            'ro_RO',
            'ru_RU',
            'sk_SK',
            'sl_SL',
            'sv_SE',
            'tr_TR',
            'ua_UA',
            'zh_CN',
            'zh_TW'];

        const mappedCulture = LocalizedResourcesHelper.mapCultureForBootstrapSelect(currentCulture);
        const foundCultures = _.filter(supportedCultures, sc => sc.indexOf(mappedCulture) === 0);
        if (foundCultures && foundCultures.length > 0) {
            return foundCultures[0];
        }

        return 'en_US';
    }

    private static findTimeagoLocalization(currentCulture: string): string {
        const supportedCultures = ['af',
            'ar',
            'az',
            'bg',
            'bs',
            'ca',
            'cs',
            'cy',
            'da',
            'de',
            'dv',
            'el',
            'en',
            'es',
            'et',
            'eu',
            'fa',
            'fi',
            'fr',
            'gl',
            'he',
            'hr',
            'hu',
            'hy',
            'id',
            'is',
            'it',
            'ja',
            'jv',
            'ko',
            'ky',
            'lt',
            'lv',
            'mk',
            'nl',
            'no',
            'pl',
            'pt-br',
            'pt',
            'ro',
            'rs',
            'ru',
            'rw',
            'si',
            'sk',
            'sl',
            'sr',
            'sv',
            'th',
            'tr',
            'uk',
            'uz',
            'vi',
            'zh-CN',
            'zh-TW'];

        let mappedCulture = LocalizedResourcesHelper.mapCultureForTimeago(currentCulture);
        let foundCultures = _.filter(supportedCultures, sc => sc.indexOf(mappedCulture) === 0);
        if (foundCultures && foundCultures.length > 0) {
            return foundCultures[0];
        }

        return 'en';
    }
}
