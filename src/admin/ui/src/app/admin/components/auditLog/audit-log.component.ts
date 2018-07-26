import { NotifyService } from '@abp/notify/notify.service';
import { Component, Injector, ViewChild, ViewEncapsulation, Input } from '@angular/core';
import { AuditLogDetailModalComponent } from '@app/admin/audit-logs/audit-log-detail-modal.component';
import { EntityChangeDetailModalComponent } from '@app/admin/audit-logs/entity-change-detail-modal.component';
import { AppComponentBase } from '@shared/common/app-component-base';
import { AuditLogListDto, AuditLogServiceProxy, EntityChangeListDto, NameValueDto } from '@shared/service-proxies/service-proxies';
import { FileDownloadService } from '@shared/utils/file-download.service';
import * as moment from 'moment';
import { LazyLoadEvent } from 'primeng/components/common/lazyloadevent';
import { Paginator } from 'primeng/components/paginator/paginator';
import { Table } from 'primeng/components/table/table';
import { PrimengTableHelper } from 'shared/helpers/PrimengTableHelper';
import { ModalDirective } from 'ngx-bootstrap';

@Component({
    selector: 'audit-log',
    templateUrl: './audit-log.component.html',
    styleUrls: ['./audit-log.component.less'],
    encapsulation: ViewEncapsulation.None
})
export class AuditLogComponent extends AppComponentBase {

    @ViewChild('auditLogDetailModal') auditLogDetailModal: AuditLogDetailModalComponent;
    @ViewChild('entityChangeDetailModal') entityChangeDetailModal: EntityChangeDetailModalComponent;
    @ViewChild('dataTableAuditLogs') dataTableAuditLogs: Table;
    @ViewChild('dataTableEntityChanges') dataTableEntityChanges: Table;
    @ViewChild('paginatorAuditLogs') paginatorAuditLogs: Paginator;
    @ViewChild('paginatorEntityChanges') paginatorEntityChanges: Paginator;

    @ViewChild('auditLogModal') modal: ModalDirective;
    @Input() public serviceName: string;
    @Input('objectName') public entityTypeFullName: string;
    //Filters
    public startDate: moment.Moment = moment().startOf('day');
    public endDate: moment.Moment = moment().endOf('day');
    public usernameAuditLog: string;
    public usernameEntityChange: string;
    public methodName: string;
    public browserInfo: string;
    public hasException: boolean = undefined;
    public minExecutionDuration: number;
    public maxExecutionDuration: number;
    public objectTypes: NameValueDto[];

    isShow = false;

    primengTableHelperAuditLogs = new PrimengTableHelper();
    primengTableHelperEntityChanges = new PrimengTableHelper();
    advancedFiltersAreShown = false;

    constructor(
        injector: Injector,
        private _auditLogService: AuditLogServiceProxy,
        private _notifyService: NotifyService,
        private _fileDownloadService: FileDownloadService
    ) {
        super(injector);
    }

    showAuditLogDetails(record: AuditLogListDto): void {
        this.auditLogDetailModal.show(record);
    }

    showEntityChangeDetails(record: EntityChangeListDto): void {
        this.entityChangeDetailModal.show(record);
    }

    getAuditLogs(event?: LazyLoadEvent) {
        if (this.primengTableHelperAuditLogs.shouldResetPaging(event)) {
            this.paginatorAuditLogs.changePage(0);

            return;
        }

        this.primengTableHelperAuditLogs.showLoadingIndicator();

        this._auditLogService.getAuditLogs(
            this.startDate,
            this.endDate,
            this.usernameAuditLog,
            this.serviceName,
            this.methodName,
            this.browserInfo,
            this.hasException,
            this.minExecutionDuration,
            this.maxExecutionDuration,
            this.primengTableHelperAuditLogs.getSorting(this.dataTableAuditLogs),
            this.primengTableHelperAuditLogs.getMaxResultCount(this.paginatorAuditLogs, event),
            this.primengTableHelperAuditLogs.getSkipCount(this.paginatorAuditLogs, event)
        ).subscribe((result) => {
            this.primengTableHelperAuditLogs.totalRecordsCount = result.totalCount;
            this.primengTableHelperAuditLogs.records = result.items;
            this.primengTableHelperAuditLogs.hideLoadingIndicator();
        });
    }

    getEntityChanges(event?: LazyLoadEvent) {
        this._auditLogService.getEntityHistoryObjectTypes()
            .subscribe((result) => {
                this.objectTypes = result;
            });

        if (this.primengTableHelperEntityChanges.shouldResetPaging(event)) {
            this.paginatorEntityChanges.changePage(0);

            return;
        }

        this.primengTableHelperEntityChanges.showLoadingIndicator();

        this._auditLogService.getEntityChanges(
            this.startDate,
            this.endDate,
            this.usernameEntityChange,
            this.entityTypeFullName,
            this.primengTableHelperEntityChanges.getSorting(this.dataTableEntityChanges),
            this.primengTableHelperEntityChanges.getMaxResultCount(this.paginatorEntityChanges, event),
            this.primengTableHelperEntityChanges.getSkipCount(this.paginatorEntityChanges, event)
        ).subscribe((result) => {
            this.primengTableHelperEntityChanges.totalRecordsCount = result.totalCount;
            this.primengTableHelperEntityChanges.records = result.items;
            this.primengTableHelperEntityChanges.hideLoadingIndicator();
        });
    }

    exportToExcelAuditLogs(): void {
        const self = this;
        self._auditLogService.getAuditLogsToExcel(
            self.startDate,
            self.endDate,
            self.usernameAuditLog,
            self.serviceName,
            self.methodName,
            self.browserInfo,
            self.hasException,
            self.minExecutionDuration,
            self.maxExecutionDuration,
            undefined,
            1,
            0)
            .subscribe(result => {
                self._fileDownloadService.downloadTempFile(result);
            });
    }

    exportToExcelEntityChanges(): void {
        const self = this;
        self._auditLogService.getEntityChangesToExcel(
            self.startDate,
            self.endDate,
            self.usernameEntityChange,
            self.entityTypeFullName,
            undefined,
            1,
            0)
            .subscribe(result => {
                self._fileDownloadService.downloadTempFile(result);
            });
    }

    truncateStringWithPostfix(text: string, length: number): string {
        return abp.utils.truncateStringWithPostfix(text, length);
    }

    show(): void {
        this.isShow = true;
        this.modal.show();
        setTimeout(() => {
            this.getAuditLogs();
            this.getEntityChanges();
        }, 0)
    }

    close(): void {
        this.isShow = false;
        this.modal.hide();
    }

    //获取操作显示名称
    getActionDisplay(action: string) {
        var preStr = "";
        if (action.startsWith("Get"))
            preStr = "Search";
        else if (action.startsWith("Remove") || action.startsWith("Delete"))
            preStr = "Delete";
        else if (action.startsWith("Create") || action.startsWith("Add"))
            preStr = "Create";
        else if (action.startsWith("Edit") || action.startsWith("Update"))
            preStr = "Edit";
        return "【" + this.l(preStr) + "】" + action;
    }
}
