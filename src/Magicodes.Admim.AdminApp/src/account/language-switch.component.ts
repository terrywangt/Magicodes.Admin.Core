import { Component, Injector, OnInit } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import * as _ from 'lodash';

@Component({
    selector: 'language-switch',
    templateUrl: './language-switch.component.html'
})
export class LanguageSwitchComponent extends AppComponentBase implements OnInit {

    currentLanguage: abp.localization.ILanguageInfo;
    languages: abp.localization.ILanguageInfo[] = [];

    constructor(injector: Injector) {
        super(injector);
    }

    ngOnInit(): void {
        this.languages = _.filter(abp.localization.languages, l => (<any>l).isDisabled === false);
        this.currentLanguage = abp.localization.currentLanguage;
    }

    changeLanguage(language: abp.localization.ILanguageInfo) {
        abp.utils.setCookieValue(
            'Abp.Localization.CultureName',
            language.name,
            new Date(new Date().getTime() + 5 * 365 * 86400000), // 5 year
            abp.appPath
        );

        location.reload();
    }
}
