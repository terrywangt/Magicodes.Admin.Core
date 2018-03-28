import { Component, Injector, OnInit, ViewChild, AfterViewChecked } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import {
    SessionServiceProxy,
    UserLoginInfoDto,
    TenantLoginInfoDto,
    ApplicationInfoDto,
    PaymentServiceProxy,
    InvoiceServiceProxy,
    CreateInvoiceDto
} from '@shared/service-proxies/service-proxies';
import { SubscriptionStartType, EditionPaymentType } from '@shared/AppEnums';
import { AppSessionService } from '@shared/common/session/app-session.service';
import { LazyLoadEvent } from 'primeng/components/common/lazyloadevent';
import { ActivatedRoute } from '@angular/router';
import { DataTable } from 'primeng/components/datatable/datatable';
import { Paginator } from 'primeng/components/paginator/paginator';

@Component({
    templateUrl: './subscription-management.component.html',
    animations: [appModuleAnimation()]
})

export class SubscriptionManagementComponent extends AppComponentBase implements OnInit, AfterViewChecked {

    @ViewChild('dataTable') dataTable: DataTable;
    @ViewChild('paginator') paginator: Paginator;

    loading: boolean;
    user: UserLoginInfoDto = new UserLoginInfoDto();
    tenant: TenantLoginInfoDto = new TenantLoginInfoDto();
    application: ApplicationInfoDto = new ApplicationInfoDto();
    subscriptionStartType: typeof SubscriptionStartType = SubscriptionStartType;
    editionPaymentType: EditionPaymentType = EditionPaymentType;

    filterText = '';

    constructor(
        injector: Injector,
        private _sessionService: SessionServiceProxy,
        private _paymentServiceProxy: PaymentServiceProxy,
        private _appSessionService: AppSessionService,
        private _invoiceServiceProxy: InvoiceServiceProxy,
        private _activatedRoute: ActivatedRoute
    ) {
        super(injector);
        this.filterText = this._activatedRoute.snapshot.queryParams['filterText'] || '';
    }

    ngAfterViewChecked(): void {
        //Temporary fix for: https://github.com/valor-software/ngx-bootstrap/issues/1508
        $('tabset ul.nav').addClass('m-tabs-line');
        $('tabset ul.nav li a.nav-link').addClass('m-tabs__link');
    }

    ngOnInit(): void {
        this.getSettings();
    }

    createOrShowInvoice(paymentId: number, invoiceNo: string): void {
        if (invoiceNo) {
            window.open('/app/admin/invoice/' + paymentId, '_blank');
        } else {
            this._invoiceServiceProxy.createInvoice(new CreateInvoiceDto({ subscriptionPaymentId: paymentId })).subscribe(() => {
                this.getPaymentHistory();
                window.open('/app/admin/invoice/' + paymentId, '_blank');
            });
        }
    }

    getSettings(): void {
        this.loading = true;
        this._appSessionService.init().then(() => {
            this.loading = false;
            this.user = this._appSessionService.user;
            this.tenant = this._appSessionService.tenant;
            this.application = this._appSessionService.application;
        });
    }

    getPaymentHistory(event?: LazyLoadEvent) {
        if (this.primengDatatableHelper.shouldResetPaging(event)) {
            this.paginator.changePage(0);

            return;
        }

        this.primengDatatableHelper.showLoadingIndicator();

        this._paymentServiceProxy.getPaymentHistory(
            this.primengDatatableHelper.getSorting(this.dataTable),
            this.primengDatatableHelper.getMaxResultCount(this.paginator, event),
            this.primengDatatableHelper.getSkipCount(this.paginator, event)
        ).subscribe(result => {
            this.primengDatatableHelper.totalRecordsCount = result.totalCount;
            this.primengDatatableHelper.records = result.items;
            this.primengDatatableHelper.hideLoadingIndicator();
        });
    }
}
