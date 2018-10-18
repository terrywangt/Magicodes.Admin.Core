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

            if (isRtl) {
                styleLoaderService.load(
                    AppConsts.appBaseUrl + '/assets/common/styles/metronic-customize-top-menu-rtl.css'
                );
            }
        }

        if (isRtl) {
            styleLoaderService.load(
                AppConsts.appBaseUrl + '/assets/common/styles/abp-zero-template-rtl.css'
            );
        }

        return Promise.resolve(true);
    }
}
