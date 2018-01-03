import { Component, Input } from '@angular/core';
import { EditionSelectDto } from '@shared/service-proxies/service-proxies';
import { PaymentPeriodType, EditionPaymentType } from '@shared/AppEnums';

@Component({
    selector: 'paymentGateways',
    templateUrl: './payment-gateways.component.html',
})
export class PaymentGatewaysComponent {
    @Input() edition: EditionSelectDto = null;
    @Input() paymentPeriodType: PaymentPeriodType = null;
    @Input() editionPaymentType: EditionPaymentType = null;
}
