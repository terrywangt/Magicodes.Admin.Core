import {
    Component,
    Injector,
    OnInit,
    ViewChild,
    ViewEncapsulation
} from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { ImpersonationService } from '@app/admin/users/impersonation.service';
import { CommonLookupModalComponent } from '@app/shared/common/lookup/common-lookup-modal.component';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { AppComponentBase } from '@shared/common/app-component-base';
import {
    EntityDtoOfInt64,
    FindUsersInput,
    NameValueDto,
    SwitchEntityInputDtoOfInt64,
    CommonServiceProxy
} from '@shared/service-proxies/service-proxies';
import * as moment from 'moment';
import { LazyLoadEvent } from 'primeng/components/common/lazyloadevent';
import { Paginator } from 'primeng/components/paginator/paginator';
import { Table } from 'primeng/components/table/table';
import { FileDownloadService } from '@shared/utils/file-download.service';
import { InputSwitchModule } from 'primeng/inputswitch';

import { TransactionLogServiceProxy } from '@shared/service-proxies/service-proxies';
import { CreateOrEditTransactionLogModalComponent } from './create-or-edit-transactionLog-modal.component';
import { StylingFlags } from '../../../../node_modules/@angular/core/src/render3/styling';
@Component({
    templateUrl: './transactionLog.component.html',
    encapsulation: ViewEncapsulation.None,
    animations: [appModuleAnimation()]
})
export class TransactionLogsComponent extends AppComponentBase
    implements OnInit {
    @ViewChild('dataTable')
    dataTable: Table;
    @ViewChild('paginator')
    paginator: Paginator;
    @ViewChild('createOrEditTransactionLogModal')
    createOrEditTransactionLogModal: CreateOrEditTransactionLogModalComponent;
    subscriptionDateRange: Date[] = [
        moment()
            .startOf('day')
            .toDate(),
        moment()
            .add(30, 'days')
            .endOf('day')
            .toDate()
    ];
    creationDateRange: Date[] = [
        moment()
            .startOf('day')
            .toDate(),
        moment()
            .endOf('day')
            .toDate()
    ];

    advancedFiltersAreShown = false;
    filters: {
        //是否仅获取回收站数据
        isOnlyGetRecycleData: boolean;
        filterText: string;
        creationDateRangeActive: boolean;
        creationDateStart: moment.Moment;
        creationDateEnd: moment.Moment;
        isFreeze: string;
    } = <any>{};

    constructor(
        injector: Injector,
        private _activatedRoute: ActivatedRoute,
        private _fileDownloadService: FileDownloadService,
        private _commonService: CommonServiceProxy,
        private _transactionLogService: TransactionLogServiceProxy
    ) {
        super(injector);
        this.setFiltersFromRoute();
    }

    setFiltersFromRoute(): void {
        if (
            this._activatedRoute.snapshot.queryParams['creationDateStart'] !=
            null
        ) {
            this.filters.creationDateRangeActive = true;
            this.creationDateRange[0] = moment(
                this._activatedRoute.snapshot.queryParams['creationDateStart']
            ).toDate();
        } else {
            this.creationDateRange[0] = moment()
                .add(-7, 'days')
                .startOf('day')
                .toDate();
        }

        if (
            this._activatedRoute.snapshot.queryParams['creationDateEnd'] != null
        ) {
            this.filters.creationDateRangeActive = true;
            this.creationDateRange[1] = moment(
                this._activatedRoute.snapshot.queryParams['creationDateEnd']
            ).toDate();
        } else {
            this.creationDateRange[1] = moment()
                .endOf('day')
                .toDate();
        }
    }

    ngOnInit(): void {
        this.filters.filterText =
            this._activatedRoute.snapshot.queryParams['filterText'] || '';
    }

    getTransactionLogs(event?: LazyLoadEvent) {
        if (this.primengTableHelper.shouldResetPaging(event)) {
            this.paginator.changePage(0);
            return;
        }
        this.primengTableHelper.showLoadingIndicator();

        this._transactionLogService
            .getTransactionLogs(
                this.filters.isOnlyGetRecycleData
                    ? this.filters.isOnlyGetRecycleData
                    : false,
                this.filters.creationDateRangeActive
                    ? moment(this.creationDateRange[0])
                    : undefined,
                this.filters.creationDateRangeActive
                    ? moment(this.creationDateRange[1])
                    : undefined,
                this.filters.filterText,
                this.primengTableHelper.getSorting(this.dataTable),
                this.primengTableHelper.getMaxResultCount(
                    this.paginator,
                    event
                ),
                this.primengTableHelper.getSkipCount(this.paginator, event)
            )
            .subscribe(result => {
                this.primengTableHelper.totalRecordsCount = result.totalCount;
                this.primengTableHelper.records = result.items;
                this.primengTableHelper.hideLoadingIndicator();
            });
    }

    //获取回收站数据
    getRecycleData(): void {
        this.filters.isOnlyGetRecycleData = !this.filters.isOnlyGetRecycleData;
        this.getTransactionLogs();
    }
    createTransactionLog(): void {
        this.createOrEditTransactionLogModal.show();
    }

    editTransactionLog(id: number): void {
        this.createOrEditTransactionLogModal.show(id);
    }

    deleteTransactionLog(id: number): void {
        this.message.confirm(
            this.l('AreYouSure'),
            this.l('DeleteWarningMessage'),
            isConfirmed => {
                if (isConfirmed) {
                    this._transactionLogService
                        .deleteTransactionLog(id)
                        .subscribe(() => {
                            this.reloadPage();
                            this.notify.success(this.l('SuccessfullyDeleted'));
                        });
                }
            }
        );
    }

    reloadPage(): void {
        this.paginator.changePage(this.paginator.getPage());
    }

    exportToExcel(): void {
        this._transactionLogService
            .getTransactionLogsToExcel(
                this.filters.isOnlyGetRecycleData,
                this.filters.creationDateRangeActive
                    ? moment(this.creationDateRange[0])
                    : undefined,
                this.filters.creationDateRangeActive
                    ? moment(this.creationDateRange[1])
                    : undefined,
                this.filters.filterText,
                undefined,
                1000,
                0
            )
            .subscribe(result => {
                this._fileDownloadService.downloadTempFile(result);
            });
    }

    handleIsFreezeSwitch(event, id: number) {
        const input = new SwitchEntityInputDtoOfInt64();
        input.id = id;
        input.switchValue = event.checked;
        this._transactionLogService
            .updateIsFreezeSwitchAsync(input)
            .subscribe(result => {
                this.notify.success(this.l('SuccessfulOperation'));
            });
    }

    getPayChannelText(value: number) {
        return this.l(PayChannelEnum[value]);
    }
    getTerminalText(value: number) {
        return this.l(TerminalEnum[value]);
    }
    getTransactionStateText(value: number) {
        return this.l(TransactionStateEnum[value]);
    }
}

//定义列表枚举字段，以便友好化展示
enum PayChannelEnum {
    WeChatPay = 0,
    AliPay = 1
}
enum TerminalEnum {
    Unknown = 0,
    Android = 1,
    Iphone = 2,
    Ipad = 3,
    MacOS = 4,
    WindowsXP = 5,
    WindowsVista = 6,
    Windows7 = 7,
    Windows8 = 8,
    Windows10 = 9
}
enum TransactionStateEnum {
    NotPay = 0,
    Success = 1,
    PayError = 2,
    PendingRefund = 3,
    Refunded = 4
}
