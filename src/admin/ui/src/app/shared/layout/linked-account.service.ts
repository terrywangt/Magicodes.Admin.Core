import { Injectable } from '@angular/core';
import { AppAuthService } from '@app/shared/common/auth/app-auth.service';
import { AppUrlService } from '@shared/common/nav/app-url.service';
import { AccountServiceProxy, SwitchToLinkedAccountInput, SwitchToLinkedAccountOutput } from '@shared/service-proxies/service-proxies';

@Injectable()
export class LinkedAccountService {

    constructor(
        private _accountService: AccountServiceProxy,
        private _appUrlService: AppUrlService,
        private _authService: AppAuthService
    ) {

    }

    switchToAccount(userId: number, tenantId?: number): void {

        const input = new SwitchToLinkedAccountInput();
        input.targetUserId = userId;
        input.targetTenantId = tenantId;

        this._accountService.switchToLinkedAccount(input)
            .subscribe((result: SwitchToLinkedAccountOutput) => {
                this._authService.logout(false);

                let targetUrl = this._appUrlService.getAppRootUrlOfTenant(result.tenancyName) + '?switchAccountToken=' + result.switchAccountToken;
                if (input.targetTenantId) {
                    targetUrl = targetUrl + '&tenantId=' + input.targetTenantId;
                }

                location.href = targetUrl;
            });
    }
}
