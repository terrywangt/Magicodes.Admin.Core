import { NgModule } from '@angular/core';
import { RouterModule, Router, NavigationEnd } from '@angular/router';
import { LoginComponent } from './login/login.component';
import { RegisterComponent } from './register/register.component';
import { BuyComponent } from './payment/buy.component';
import { UpgradeOrExtendComponent } from './payment/upgrade-or-extend.component';
import { RegisterTenantComponent } from './register/register-tenant.component';
import { RegisterTenantResultComponent } from './register/register-tenant-result.component';
import { SelectEditionComponent } from './register/select-edition.component';
import { ForgotPasswordComponent } from './password/forgot-password.component';
import { ResetPasswordComponent } from './password/reset-password.component';
import { EmailActivationComponent } from './email-activation/email-activation.component';
import { ConfirmEmailComponent } from './email-activation/confirm-email.component';
import { SendTwoFactorCodeComponent } from './login/send-two-factor-code.component';
import { ValidateTwoFactorCodeComponent } from './login/validate-two-factor-code.component';
import { AccountComponent } from './account.component';
import { AppUiCustomizationService } from '@shared/common/ui/app-ui-customization.service';

@NgModule({
    imports: [
        RouterModule.forChild([
            {
                path: '',
                component: AccountComponent,
                children: [
                    { path: 'login', component: LoginComponent },
                    { path: 'register', component: RegisterComponent },
                    { path: 'buy', component: BuyComponent },
                    { path: 'extend', component: UpgradeOrExtendComponent },
                    { path: 'upgrade', component: UpgradeOrExtendComponent },
                    { path: 'register-tenant', component: RegisterTenantComponent },
                    { path: 'register-tenant-result', component: RegisterTenantResultComponent },
                    { path: 'forgot-password', component: ForgotPasswordComponent },
                    { path: 'reset-password', component: ResetPasswordComponent },
                    { path: 'email-activation', component: EmailActivationComponent },
                    { path: 'confirm-email', component: ConfirmEmailComponent },
                    { path: 'send-code', component: SendTwoFactorCodeComponent },
                    { path: 'verify-code', component: ValidateTwoFactorCodeComponent },
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
