import { AbpSessionService } from '@abp/session/abp-session.service';
import { Component, Injector, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { accountModuleAnimation } from '@shared/animations/routerTransition';
import { AppComponentBase } from '@shared/common/app-component-base';
import {
    SessionServiceProxy,
    UpdateUserSignInTokenOutput,
    AccountServiceProxy
} from '@shared/service-proxies/service-proxies';
import { UrlHelper } from 'shared/helpers/UrlHelper';
import { ExternalLoginProvider, LoginService } from './login.service';

@Component({
    templateUrl: './login.component.html',
    animations: [accountModuleAnimation()]
})
export class LoginComponent extends AppComponentBase implements OnInit {
    submitting = false;
    isMultiTenancyEnabled: boolean = this.multiTenancy.isEnabled;
    tenantId: string;
    isEnableTenantLogin = true;

    constructor(
        injector: Injector,
        public loginService: LoginService,
        private _router: Router,
        private _sessionService: AbpSessionService,
        private _sessionAppService: SessionServiceProxy,
        private _accountService: AccountServiceProxy
    ) {
        super(injector);
        this.getEnableTenantLogin();
    }

    get multiTenancySideIsTeanant(): boolean {
        return this._sessionService.tenantId > 0;
    }

    get isTenantSelfRegistrationAllowed(): boolean {
        return this.setting.getBoolean(
            'App.TenantManagement.AllowSelfRegistration'
        );
    }

    get isSelfRegistrationAllowed(): boolean {
        if (!this._sessionService.tenantId) {
            return false;
        }

        return this.setting.getBoolean(
            'App.UserManagement.AllowSelfRegistration'
        );
    }

    ngOnInit(): void {
        if (
            this._sessionService.userId > 0 &&
            UrlHelper.getReturnUrl() &&
            UrlHelper.getSingleSignIn()
        ) {
            this._sessionAppService
                .updateUserSignInToken()
                .subscribe((result: UpdateUserSignInTokenOutput) => {
                    const initialReturnUrl = UrlHelper.getReturnUrl();
                    const returnUrl =
                        initialReturnUrl +
                        (initialReturnUrl.indexOf('?') >= 0 ? '&' : '?') +
                        'accessToken=' +
                        result.signInToken +
                        '&userId=' +
                        result.encodedUserId +
                        '&tenantId=' +
                        result.encodedTenantId;

                    location.href = returnUrl;
                });
        }

        let state = UrlHelper.getQueryParametersUsingHash().state;
        if (state && state.indexOf('openIdConnect') >= 0) {
            this.loginService.openIdConnectLoginCallback({});
        }
    }

    login(): void {
        this.submitting = true;
        this.loginService.authenticate(
            () => (this.submitting = false),
            null,
            this.tenantId
        );
    }

    externalLogin(provider: ExternalLoginProvider) {
        console.log(provider);
        this.loginService.externalAuthenticate(provider);
    }

    getCacheTenantInfo(userName: string) {
        if (this.isEnableTenantLogin) {
            return;
        }
        if (userName.length > 0) {
            this._accountService
                .getTenantBasisInfoByCache(userName)
                .subscribe(result => {
                    abp.multiTenancy.setTenantIdCookie(result.id);
                });
        }
    }

    getEnableTenantLogin() {
        this._accountService.getIfEnableTenantLogin().subscribe(result => {
            console.log(result);
            this.isEnableTenantLogin = result;
        });
    }
}
