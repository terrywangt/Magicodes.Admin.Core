import {
    Component,
    Injector,
    OnInit,
    ViewChild,
    ElementRef,
    ViewEncapsulation
} from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { AppComponentBase } from '@shared/common/app-component-base';
import {
    SwitchEntityInputDtoOfInt64,
    MoveToInputDtoOfInt64,
    MoveToInputDtoOfInt64MoveToPosition
} from '@shared/service-proxies/service-proxies';
import * as moment from 'moment';
import { LazyLoadEvent } from 'primeng/components/common/lazyloadevent';
import { Paginator } from 'primeng/components/paginator/paginator';
import { Table } from 'primeng/components/table/table';
import { FileDownloadService } from '@shared/utils/file-download.service';
import { ColumnInfoServiceProxy } from '@shared/service-proxies/service-proxies';
import { CreateOrEditColumnInfoModalComponent } from './create-or-edit-columnInfo-modal.component';
import { TreeNode } from 'primeng/api';

export interface TreeNode {
    data?: any;
    children?: TreeNode[];
    leaf?: boolean;
    expanded?: boolean;
}

@Component({
    templateUrl: './columnInfo.component.html',
    encapsulation: ViewEncapsulation.None,
    animations: [appModuleAnimation()]
})
export class ColumnInfosComponent extends AppComponentBase implements OnInit {
    @ViewChild('dataTable')
    dataTable: Table;
    @ViewChild('paginator')
    paginator: Paginator;
    @ViewChild('createOrEditColumnInfoModal')
    createOrEditColumnInfoModal: CreateOrEditColumnInfoModalComponent;
    @ViewChild('IsFooterNav') isFooterNav: ElementRef;
    advancedFiltersAreShown = false;
    list: TreeNode[] = [];
    loading = false;
    isShowTreeTable = true;
    creationDateRange: Date[] = [
        moment()
            .startOf('day')
            .toDate(),
        moment()
            .endOf('day')
            .toDate()
    ];
    updateDateRange: Date[] = [
        moment()
            .startOf('day')
            .toDate(),
        moment()
            .add(30, 'days')
            .endOf('day')
            .toDate()
    ];
    filters: {
        isOnlyGetRecycleData: boolean;
        parentId: number;
        isFooterNav: boolean;
        isHeaderNav: boolean;
        filterText: string;
        creationDateRangeActive: boolean;
        updateDateRangeActive: boolean;
        isActive: string;
        isNeedAuthorizeAccess: string;
    } = <any>{};

    constructor(
        injector: Injector,
        private _activatedRoute: ActivatedRoute,
        private _fileDownloadService: FileDownloadService,
        private _columnInfoService: ColumnInfoServiceProxy
    ) {
        super(injector);
        this.setFiltersFromRoute();
    }

    onCheckbox() {
        this.isFooterNav.nativeElement.disabled = !this.filters.isHeaderNav;
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

        if (
            this._activatedRoute.snapshot.queryParams['updateDateStart'] != null
        ) {
            this.filters.updateDateRangeActive = true;
            this.updateDateRange[0] = moment(
                this._activatedRoute.snapshot.queryParams['updateDateStart']
            ).toDate();
        } else {
            this.updateDateRange[0] = moment()
                .add(-7, 'days')
                .startOf('day')
                .toDate();
        }
        if (
            this._activatedRoute.snapshot.queryParams['updateDateStart'] != null
        ) {
            this.filters.updateDateRangeActive = true;
            this.updateDateRange[1] = moment(
                this._activatedRoute.snapshot.queryParams['updateDateStart']
            ).toDate();
        } else {
            this.updateDateRange[1] = moment()
                .endOf('day')
                .toDate();
        }
    }

    ngOnInit(): void {
        this.filters.filterText =
            this._activatedRoute.snapshot.queryParams['filterText'] || '';
        this.filters.isHeaderNav = true;
        this.isFooterNav.nativeElement.disabled = !this.filters.isHeaderNav;
        this.getList();
    }

    showOrdinaryTable() {
        this.isShowTreeTable = !this.isShowTreeTable;
    }

    getColumnInfos(event?: LazyLoadEvent) {
        if (this.primengTableHelper.shouldResetPaging(event)) {
            this.paginator.changePage(0);
            return;
        }
        this.primengTableHelper.showLoadingIndicator();

        this._columnInfoService
            .getColumnInfos(
                this.filters.isOnlyGetRecycleData
                    ? this.filters.isOnlyGetRecycleData
                    : false,
                this.filters.creationDateRangeActive
                    ? moment(this.creationDateRange[0])
                    : undefined,
                this.filters.creationDateRangeActive
                    ? moment(this.creationDateRange[1])
                    : undefined,
                this.filters.updateDateRangeActive
                    ? moment(this.updateDateRange[0])
                    : undefined,
                this.filters.updateDateRangeActive
                    ? moment(this.updateDateRange[1])
                    : undefined,
                this.filters.filterText,
                this.dataTable === undefined
                    ? undefined
                    : this.primengTableHelper.getSorting(this.dataTable),
                this.paginator === undefined
                    ? 10
                    : this.primengTableHelper.getMaxResultCount(
                          this.paginator,
                          event
                      ),
                this.paginator === undefined
                    ? 0
                    : this.primengTableHelper.getSkipCount(
                          this.paginator,
                          event
                      )
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
        this.getColumnInfos();
    }
    //恢复数据
    restore(id: number): void {
        this.message.confirm(
            this.l('AreYouSure'),
            this.l('RestoreWarningMessage'),
            isConfirmed => {
                if (isConfirmed) {
                    this._columnInfoService
                        .restoreColumnInfo(id)
                        .subscribe(() => {
                            this.reloadPage();
                            this.notify.success(this.l('SuccessfullyRestore'));
                        });
                }
            }
        );
    }
    createColumnInfo(): void {
        this.createOrEditColumnInfoModal.show();
    }

    editColumnInfo(id: number): void {
        this.createOrEditColumnInfoModal.show(id);
    }

    deleteColumnInfo(id: number): void {
        this.message.confirm(
            this.l('AreYouSure'),
            this.l('DeleteWarningMessage'),
            isConfirmed => {
                if (isConfirmed) {
                    this._columnInfoService
                        .deleteColumnInfo(id)
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
        this._columnInfoService
            .getColumnInfosToExcel(
                this.filters.isOnlyGetRecycleData,
                this.filters.creationDateRangeActive
                    ? moment(this.creationDateRange[0])
                    : undefined,
                this.filters.creationDateRangeActive
                    ? moment(this.creationDateRange[1])
                    : undefined,
                this.filters.updateDateRangeActive
                    ? moment(this.updateDateRange[0])
                    : undefined,
                this.filters.updateDateRangeActive
                    ? moment(this.updateDateRange[1])
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
    getReorder(event) {
        let sourceIndex,
            targetIndex = 0;
        let dropIndex = event.dropIndex;
        let input = new MoveToInputDtoOfInt64();
        //拖到最顶部
        if (dropIndex === 0) {
            sourceIndex = 0;
            targetIndex = 1;
            input.moveToPosition = MoveToInputDtoOfInt64MoveToPosition._0;
        } else {
            //拖拽到目标项的下面
            sourceIndex = dropIndex;
            targetIndex = dropIndex - 1;
            input.moveToPosition = MoveToInputDtoOfInt64MoveToPosition._1;
        }
        input.sourceId = this.primengTableHelper.records[sourceIndex].id;
        input.targetId = this.primengTableHelper.records[targetIndex].id;
        this._columnInfoService.moveTo(input).subscribe();
    }
    handleIsActiveSwitch(event, id: number) {
        const input = new SwitchEntityInputDtoOfInt64();
        input.id = id;
        input.switchValue = event.checked;
        this._columnInfoService
            .updateIsActiveSwitchAsync(input)
            .subscribe(result => {
                this.notify.success(this.l('SuccessfulOperation'));
            });
    }
    handleIsNeedAuthorizeAccessSwitch(event, id: number) {
        const input = new SwitchEntityInputDtoOfInt64();
        input.id = id;
        input.switchValue = event.checked;
        this._columnInfoService
            .updateIsNeedAuthorizeAccessSwitchAsync(input)
            .subscribe(result => {
                this.notify.success(this.l('SuccessfulOperation'));
            });
    }

    getList(node?) {
        this.loading = true;
        // node ? node.data.id : undefined, false;
        this._columnInfoService
            .getChildrenColumnInfos(
                this.filters.parentId,
                this.filters.isHeaderNav,
                this.filters.isFooterNav,
                this.filters.isOnlyGetRecycleData
            )
            .subscribe(
                result => {
                    if (node) {
                        node.children = <TreeNode[]>result.data;
                    } else {
                        this.list = <TreeNode[]>result.data;
                    }
                },
                error => {},
                () => {
                    this.loading = false;
                }
            );
    }

    onNodeExpand(event) {
        if (event.node.children.length <= 0) {
            this.getList(event.node);
        } else {
            event.node.children.forEach(element => {
                if (
                    typeof element.children === 'undefined' ||
                    element.children.length <= 0
                ) {
                    this.getList(element);
                }
            });
        }
    }

    /**
     * 刷新Table
     */
    RefreshTable() {
        if (this.isShowTreeTable) {
            this.getList();
        } else {
            this.getColumnInfos();
        }
    }

    getColumnTypeText(value: number) {
        return this.l(ColumnTypeEnum[value]);
    }
}

//定义列表枚举字段，以便友好化展示
enum ColumnTypeEnum {
    Html = 0,
    Image = 1
}
