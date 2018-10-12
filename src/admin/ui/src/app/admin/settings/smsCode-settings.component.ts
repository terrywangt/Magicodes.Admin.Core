import { AfterViewChecked, Component, Injector, OnInit } from '@angular/core';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { AppComponentBase } from '@shared/common/app-component-base';
import { SmsCodeSettingEditDto, SmsCodeSettingServiceProxy, AliSmsCodeSettingEditDto } from '@shared/service-proxies/service-proxies';

@Component({
    templateUrl: './smsCode-settings.component.html',
    animations: [appModuleAnimation()]
})
export class SmsCodeSettingsComponent extends AppComponentBase implements AfterViewChecked {
    smsCodeSettings: SmsCodeSettingEditDto;

    constructor(
        injector: Injector,
        private _smsCodeSettingService: SmsCodeSettingServiceProxy
    ) {
        super(injector);
        //this.smsCodeSettings.aliSmsCodeSetting = new AliSmsCodeSettingEditDto();
    }

    ngOnInit(): void {
        this._smsCodeSettingService.getAllSettings()
            .subscribe(setting => {
                this.smsCodeSettings = setting;
            });
    }

    ngAfterViewChecked(): void {
        // $('tabset ul.nav').addClass('m-tabs-line');
        // $('tabset ul.nav li a.nav-link').addClass('m-tabs__link');
    }

    saveAll(): void {
        this._smsCodeSettingService.updateAllSettings(this.smsCodeSettings).subscribe(result => {
            this.notify.info(this.l('SavedSuccessfully'));
        });
    }
}
