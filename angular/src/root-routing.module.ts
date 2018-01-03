import { NgModule } from '@angular/core';
import { Routes, RouterModule, Router, NavigationEnd } from '@angular/router';
import { AppUiCustomizationService } from '@shared/common/ui/app-ui-customization.service';

const routes: Routes = [
    { path: '', redirectTo: '/app/main/dashboard', pathMatch: 'full' },
    {
        path: 'account',
        loadChildren: 'account/account.module#AccountModule', //Lazy load account module
        data: { preload: true }
    }
];

@NgModule({
    imports: [RouterModule.forRoot(routes)],
    exports: [RouterModule],
    providers: []
})
export class RootRoutingModule {
    constructor(
        private router: Router,
        private _uiCustomizationService: AppUiCustomizationService) {
        router.events.subscribe((event: NavigationEnd) => {
            setTimeout(() => {
                this.toggleBodyCssClass(event.url);
            }, 0);
        });
    }

    toggleBodyCssClass(url: string): void {
        if (url) {
            if (url === '/') {
                if (abp.session.userId > 0) {
                    $('body').attr('class', this._uiCustomizationService.getAppModuleBodyClass());
                } else {
                    $('body').attr('class', this._uiCustomizationService.getAccountModuleBodyClass());
                }
            }

            if (url.indexOf('/account/') >= 0) {
                $('body').attr('class', this._uiCustomizationService.getAccountModuleBodyClass());
            } else {
                $('body').attr('class', this._uiCustomizationService.getAppModuleBodyClass());
            }
        }
    }

    getSetting(key: string): string {
        return abp.setting.get(key);
    }
}
