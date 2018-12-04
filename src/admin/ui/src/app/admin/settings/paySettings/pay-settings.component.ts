import {
    AfterViewChecked,
    Component,
    Injector,
    OnInit,
    ViewChild
} from '@angular/core';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { AppComponentBase } from '@shared/common/app-component-base';
import { Paginator } from 'primeng/components/paginator/paginator';
import { Table } from 'primeng/components/table/table';
import { LazyLoadEvent } from 'primeng/components/common/lazyloadevent';
import {
    PaySettingEditDto,
    PaySettingsServiceProxy,
    SplitFundSettingDto
} from '@shared/service-proxies/service-proxies';
import { CreateSplitFundInfoModalComponent } from './create-splitFundInfo-modal.component';
import { EditSplitFundInfoModalComponent } from './edit-splitFundInfo-modal.component';

@Component({
    templateUrl: './pay-settings.component.html',
    animations: [appModuleAnimation()]
})
export class PaySettingsComponent extends AppComponentBase
    implements OnInit, AfterViewChecked {
    @ViewChild('dataTable')
    dataTable: Table;
    @ViewChild('paginator')
    paginator: Paginator;
    @ViewChild('createSplitFundInfoModal')
    createSplitFundModal: CreateSplitFundInfoModalComponent;
    @ViewChild('editSplitFundInfoModal')
    editSplitFundMoal: EditSplitFundInfoModalComponent;
    paySettings: PaySettingEditDto;
    splitFunds: SplitFundSettingDto[];

    constructor(
        injector: Injector,
        private _paySettingService: PaySettingsServiceProxy
    ) {
        super(injector);
    }

    ngOnInit(): void {
        this._paySettingService.getAllSettings().subscribe(setting => {
            this.splitFunds = setting.globalAliPay.splitFundSettings;
            this.primengTableHelper.totalRecordsCount = this.splitFunds.length;
            this.primengTableHelper.records = this.splitFunds;
            this.primengTableHelper.hideLoadingIndicator();
            this.paySettings = setting;
        });
    }

    ngAfterViewChecked(): void {
        // $('tabset ul.nav').addClass('m-tabs-line');
        // $('tabset ul.nav li a.nav-link').addClass('m-tabs__link');
    }

    saveAll(): void {
        this.paySettings.globalAliPay.splitFundSettings = this.splitFunds;
        this._paySettingService
            .updateAllSettings(this.paySettings)
            .subscribe(result => {
                this.notify.info(this.l('SavedSuccessfully'));
            });
    }

    createSplitFundInfo() {
        this.createSplitFundModal.show(this.splitFunds);
    }

    reloadPage(): void {
        this.paginator.changePage(this.paginator.getPage());
    }

    createCallback(event: any) {
        if (event != null) {
            this.splitFunds.push(event);
        }
        this.getSplitFunds();
    }

    getSplitFunds(event?: LazyLoadEvent) {
        this.primengTableHelper.showLoadingIndicator();
        if (this.splitFunds.length > 0) {
            this.primengTableHelper.totalRecordsCount = this.splitFunds.length;
            this.primengTableHelper.records = this.splitFunds;
            console.log(this.primengTableHelper.records);
            this.primengTableHelper.hideLoadingIndicator();
        }
    }

    deleteSplitFund(event: any) {
        let index = this.splitFunds.indexOf(event);
        this.splitFunds.splice(index, 1);
    }

    editSplitFund(event: any) {
        this.editSplitFundMoal.show(this.splitFunds,event);
    }
}
