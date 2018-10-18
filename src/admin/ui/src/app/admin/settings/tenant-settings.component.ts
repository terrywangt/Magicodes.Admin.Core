import { IAjaxResponse } from '@abp/abpHttpInterceptor';
import { TokenService } from '@abp/auth/token.service';
import { Component, Injector, OnInit } from '@angular/core';
import { AppConsts } from '@shared/AppConsts';
import { AppTimezoneScope } from '@shared/AppEnums';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { AppComponentBase } from '@shared/common/app-component-base';
import { DefaultTimezoneScope, SendTestEmailInput, TenantSettingsEditDto, TenantSettingsServiceProxy } from '@shared/service-proxies/service-proxies';
import { FileUploader, FileUploaderOptions } from 'ng2-file-upload';
import { finalize } from 'rxjs/operators';

@Component({
    templateUrl: './tenant-settings.component.html',
    animations: [appModuleAnimation()]
})
export class TenantSettingsComponent extends AppComponentBase implements OnInit {

    usingDefaultTimeZone = false;
    initialTimeZone: string = null;
    testEmailAddress: string = undefined;

    isMultiTenancyEnabled: boolean = this.multiTenancy.isEnabled;
    showTimezoneSelection: boolean = abp.clock.provider.supportsMultipleTimezone;
    activeTabIndex: number = (abp.clock.provider.supportsMultipleTimezone) ? 0 : 1;
    loading = false;
    settings: TenantSettingsEditDto = undefined;

    logoUploader: FileUploader;
    customCssUploader: FileUploader;

    remoteServiceBaseUrl = AppConsts.remoteServiceBaseUrl;

    defaultTimezoneScope: DefaultTimezoneScope = AppTimezoneScope.Tenant;

    constructor(
        injector: Injector,
        private _tenantSettingsService: TenantSettingsServiceProxy,
        private _tokenService: TokenService
    ) {
        super(injector);
    }

    ngOnInit(): void {
        this.testEmailAddress = this.appSession.user.emailAddress;
        this.getSettings();
        this.initUploaders();
    }

    getSettings(): void {
        this.loading = true;
        this._tenantSettingsService.getAllSettings()
            .pipe(finalize(() => { this.loading = false; }))
            .subscribe((result: TenantSettingsEditDto) => {
                this.settings = result;
                if (this.settings.general) {
                    this.initialTimeZone = this.settings.general.timezone;
                    this.usingDefaultTimeZone = this.settings.general.timezoneForComparison === abp.setting.values['Abp.Timing.TimeZone'];
                }
            });
    }

    initUploaders(): void {
        this.logoUploader = this.createUploader(
            '/TenantCustomization/UploadLogo',
            result => {
                this.appSession.tenant.logoFileType = result.fileType;
                this.appSession.tenant.logoId = result.id;
            }
        );

        this.customCssUploader = this.createUploader(
            '/TenantCustomization/UploadCustomCss',
            result => {
                this.appSession.tenant.customCssId = result.id;

                let oldTenantCustomCss = document.getElementById('TenantCustomCss');
                if (oldTenantCustomCss) {
                    oldTenantCustomCss.remove();
                }

                let tenantCustomCss = document.createElement('link');
                tenantCustomCss.setAttribute('id', 'TenantCustomCss');
                tenantCustomCss.setAttribute('rel', 'stylesheet');
                tenantCustomCss.setAttribute('href', AppConsts.remoteServiceBaseUrl + '/TenantCustomization/GetCustomCss?id=' + this.appSession.tenant.customCssId);
                document.head.appendChild(tenantCustomCss);
            }
        );
    }

    createUploader(url: string, success?: (result: any) => void): FileUploader {
        const uploader = new FileUploader({ url: AppConsts.remoteServiceBaseUrl + url });

        uploader.onAfterAddingFile = (file) => {
            file.withCredentials = false;
        };

        uploader.onSuccessItem = (item, response, status) => {
            const ajaxResponse = <IAjaxResponse>JSON.parse(response);
            if (ajaxResponse.success) {
                this.notify.info(this.l('SavedSuccessfully'));
                if (success) {
                    success(ajaxResponse.result);
                }
            } else {
                this.message.error(ajaxResponse.error.message);
            }
        };

        const uploaderOptions: FileUploaderOptions = {};
        uploaderOptions.authToken = 'Bearer ' + this._tokenService.getToken();
        uploaderOptions.removeAfterUpload = true;
        uploader.setOptions(uploaderOptions);
        return uploader;
    }

    uploadLogo(): void {
        this.logoUploader.uploadAll();
    }

    uploadCustomCss(): void {
        this.customCssUploader.uploadAll();
    }

    clearLogo(): void {
        this._tenantSettingsService.clearLogo().subscribe(() => {
            this.appSession.tenant.logoFileType = null;
            this.appSession.tenant.logoId = null;
            this.notify.info(this.l('ClearedSuccessfully'));
        });
    }

    clearCustomCss(): void {
        this._tenantSettingsService.clearCustomCss().subscribe(() => {
            this.appSession.tenant.customCssId = null;

            let oldTenantCustomCss = document.getElementById('TenantCustomCss');
            if (oldTenantCustomCss) {
                oldTenantCustomCss.remove();
            }

            this.notify.info(this.l('ClearedSuccessfully'));
        });
    }

    saveAll(): void {
        this._tenantSettingsService.updateAllSettings(this.settings).subscribe(() => {
            this.notify.info(this.l('SavedSuccessfully'));

            if (abp.clock.provider.supportsMultipleTimezone && this.usingDefaultTimeZone && this.initialTimeZone !== this.settings.general.timezone) {
                this.message.info(this.l('TimeZoneSettingChangedRefreshPageNotification')).then(() => {
                    window.location.reload();
                });
            }
        });
    }

    sendTestEmail(): void {
        const input = new SendTestEmailInput();
        input.emailAddress = this.testEmailAddress;
        this._tenantSettingsService.sendTestEmail(input).subscribe(result => {
            this.notify.info(this.l('TestEmailSentSuccessfully'));
        });
    }
}
