import { Component, Injector, OnInit, ComponentFactoryResolver, ViewContainerRef, ViewEncapsulation } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import { accountModuleAnimation } from '@shared/animations/routerTransition';

import {
    EditionSelectDto,
    PaymentServiceProxy,
    TenantRegistrationServiceProxy
} from '@shared/service-proxies/service-proxies';
import {
    PaymentPeriodType,
    SubscriptionPaymentGatewayType,
    EditionPaymentType
} from '@shared/AppEnums';
import { ActivatedRoute, Router } from '@angular/router';

@Component({
    templateUrl: './buy.component.html',
    animations: [accountModuleAnimation()]
})

export class BuyComponent extends AppComponentBase implements OnInit {

    editionPaymentType: EditionPaymentType;
    edition: EditionSelectDto = new EditionSelectDto();
    tenantId: number = abp.session.tenantId;
    paymentPeriodType = PaymentPeriodType;
    subscriptionPaymentGateway = SubscriptionPaymentGatewayType;
    selectedPaymentPeriodType: PaymentPeriodType = PaymentPeriodType.Monthly;

    constructor(
        injector: Injector,
        private _activatedRoute: ActivatedRoute,
        private _router: Router,
        private _paymentAppService: PaymentServiceProxy,
        private _tenantRegistrationService: TenantRegistrationServiceProxy,
        private vcRef: ViewContainerRef,
        private _componentFactoryResolver: ComponentFactoryResolver
    ) {
        super(injector);
    }

    ngOnInit(): void {
        this.editionPaymentType = parseInt(this._activatedRoute.snapshot.queryParams['editionPaymentType']);
        const editionId = this._activatedRoute.snapshot.queryParams['editionId'];

        this._tenantRegistrationService.getEdition(editionId)
            .subscribe((result: EditionSelectDto) => {
                this.edition = result;
            });
    }

    onPaymentPeriodChangeChange(selectedPaymentPeriodType) {
        this.selectedPaymentPeriodType = selectedPaymentPeriodType;
    }
}
