import { AfterViewChecked, Component, Injector, OnInit } from '@angular/core';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { AppComponentBase } from '@shared/common/app-component-base';
import { StorageSettingEditDto, StorageSettingServiceProxy, AliStorageSettingEditDto                                                                                                                                     } from '@shared/service-proxies/service-proxies';

@Component({
    templateUrl: './storage-settings.component.html',
    animations: [appModuleAnimation()]
})
export class StorageSettingsComponent extends AppComponentBase implements AfterViewChecked {
    storageSettings: StorageSettingEditDto;
    initialStorageSettings:StorageSettingEditDto;
    test:false;
    constructor(
        injector: Injector,
        private _storageSettingService: StorageSettingServiceProxy
    ) {
        super(injector);
        //this.storageSettings.aliStorageSetting = new AliStorageSettingEditDto();
    }

    ngOnInit(): void {
        this._storageSettingService.getAllSettings()
            .subscribe(setting => {
                this.storageSettings = setting;
                this.initialStorageSettings = setting;
            });
    }

    ngAfterViewChecked(): void {
        // $('tabset ul.nav').addClass('m-tabs-line');
        // $('tabset ul.nav li a.nav-link').addClass('m-tabs__link');
    }

    saveAll(): void {
        console.log(this.storageSettings);
        this._storageSettingService.updateAllSettings(this.storageSettings).subscribe(result => {
            this.notify.info(this.l('SavedSuccessfully'));
        });
    }
}
