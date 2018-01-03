import { Component, Injector, OnInit } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import { accountModuleAnimation } from '@shared/animations/routerTransition';

import {
    EditionSelectDto,
    PaymentServiceProxy,
    TenantRegistrationServiceProxy,
    PaymentInfoDto
} from '@shared/service-proxies/service-proxies';
import {
    PaymentPeriodType,
    SubscriptionPaymentGatewayType,
    EditionPaymentType
} from '@shared/AppEnums';
import { ActivatedRoute } from '@angular/router';

@Component({
    templateUrl: './upgrade-or-extend.component.html',
    animations: [accountModuleAnimation()]
})

export class UpgradeOrExtendComponent extends AppComponentBase implements OnInit {

    editionPaymentType: EditionPaymentType;
    edition: EditionSelectDto = new EditionSelectDto();
    tenantId: number = abp.session.tenantId;
    paymentPeriodType = PaymentPeriodType;
    subscriptionPaymentGateway = SubscriptionPaymentGatewayType;
    selectedPaymentPeriodType: PaymentPeriodType = PaymentPeriodType.Monthly;
    additionalPrice: number;

    editionPaymentTypeCheck: typeof EditionPaymentType = EditionPaymentType;

    constructor(
        injector: Injector,
        private _activatedRoute: ActivatedRoute,
        private _paymentAppService: PaymentServiceProxy,
        private _tenantRegistrationService: TenantRegistrationServiceProxy
    ) {
        super(injector);
    }

    ngOnInit(): void {
        this.editionPaymentType = parseInt(this._activatedRoute.snapshot.queryParams['editionPaymentType']);
        const upgradeEditionId = this._activatedRoute.snapshot.queryParams['upgradeEditionId'];

        this._paymentAppService.getPaymentInfo(upgradeEditionId)
            .subscribe((result: PaymentInfoDto) => {
                this.edition = result.edition;
                this.additionalPrice = Number(result.additionalPrice.toFixed(2));
            });
    }

    onPaymentPeriodChangeChange(selectedPaymentPeriodType) {
        this.selectedPaymentPeriodType = selectedPaymentPeriodType;
    }

    isUpgrade(): boolean {
        return this.additionalPrice > 0;
    }
}
