import { AfterViewChecked, Component, Injector, OnInit } from '@angular/core';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { AppComponentBase } from '@shared/common/app-component-base';
import { MiniProgramSettingsServiceProxy, MiniProgramSettingsEditDto } from '@shared/service-proxies/service-proxies';

@Component({
    templateUrl: './miniProgram-settings.component.html',
    animations: [appModuleAnimation()]
})
export class MiniProgramSettingsComponent extends AppComponentBase implements AfterViewChecked {
    miniProgramSettings: MiniProgramSettingsEditDto;
    constructor(
        injector: Injector,
        private _miniProgramSettingService: MiniProgramSettingsServiceProxy
    ) {
        super(injector);
    }

    ngOnInit(): void {
        this._miniProgramSettingService.getAllSettings()
            .subscribe(setting => {
                this.miniProgramSettings = setting;
            });
    }

    ngAfterViewChecked(): void {
        // $('tabset ul.nav').addClass('m-tabs-line');
        // $('tabset ul.nav li a.nav-link').addClass('m-tabs__link');
    }

    saveAll(): void {
        this._miniProgramSettingService.updateAllSettings(this.miniProgramSettings).subscribe(result => {
            this.notify.info(this.l('SavedSuccessfully'));
        });
    }
}
