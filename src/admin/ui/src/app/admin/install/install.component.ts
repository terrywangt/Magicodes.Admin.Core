import { Component, Injector, OnInit } from '@angular/core';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { AppComponentBase } from '@shared/common/app-component-base';
import { AppSessionService } from '@shared/common/session/app-session.service';
import { CommonLookupServiceProxy, EmailSettingsEditDto, HostBillingSettingsEditDto, InstallDto, InstallServiceProxy, NameValue } from '@shared/service-proxies/service-proxies';
import { finalize } from 'rxjs/operators';

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
            .pipe(finalize(() => { this.saving = false; }))
            .subscribe(() => {
                window.location.href = '/';
            });
    }
}
