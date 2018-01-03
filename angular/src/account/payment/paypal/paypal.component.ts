import { Component, Input, Injector, } from '@angular/core';
import { Router } from '@angular/router';
import { AppSessionService } from '@shared/common/session/app-session.service';
import { AppComponentBase } from '@shared/common/app-component-base';

import {
    EditionSelectDto,
    CreatePaymentDto,
    ExecutePaymentDto,
    CreatePaymentDtoPaymentPeriodType,
    CreatePaymentDtoEditionPaymentType,
    ExecutePaymentDtoPaymentPeriodType,
    ExecutePaymentDtoEditionPaymentType,
    PaymentServiceProxy
} from '@shared/service-proxies/service-proxies';

import {
    PaymentPeriodType,
    SubscriptionPaymentGatewayType,
    SubscriptionStartType,
    EditionPaymentType
} from '@shared/AppEnums';

@Component({
    selector: 'paypal-component',
    templateUrl: './paypal.component.html'
})

export class PayPalComponent extends AppComponentBase {

    @Input() selectedPaymentPeriodType: PaymentPeriodType = PaymentPeriodType.Monthly;
    @Input() editionPaymentType: EditionPaymentType;

    _edition: EditionSelectDto = new EditionSelectDto();

    demoUsername = '';
    demoPassword = '';

    @Input()
    get edition() {
        return this._edition;
    }

    set edition(val: EditionSelectDto) {
        this._edition = val;
        if (val && val.id) {
            jQuery.getScript('https://www.paypalobjects.com/api/checkout.js').done(() => {
                this.paypalIsLoading = false;
                this.preparePaypalButton();

                this.demoUsername = this.getAdditionalData('DemoUsername');
                this.demoPassword = this.getAdditionalData('DemoPassword');
            });
        }
    }

    paypalIsLoading = true;
    subscriptionPaymentGateway = SubscriptionPaymentGatewayType;
    subscriptionStartType = SubscriptionStartType;

    constructor(
        injector: Injector,
        private _paymentAppService: PaymentServiceProxy,
        private _appSessionService: AppSessionService,
        private _router: Router
    ) {
        super(injector);
    }

    getAdditionalData(key: string): string {
        return this._edition.additionalData['paypal'][key];
    }

    setAdditionalData(key: string, value: string): string {
        return this._edition.additionalData['paypal'][key] = value;
    }

    preparePaypalButton(): void {

        const self = this;
        (<any>window).paypal.Button.render({
            style: { size: 'responsive' },
            env: this.getAdditionalData('Environment'),
            commit: true,
            payment() {
                const input = new CreatePaymentDto();
                input.editionId = self.edition.id;
                input.editionPaymentType = <CreatePaymentDtoEditionPaymentType>(self.editionPaymentType);
                input.subscriptionPaymentGatewayType = self.subscriptionPaymentGateway.Paypal;
                input.paymentPeriodType = <CreatePaymentDtoPaymentPeriodType>(self.selectedPaymentPeriodType);
                return self._paymentAppService
                    .createPayment(input).toPromise()
                    .then((result: any) => {
                        return result.id;
                    });
            },

            onAuthorize(data) {
                const input = new ExecutePaymentDto();

                input.gateway = self.subscriptionPaymentGateway.Paypal;
                input.paymentPeriodType = <ExecutePaymentDtoPaymentPeriodType>(self.selectedPaymentPeriodType);
                input.editionId = self.edition.id;
                input.editionPaymentType = <ExecutePaymentDtoEditionPaymentType>(self.editionPaymentType);

                self.setAdditionalData('PaymentId', data.paymentID);
                self.setAdditionalData('PayerId', data.payerID);
                input.additionalData = self._edition.additionalData.paypal;

                self._paymentAppService
                    .executePayment(input)
                    .toPromise().then((result: ExecutePaymentDto) => {
                        if (self._appSessionService.userId) {
                            self._router.navigate(['app/admin/subscription-management']);
                        } else {
                            self._router.navigate(['account/register-tenant'], {
                                queryParams: {
                                    editionId: self._edition.id,
                                    subscriptionStartType: self.subscriptionStartType.Paid,
                                    gateway: self.subscriptionPaymentGateway.Paypal,
                                    paymentId: data.paymentID
                                }
                            });
                        }
                        return;
                    });

            }
        }, '#paypal-button');

    }
}
