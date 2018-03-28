import { Component, OnInit, Injector } from '@angular/core';
import { InstallServiceProxy, EmailSettingsEditDto, InstallDto, NameValue, HostBillingSettingsEditDto, CommonLookupServiceProxy } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import { AppSessionService } from '@shared/common/session/app-session.service';
import { appModuleAnimation } from '@shared/animations/routerTransition';

@Component({
    templateUrl: './install.component.html',
    animations: [appModuleAnimation()]
})
export class InstallComponent extends AppComponentBase implements OnInit {

    saving = false;
    setupSettings: InstallDto;
    languages: NameValue[];

    constructor(
        injector: Injector,
        private _installSettingService: InstallServiceProxy,
        private _commonLookupService: CommonLookupServiceProxy,
        private _appSessionService: AppSessionService
    ) {
        super(injector);
    }

    loadAppSettingsJson(): void {
        let self = this;
        self._installSettingService.getAppSettingsJson()
            .subscribe(result => {
                this.setupSettings.webSiteUrl = result.webSiteUrl;
                this.setupSettings.serverUrl = result.serverSiteUrl;
                this.languages = result.languages;
            });
    }

    init(): void {
        this._installSettingService.checkDatabase()
            .subscribe(result => {
                if (result.isDatabaseExist) {
                    window.location.href = '/';
                }
            });

        this.setupSettings = new InstallDto();
        this.setupSettings.smtpSettings = new EmailSettingsEditDto();
        this.setupSettings.billInfo = new HostBillingSettingsEditDto();
        this.setupSettings.defaultLanguage = 'en';
        this.loadAppSettingsJson();
    }

    ngOnInit(): void {
        let self = this;
        self.init();
    }

    saveAll(): void {
        this.saving = true;
        this._installSettingService.setup(this.setupSettings)
            .finally(() => {
                this.saving = false;
            })
            .subscribe(() => {
                window.location.href = '/';
            });
    }
}
