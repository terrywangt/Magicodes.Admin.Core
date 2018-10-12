import { NgModule } from '@angular/core';
import { NavigationEnd, Router, RouterModule, Routes } from '@angular/router';
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
                    this.setAppModuleBodyClassInternal();
                } else {
                    document.body.className = this._uiCustomizationService.getAccountModuleBodyClass();
                }
            }

            if (url.indexOf('/account/') >= 0) {
                document.body.className = this._uiCustomizationService.getAccountModuleBodyClass();
            } else {
                this.setAppModuleBodyClassInternal();
            }
        }
    }

    setAppModuleBodyClassInternal(): void {
        let currentBodyClass = document.body.className;
        let classesToRemember = '';

        if (currentBodyClass.indexOf('m-brand--minimize') >= 0) {
            classesToRemember += ' m-brand--minimize ';
        }

        if (currentBodyClass.indexOf('m-aside-left--minimize') >= 0) {
            classesToRemember += ' m-aside-left--minimize';
        }

        if (currentBodyClass.indexOf('m-brand--hide') >= 0) {
            classesToRemember += ' m-brand--hide';
        }

        if (currentBodyClass.indexOf('m-aside-left--hide') >= 0) {
            classesToRemember += ' m-aside-left--hide';
        }

        if (currentBodyClass.indexOf('swal2-toast-shown') >= 0) {
            classesToRemember += ' swal2-toast-shown';
        }

        document.body.className = this._uiCustomizationService.getAppModuleBodyClass() + ' ' + classesToRemember;
    }

    getSetting(key: string): string {
        return abp.setting.get(key);
    }
}
