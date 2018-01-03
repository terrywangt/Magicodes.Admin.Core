import { Component, Injector, OnInit, ViewChild } from '@angular/core';
import { ActivatedRoute, Params } from '@angular/router';
import { AppComponentBase } from '@shared/common/app-component-base';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import {
    InvoiceDto,
    InvoiceServiceProxy
} from '@shared/service-proxies/service-proxies';

@Component({
    templateUrl: './invoice.component.html',
    styleUrls: ['./invoice.component.less'],
    animations: [appModuleAnimation()]
})

export class InvoiceComponent extends AppComponentBase implements OnInit {

    paymentId = 0;
    invoiceInfo: InvoiceDto = new InvoiceDto();

    constructor(
        injector: Injector,
        private _invoiceServiceProxy: InvoiceServiceProxy,
        private activatedRoute: ActivatedRoute
    ) {
        super(injector);
    }

    ngOnInit(): void {
        this.getAllInfo();
    }

    getAllInfo(): void {

        this.activatedRoute.params.subscribe((params: Params) => {
            this.paymentId = params['paymentId'];
        });

        this._invoiceServiceProxy.getInvoiceInfo(this.paymentId).subscribe(result => {
            this.invoiceInfo = result;
        });
    }
}
