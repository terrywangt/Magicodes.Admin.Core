import { PermissionCheckerService } from '@abp/auth/permission-checker.service';
import { Component, Injector, OnInit, ViewEncapsulation } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import { AppSessionService } from '@shared/common/session/app-session.service';
import { AppUiCustomizationService } from '@shared/common/ui/app-ui-customization.service';
import { AppMenu } from './app-menu';
import { AppNavigationService } from './app-navigation.service';

@Component({
    templateUrl: './top-bar-menu.component.html',
    selector: 'top-bar-menu',
    encapsulation: ViewEncapsulation.None
})
export class TopBarMenuComponent extends AppComponentBase implements OnInit {

    menu: AppMenu = null;

    constructor(
        injector: Injector,
        public permission: PermissionCheckerService,
        private _appSessionService: AppSessionService,
        private _uiCustomizationService: AppUiCustomizationService,
        private _appNavigationService: AppNavigationService) {
        super(injector);
    }

    ngOnInit() {
        this.menu = this._appNavigationService.getMenu();
    }

    showMenuItem(menuItem): boolean {
        if (menuItem.permissionName === 'Pages.Administration.Tenant.SubscriptionManagement' && this._appSessionService.tenant && !this._appSessionService.tenant.edition) {
            return false;
        }

        if (menuItem.permissionName) {
            return this.permission.isGranted(menuItem.permissionName);
        }

        if (menuItem.items && menuItem.items.length) {
            return this._appNavigationService.checkChildMenuItemPermission(menuItem);
        }

        return true;
    }
}
