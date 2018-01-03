import { Component, Injector, OnInit } from '@angular/core';
import { Router, CanActivate } from '@angular/router';
import { TokenAuthServiceProxy, SendTwoFactorAuthCodeModel } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import { LoginService } from './login.service';
import { accountModuleAnimation } from '@shared/animations/routerTransition';

@Component({
    templateUrl: './send-two-factor-code.component.html',
    animations: [accountModuleAnimation()]
})
export class SendTwoFactorCodeComponent extends AppComponentBase implements CanActivate, OnInit {

    selectedTwoFactorProvider: string;
    submitting = false;

    constructor(
        injector: Injector,
        public loginService: LoginService,
        private _tokenAuthService: TokenAuthServiceProxy,
        private _router: Router
    ) {
        super(injector);
    }

    canActivate(): boolean {
        if (this.loginService.authenticateModel &&
            this.loginService.authenticateResult &&
            this.loginService.authenticateResult.twoFactorAuthProviders &&
            this.loginService.authenticateResult.twoFactorAuthProviders.length
            ) {
            return true;
        }

        return false;
    }

    ngOnInit(): void {
        this.selectedTwoFactorProvider = this.loginService.authenticateResult.twoFactorAuthProviders[0];
    }

    submit(): void {
        const model = new SendTwoFactorAuthCodeModel();
        model.userId = this.loginService.authenticateResult.userId;
        model.provider = this.selectedTwoFactorProvider;

        this.submitting = true;
        this._tokenAuthService
            .sendTwoFactorAuthCode(model)
            .finally(() => this.submitting = false)
            .subscribe(() => {
                this._router.navigate(['account/verify-code']);
            });
    }
}
