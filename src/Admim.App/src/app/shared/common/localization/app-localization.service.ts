import { Injectable } from '@angular/core';
import { LocalizationService } from '@abp/localization/localization.service';
import { AppConsts } from '@shared/AppConsts';

@Injectable()
export class AppLocalizationService extends LocalizationService {

    l(key: string, ...args: any[]): string {
        return this.ls(AppConsts.localization.defaultLocalizationSourceName, key);
    }

    ls(sourcename: string, key: string, ...args: any[]): string {
        let localizedText = this.localize(key, sourcename);

        if (!localizedText) {
            localizedText = key;
        }

        if (!args || !args.length) {
            return localizedText;
        }

        args.unshift(localizedText);
        return abp.utils.formatString.apply(this, args);
    }
}
