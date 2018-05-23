import { Component, Injector, OnInit, ViewChild } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import { AppSessionService } from '@shared/common/session/app-session.service';
import { AccountServiceProxy } from '@shared/service-proxies/service-proxies';
import { TenantChangeModalComponent } from './tenant-change-modal.component';

@Component({
    selector: 'tenant-change',
    template:
    `<span *ngIf="isMultiTenancyEnabled">
        {{l("CurrentTenant")}}: <span *ngIf="tenancyName" title="{{name}}"><strong>{{tenancyName}}</strong></span> <span *ngIf="!tenancyName">{{l("NotSelected")}}</span> (<a href="javascript:;" (click)="showChangeModal()">{{l("Change")}}</a>)
        <tenantChangeModal #tenantChangeModal></tenantChangeModal>
    </span>`
})
export class TenantChangeComponent extends AppComponentBase implements OnInit {

    @ViewChild('tenantChangeModal') tenantChangeModal: TenantChangeModalComponent;

    tenancyName: string;
    name: string;

    constructor(
        injector: Injector,
        private _appSessionService: AppSessionService,
        private _accountService: AccountServiceProxy
        ) {
        super(injector);
    }

    ngOnInit() {
        if (this._appSessionService.tenant) {
            this.tenancyName = this._appSessionService.tenant.tenancyName;
            this.name = this._appSessionService.tenant.name;
        }
    }

    get isMultiTenancyEnabled(): boolean {
        return abp.multiTenancy.isEnabled;
    }

    showChangeModal(): void {
        this.tenantChangeModal.show(this.tenancyName);
    }
}
