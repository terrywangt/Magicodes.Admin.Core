import { Component, Injector, OnInit, ViewChild, ViewEncapsulation } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { ImpersonationService } from '@app/admin/users/impersonation.service';
import { CommonLookupModalComponent } from '@app/shared/common/lookup/common-lookup-modal.component';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { AppComponentBase } from '@shared/common/app-component-base';
import { EntityDtoOfInt64, FindUsersInput, NameValueDto, SwitchEntityInputDtoOfInt64, CommonServiceProxy,} from '@shared/service-proxies/service-proxies';
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

export class TransactionLogsComponent extends AppComponentBase implements OnInit {
    @ViewChild('dataTable') dataTable: Table;
    @ViewChild('paginator') paginator: Paginator;
    @ViewChild('createOrEditTransactionLogModal') createOrEditTransactionLogModal: CreateOrEditTransactionLogModalComponent;

	advancedFiltersAreShown = false;
	model: {
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
		private _transactionLogService: TransactionLogServiceProxy,

    ) {
        super(injector);
        this.setFiltersFromRoute();
    }

	setFiltersFromRoute(): void {
		const self = this;
        if (self._activatedRoute.snapshot.queryParams['creationDateStart'] != null) {
            self.model.creationDateRangeActive = true;
            self.model.creationDateStart = moment(self._activatedRoute.snapshot.queryParams['creationDateStart']);
        } else {
            self.model.creationDateStart = moment().add(-7, 'days').startOf('day');
        }

        if (self._activatedRoute.snapshot.queryParams['creationDateEnd'] != null) {
            self.model.creationDateRangeActive = true;
            self.model.creationDateEnd = moment(self._activatedRoute.snapshot.queryParams['creationDateEnd']);
        } else {
            self.model.creationDateEnd = moment().endOf('day');
        }
    }

	ngOnInit(): void {
		const self = this;
        self.model.filterText = self._activatedRoute.snapshot.queryParams['filterText'] || '';
    }

	getTransactionLogs(event?: LazyLoadEvent) {
		const self = this;
        if (self.primengTableHelper.shouldResetPaging(event)) {
            self.paginator.changePage(0);
            return;
        }
        self.primengTableHelper.showLoadingIndicator();

        self._transactionLogService.getTransactionLogs(
            self.model.isOnlyGetRecycleData ? self.model.isOnlyGetRecycleData : false,
			self.model.creationDateRangeActive ? self.model.creationDateStart : undefined,
			self.model.creationDateRangeActive ? self.model.creationDateEnd : undefined,
            self.model.filterText,
            self.primengTableHelper.getSorting(self.dataTable),
            self.primengTableHelper.getMaxResultCount(self.paginator, event),
            self.primengTableHelper.getSkipCount(self.paginator, event)
        ).subscribe(result => {
            self.primengTableHelper.totalRecordsCount = result.totalCount;
            console.log(result.items);
            self.primengTableHelper.records = result.items;
            self.primengTableHelper.hideLoadingIndicator();
        });
    }

    //获取回收站数据
    getRecycleData(): void {
        this.model.isOnlyGetRecycleData = !this.model.isOnlyGetRecycleData;
        this.getTransactionLogs();
    }
	createTransactionLog(): void {
		const self = this;
        self.createOrEditTransactionLogModal.show();
    }

	editTransactionLog(id:number): void {
		const self = this;
        self.createOrEditTransactionLogModal.show(id);
    }

	deleteTransactionLog(id: number): void {
		const self = this;
		self.message.confirm(
            self.l('AreYouSure'),
            self.l('DeleteWarningMessage'),
            isConfirmed => {
                if (isConfirmed) {
                    self._transactionLogService.deleteTransactionLog(id).subscribe(() => {
                        self.reloadPage();
                        self.notify.success(self.l('SuccessfullyDeleted'));
                    });
                }
            }
        );
    }

	reloadPage(): void {
		const self = this;
        self.paginator.changePage(self.paginator.getPage());
    }

	exportToExcel(): void {
        const self = this;
        self._transactionLogService.getTransactionLogsToExcel(
            self.model.isOnlyGetRecycleData,
			self.model.creationDateRangeActive ? self.model.creationDateStart : undefined,
			self.model.creationDateRangeActive ? self.model.creationDateEnd : undefined,
            self.model.filterText,
            undefined,
            1000,
            0).subscribe(result => {
                self._fileDownloadService.downloadTempFile(result);
            });
    }

	handleIsFreezeSwitch(event, id: number){
			const self = this;
			const input = new SwitchEntityInputDtoOfInt64();
			input.id = id;
			input.switchValue = event.checked;
			self._transactionLogService.updateIsFreezeSwitchAsync(input).subscribe(result => {
                self.notify.success(self.l('SuccessfulOperation'));
			})
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
