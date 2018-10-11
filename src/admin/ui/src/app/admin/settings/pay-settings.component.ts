import { AfterViewChecked, Component, Injector, OnInit } from '@angular/core';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { AppComponentBase } from '@shared/common/app-component-base';
import { PaySettingEditDto, PaySettingsServiceProxy, } from '@shared/service-proxies/service-proxies';

@Component({
    templateUrl: './pay-settings.component.html',
    animations: [appModuleAnimation()]
})
export class PaySettingsComponent extends AppComponentBase implements OnInit, AfterViewChecked {
    paySettings: PaySettingEditDto;
    constructor(
        injector: Injector,
        private _paySettingService: PaySettingsServiceProxy
    ) {
        super(injector);
    }

    ngOnInit(): void {
        this._paySettingService.getAllSettings()
            .subscribe(setting => {
                this.paySettings = setting;
            });
    }

    ngAfterViewChecked(): void {
        // $('tabset ul.nav').addClass('m-tabs-line');
        // $('tabset ul.nav li a.nav-link').addClass('m-tabs__link');
    }

    saveAll(): void {
        this._paySettingService.updateAllSettings(this.paySettings).subscribe(result => {
            this.notify.info(this.l('SavedSuccessfully'));
        });
    }
}
