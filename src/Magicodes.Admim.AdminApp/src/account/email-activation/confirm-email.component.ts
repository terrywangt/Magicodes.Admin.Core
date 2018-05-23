import { Component, Injector, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { AppComponentBase } from '@shared/common/app-component-base';
import { AppSessionService } from '@shared/common/session/app-session.service';
import { AccountServiceProxy, ActivateEmailInput } from '@shared/service-proxies/service-proxies';

@Component({
    template: `<p>{{waitMessage}}</p>`
})
export class ConfirmEmailComponent extends AppComponentBase implements OnInit {

    waitMessage: string;

    model: ActivateEmailInput = new ActivateEmailInput();

    constructor(
        injector: Injector,
        private _accountService: AccountServiceProxy,
        private _router: Router,
        private _activatedRoute: ActivatedRoute,
        private _appSessionService: AppSessionService

    ) {
        super(injector);
    }

    ngOnInit(): void {
        this.waitMessage = this.l('PleaseWaitToConfirmYourEmailMessage');

        this.model.userId = this._activatedRoute.snapshot.queryParams['userId'];
        this.model.confirmationCode = this._activatedRoute.snapshot.queryParams['confirmationCode'];

        if (this._appSessionService.changeTenantIfNeeded(
            this.parseTenantId(
                this._activatedRoute.snapshot.queryParams['tenantId']
            )
        )) {
            return;
        }

        this._accountService.activateEmail(this.model)
            .subscribe(() => {
                this.notify.success(this.l('YourEmailIsConfirmedMessage'));
                this._router.navigate(['account/login']);
            });
    }


    parseTenantId(tenantIdAsStr?: string): number {
        let tenantId = !tenantIdAsStr ? undefined : parseInt(tenantIdAsStr, 10);
        if (tenantId === NaN) {
            tenantId = undefined;
        }

        return tenantId;
    }
}
