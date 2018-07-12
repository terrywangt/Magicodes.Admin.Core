import { NgModule } from '@angular/core';
import { NavigationEnd, Router, RouterModule } from '@angular/router';
import { AppUiCustomizationService } from '@shared/common/ui/app-ui-customization.service';
import { AccountComponent } from './account.component';
import { AccountRouteGuard } from './auth/account-route-guard';
import { ConfirmEmailComponent } from './email-activation/confirm-email.component';
import { EmailActivationComponent } from './email-activation/email-activation.component';
import { LoginComponent } from './login/login.component';
import { SendTwoFactorCodeComponent } from './login/send-two-factor-code.component';
import { ValidateTwoFactorCodeComponent } from './login/validate-two-factor-code.component';
import { ForgotPasswordComponent } from './password/forgot-password.component';
import { ResetPasswordComponent } from './password/reset-password.component';
import { BuyComponent } from './payment/buy.component';
import { UpgradeOrExtendComponent } from './payment/upgrade-or-extend.component';
import { RegisterTenantResultComponent } from './register/register-tenant-result.component';
import { RegisterTenantComponent } from './register/register-tenant.component';
import { RegisterComponent } from './register/register.component';
import { SelectEditionComponent } from './register/select-edition.component';

@NgModule({
    imports: [
        RouterModule.forChild([
            {
                path: '',
                component: AccountComponent,
                children: [
                    { path: '', redirectTo: 'login' },
                    { path: 'login', component: LoginComponent, canActivate: [AccountRouteGuard] },
                    { path: 'register', component: RegisterComponent, canActivate: [AccountRouteGuard] },
                    { path: 'register-tenant', component: RegisterTenantComponent, canActivate: [AccountRouteGuard] },
                    { path: 'register-tenant-result', component: RegisterTenantResultComponent, canActivate: [AccountRouteGuard] },
                    { path: 'forgot-password', component: ForgotPasswordComponent, canActivate: [AccountRouteGuard] },
                    { path: 'reset-password', component: ResetPasswordComponent, canActivate: [AccountRouteGuard] },
                    { path: 'email-activation', component: EmailActivationComponent, canActivate: [AccountRouteGuard] },
                    { path: 'confirm-email', component: ConfirmEmailComponent, canActivate: [AccountRouteGuard] },
                    { path: 'send-code', component: SendTwoFactorCodeComponent, canActivate: [AccountRouteGuard] },
                    { path: 'verify-code', component: ValidateTwoFactorCodeComponent, canActivate: [AccountRouteGuard] },

                    { path: 'buy', component: BuyComponent },
                    { path: 'extend', component: UpgradeOrExtendComponent },
                    { path: 'upgrade', component: UpgradeOrExtendComponent },
                    { path: 'select-edition', component: SelectEditionComponent }
                ]
            }
        ])
    ],
    exports: [
        RouterModule
    ]
})
export class AccountRoutingModule {
    constructor(
        private router: Router,
        private _uiCustomizationService: AppUiCustomizationService
    ) {
        router.events.subscribe((event: NavigationEnd) => {
            setTimeout(() => {
                //this will reinitialize metronic App, when navigated to admin module
                mApp.initialized = false;

                this.toggleBodyCssClass(event.url);
            }, 0);
        });
    }

    toggleBodyCssClass(url: string): void {
        if (!url) {
            $('body').attr('class', this._uiCustomizationService.getAccountModuleBodyClass());
            return;
        }

        if (url.indexOf('/account/') >= 0) {
            $('body').attr('class', this._uiCustomizationService.getAccountModuleBodyClass());
        } else {
            $('body').attr('class', this._uiCustomizationService.getAppModuleBodyClass());
        }
    }
}
