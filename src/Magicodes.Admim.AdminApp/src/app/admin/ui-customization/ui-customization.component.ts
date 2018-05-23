import { AfterViewChecked, Component, Injector, OnInit } from '@angular/core';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { AppComponentBase } from '@shared/common/app-component-base';
import { UiCustomizationSettingsEditDto, UiCustomizationSettingsServiceProxy } from '@shared/service-proxies/service-proxies';

@Component({
    templateUrl: './ui-customization.component.html',
    animations: [appModuleAnimation()]
})
export class UiCustomizationComponent extends AppComponentBase implements AfterViewChecked, OnInit {

    settings: UiCustomizationSettingsEditDto;

    constructor(
        injector: Injector,
        private _uiCustomizationService: UiCustomizationSettingsServiceProxy
    ) {
        super(injector);
    }

    ngAfterViewChecked(): void {
        //Temporary fix for: https://github.com/valor-software/ngx-bootstrap/issues/1508
        $('tabset ul.nav').addClass('m-tabs-line');
        $('tabset ul.nav li a.nav-link').addClass('m-tabs__link');
    }

    ngOnInit(): void {
        this._uiCustomizationService.getUiManagementSettings().subscribe((settingsResult) => {
            this.settings = settingsResult;
        });
    }

    leftMenuPositionSelected(): boolean {
        return this.settings.menu.position === 'left';
    }

    updateDefaultUiManagementSettings(): void {
        this._uiCustomizationService.updateDefaultUiManagementSettings(this.settings).subscribe(() => {
            window.location.reload();
        });
    }

    updateUiManagementSettings(): void {
        this._uiCustomizationService.updateUiManagementSettings(this.settings).subscribe(() => {
            window.location.reload();
        });
    }

    useSystemDefaultSettings(): void {
        this._uiCustomizationService.useSystemDefaultSettings().subscribe(() => {
            window.location.reload();
        });
    }

    allowAsideMinimizingChange(val): void {
        if (val) {
            this.settings.menu.allowAsideHiding = false;
            this.settings.menu.defaultHiddenAside = false;
        } else {
            this.settings.menu.defaultMinimizedAside = false;
        }
    }

    allowAsideHidingChange(val): void {
        if (!val) {
            this.settings.menu.defaultHiddenAside = false;
        }
    }
}
