import { BsDatepickerConfig, BsDaterangepickerConfig, BsLocaleService } from 'ngx-bootstrap/datepicker';
import { NgxBootstrapLocaleMappingService } from 'assets/ngx-bootstrap/ngx-bootstrap-locale-mapping.service';
import { defineLocale } from 'ngx-bootstrap/chronos';

export class NgxBootstrapDatePickerConfigService {

    static getDaterangepickerConfig(): BsDaterangepickerConfig {
        return Object.assign(new BsDaterangepickerConfig(), {
            containerClass: 'theme-' + abp.setting.get('App.UiManagement.Theme')
        });
    }

    static getDatepickerConfig(): BsDatepickerConfig {
        return Object.assign(new BsDatepickerConfig(), {
            containerClass: 'theme-' + abp.setting.get('App.UiManagement.Theme')
        });
    }

    static getDatepickerLocale(): BsLocaleService {
        let localeService = new BsLocaleService();
        localeService.use(abp.localization.currentLanguage.name);
        return localeService;
    }

    static registerNgxBootstrapDatePickerLocales() {
        if (abp.localization.currentLanguage.name === 'en') {
            return;
        }

        let supportedLocale = new NgxBootstrapLocaleMappingService().map(abp.localization.currentLanguage.name).toLowerCase();

        import(`ngx-bootstrap/chronos/i18n/${supportedLocale}.js`)
            .then(module => {
                defineLocale(abp.localization.currentLanguage.name, module[`${abp.localization.currentLanguage.name}Locale`]);
            });
    }
}
