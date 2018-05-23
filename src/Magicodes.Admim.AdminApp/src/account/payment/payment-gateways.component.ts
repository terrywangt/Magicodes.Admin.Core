import { Component, Input } from '@angular/core';
import { EditionPaymentType, PaymentPeriodType } from '@shared/AppEnums';
import { EditionSelectDto } from '@shared/service-proxies/service-proxies';

@Component({
    selector: 'paymentGateways',
    templateUrl: './payment-gateways.component.html',
})
export class PaymentGatewaysComponent {
    @Input() edition: EditionSelectDto = null;
    @Input() paymentPeriodType: PaymentPeriodType = null;
    @Input() editionPaymentType: EditionPaymentType = null;
}
