import { Component, ElementRef, EventEmitter, Injector, Input, OnInit, Output, ViewChild } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import { ActivatedRoute } from '@angular/router';
import { UrlHelper } from '@shared/helpers/UrlHelper';
import { MessageService } from '@abp/message/message.service';
import { TokenService } from '@abp/auth/token.service';
import { LogService } from '@abp/log/log.service';
import { UtilsService } from '@abp/utils/utils.service';
import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { AppConsts } from '@shared/AppConsts';
import { AuthenticateModel, AuthenticateResultModel, ExternalAuthenticateModel, ExternalAuthenticateResultModel, ExternalLoginProviderInfoModel, TokenAuthServiceProxy } from '@shared/service-proxies/service-proxies';
import { ScriptLoaderService } from '@shared/utils/script-loader.service';
import * as _ from 'lodash';
import { finalize } from 'rxjs/operators';

import { OAuthService, AuthConfig } from 'angular-oauth2-oidc';

@Component({
    selector: 'send-wechat-code',
    template:
        `<p>{{waitMessage}}</p>`
})
export class SendWeChatCodeComponent extends AppComponentBase implements OnInit {


    @Input() selectedEdition: string = undefined;
    @Output() selectedEditionChange: EventEmitter<string> = new EventEmitter<string>();

    waitMessage: string;

    constructor(
        private _tokenAuthService: TokenAuthServiceProxy,
        private _activatedRoute:ActivatedRoute,
        private _router: Router,
        private _utilsService: UtilsService,
        private _messageService: MessageService,
        private _tokenService: TokenService,
        private _logService: LogService,
        private oauthService: OAuthService,
        injector: Injector) {
        super(injector);
    }

    ngOnInit(): void {
        this.waitMessage = this.l('PleaseWaitToConfirmYourAuthenticate');
        this.weChatLoginStatusChangeCallback()
    }

    private weChatLoginStatusChangeCallback() {
        const model = new ExternalAuthenticateModel();
        model.authProvider = 'WeChat';
        model.providerAccessCode = this._activatedRoute.snapshot.queryParams['code'];
        model.providerKey = this._activatedRoute.snapshot.queryParams['code'];
        model.singleSignIn = UrlHelper.getSingleSignIn();
        model.returnUrl = UrlHelper.getReturnUrl();

        this._tokenAuthService.weChatAuthenticate(model)
            .subscribe((result: ExternalAuthenticateResultModel) => {
                if (result.waitingForActivation) {
                    this._messageService.info('You have successfully registered. Waiting for activation!');
                    return;
                }

                this.login(result.accessToken, result.encryptedAccessToken, result.expireInSeconds, false,  result.returnUrl);
            });
    }

    private login(accessToken: string, encryptedAccessToken: string, expireInSeconds: number, rememberMe?: boolean, redirectUrl?: string): void {

        let tokenExpireDate = rememberMe ? (new Date(new Date().getTime() + 1000 * expireInSeconds)) : undefined;

        this._tokenService.setToken(
            accessToken,
            tokenExpireDate
        );

        this._utilsService.setCookieValue(
            AppConsts.authorization.encrptedAuthTokenName,
            encryptedAccessToken,
            tokenExpireDate,
            abp.appPath
        );

        if (redirectUrl) {
            location.href = redirectUrl;
        } else {
            let initialUrl = UrlHelper.initialUrl;

            if (initialUrl.indexOf('/account') > 0) {
                initialUrl = AppConsts.appBaseUrl;
            }

            location.href = initialUrl;
        }
    }

}
