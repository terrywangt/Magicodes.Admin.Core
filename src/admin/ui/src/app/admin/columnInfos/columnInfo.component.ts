import { Data } from '@angular/router/src/config';
import { Component, Injector, OnInit, ViewChild, ViewEncapsulation } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { ImpersonationService } from '@app/admin/users/impersonation.service';
import { CommonLookupModalComponent } from '@app/shared/common/lookup/common-lookup-modal.component';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { AppComponentBase } from '@shared/common/app-component-base';
import { EntityDtoOfInt64, FindUsersInput, NameValueDto, SwitchEntityInputDtoOfInt64, MoveToInputDtoOfInt64, MoveToInputDtoOfInt64MoveToPosition } from '@shared/service-proxies/service-proxies';
import * as moment from 'moment';
import { LazyLoadEvent } from 'primeng/components/common/lazyloadevent';
import { Paginator } from 'primeng/components/paginator/paginator';
import { Table } from 'primeng/components/table/table';
import { FileDownloadService } from '@shared/utils/file-download.service';
import { InputSwitchModule } from 'primeng/inputswitch';
import { ColumnInfoServiceProxy } from '@shared/service-proxies/service-proxies';
import { CreateOrEditColumnInfoModalComponent } from './create-or-edit-columnInfo-modal.component';
import { TreeNode } from 'primeng/api';
import { TreeTableModule } from 'primeng/treetable';


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
    @ViewChild('dataTable') dataTable: Table;
    @ViewChild('paginator') paginator: Paginator;
	@ViewChild('createOrEditColumnInfoModal') createOrEditColumnInfoModal: CreateOrEditColumnInfoModalComponent;
	advancedFiltersAreShown = false;
	list: TreeNode[] = [];
    loading = false;
	isShowTreeTable=true;
	model: {
        //是否仅获取回收站数据
        isOnlyGetRecycleData: boolean;
        filterText: string;
		creationDateRangeActive: boolean;        
        creationDateStart: moment.Moment;
        creationDateEnd: moment.Moment;
		subscriptionEndDateRangeActive: boolean;
        subscriptionEndDateStart: moment.Moment;
        subscriptionEndDateEnd: moment.Moment;
		isActive: string;
		isNeedAuthorizeAccess: string;
    } = <any>{};

	constructor(
        injector: Injector,        
        private _activatedRoute: ActivatedRoute,
		private _fileDownloadService: FileDownloadService,

		private _columnInfoService: ColumnInfoServiceProxy,
        
    ) {		
        super(injector);
        this.setFiltersFromRoute();
    }

	setFiltersFromRoute(): void {
		const self = this;
        if (self._activatedRoute.snapshot.queryParams['subscriptionEndDateStart'] != null) {
            self.model.subscriptionEndDateRangeActive = true;
            self.model.subscriptionEndDateStart = moment(this._activatedRoute.snapshot.queryParams['subscriptionEndDateStart']);
        } else {
            self.model.subscriptionEndDateStart = moment().startOf('day');
        }

        if (self._activatedRoute.snapshot.queryParams['subscriptionEndDateEnd'] != null) {
            self.model.subscriptionEndDateRangeActive = true;
            self.model.subscriptionEndDateEnd = moment(this._activatedRoute.snapshot.queryParams['subscriptionEndDateEnd']);
        } else {
            self.model.subscriptionEndDateEnd = moment().add(30, 'days').endOf('day');
        }
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
		this.getList();
    }

	showOrdinaryTable(){
        this.isShowTreeTable=!this.isShowTreeTable;
    }

	getColumnInfos(event?: LazyLoadEvent) {
		const self = this;
        if (self.primengTableHelper.shouldResetPaging(event)) {
            self.paginator.changePage(0);
            return;
        }
        self.primengTableHelper.showLoadingIndicator();

        self._columnInfoService.getColumnInfos(
            self.model.isOnlyGetRecycleData ? self.model.isOnlyGetRecycleData : false,
			self.model.creationDateRangeActive ? self.model.creationDateStart : undefined,
			self.model.creationDateRangeActive ? self.model.creationDateEnd : undefined,
			self.model.subscriptionEndDateRangeActive ? self.model.subscriptionEndDateStart : undefined,
            self.model.subscriptionEndDateRangeActive ? self.model.subscriptionEndDateEnd : undefined,
			self.model.filterText,                  
            self.dataTable==undefined ? undefined : self.primengTableHelper.getSorting(self.dataTable),
            self.paginator==undefined ? 10 : self.primengTableHelper.getMaxResultCount(self.paginator, event),
            self.paginator==undefined ? 0 : self.primengTableHelper.getSkipCount(self.paginator, event)
        ).subscribe(result => {
            self.primengTableHelper.totalRecordsCount = result.totalCount;
            self.primengTableHelper.records = result.items;
            self.primengTableHelper.hideLoadingIndicator();
        });
    }

    //获取回收站数据
    getRecycleData(): void {
        this.model.isOnlyGetRecycleData = !this.model.isOnlyGetRecycleData;
        this.getColumnInfos();
    }
    //恢复数据
    restore(id: number): void {
        const self = this;
        self.message.confirm(
            self.l('AreYouSure'),
            self.l('RestoreWarningMessage'),
            isConfirmed => {
                if (isConfirmed) {
                    self._columnInfoService.restoreColumnInfo(id).subscribe(() => {
                        self.reloadPage();
                        self.notify.success(self.l('SuccessfullyRestore'));
                    });
                }
            }
        );
    }
	createColumnInfo(): void {
		const self = this;
        self.createOrEditColumnInfoModal.show();
    }

	editColumnInfo(id:number): void {
		const self = this;
        self.createOrEditColumnInfoModal.show(id);
    }

	deleteColumnInfo(id: number): void {
		const self = this;
		self.message.confirm(
            self.l('AreYouSure'),
            self.l('DeleteWarningMessage'),
            isConfirmed => {
                if (isConfirmed) {
                    self._columnInfoService.deleteColumnInfo(id).subscribe(() => {
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
        self._columnInfoService.getColumnInfosToExcel(
            self.model.isOnlyGetRecycleData,
			self.model.subscriptionEndDateRangeActive ? self.model.subscriptionEndDateStart : undefined,
            self.model.subscriptionEndDateRangeActive ? self.model.subscriptionEndDateEnd : undefined,
			self.model.creationDateRangeActive ? self.model.creationDateStart : undefined,
			self.model.creationDateRangeActive ? self.model.creationDateEnd : undefined,
			self.model.filterText,
            undefined,
            1000,
            0).subscribe(result => {
                self._fileDownloadService.downloadTempFile(result);
            });
    }
    getReorder(event) {
        var sourceIndex, targetIndex = 0;
        var dropIndex = event.dropIndex;
        var input = new MoveToInputDtoOfInt64();
        //拖到最顶部
        if (dropIndex == 0) {
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
	handleIsActiveSwitch(event, id: number){
			const self = this;
			const input = new SwitchEntityInputDtoOfInt64();
			input.id = id;
			input.switchValue = event.checked;
			self._columnInfoService.updateIsActiveSwitchAsync(input).subscribe(result => {
                self.notify.success(self.l('SuccessfulOperation'));
			})
		}
	handleIsNeedAuthorizeAccessSwitch(event, id: number){
			const self = this;
			const input = new SwitchEntityInputDtoOfInt64();
			input.id = id;
			input.switchValue = event.checked;
			self._columnInfoService.updateIsNeedAuthorizeAccessSwitchAsync(input).subscribe(result => {
                self.notify.success(self.l('SuccessfulOperation'));
			})
		}


getList(node?) {
        this.loading = true;
        this._columnInfoService.getChildrenColumnInfos(node ? node.data.id : undefined, false)
            .subscribe(result => {
                if (node) {
                    node.children = <TreeNode[]>result.data;
                } else {
                    this.list = <TreeNode[]>result.data;
                }
            }, error => {

            }, () => {
                this.loading = false;
            });

    }

    onNodeExpand(event) {
        if (event.node.children.length <= 0) {
            this.getList(event.node);
        } else {
            event.node.children.forEach(element => {
                if (typeof (element.children) === 'undefined' || element.children.length <= 0) {
                    this.getList(element);
                }
            });
        }

    }

}