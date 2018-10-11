import { Component, Injector, OnInit, ViewChild, ViewEncapsulation } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { ImpersonationService } from '@app/admin/users/impersonation.service';
import { CommonLookupModalComponent } from '@app/shared/common/lookup/common-lookup-modal.component';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { AppComponentBase } from '@shared/common/app-component-base';
import { EntityDtoOfInt64, FindUsersInput, NameValueDto, SwitchEntityInputDtoOfInt64, } from '@shared/service-proxies/service-proxies';
import * as moment from 'moment';
import { LazyLoadEvent } from 'primeng/components/common/lazyloadevent';
import { Paginator } from 'primeng/components/paginator/paginator';
import { Table } from 'primeng/components/table/table';
import { FileDownloadService } from '@shared/utils/file-download.service';
import { InputSwitchModule } from 'primeng/inputswitch'
import { ModalDirective } from 'ngx-bootstrap';

import { ArticleInfoArticleTagInfoServiceProxy } from '@shared/service-proxies/service-proxies';
import { ArticleInfoArticleTagInfoCreateOrEditModalComponent } from './create-or-edit-articleTagInfo-modal.component';

@Component({
    selector: 'ArticleTagInfoModal',
    templateUrl: './articleTagInfo.component.html'
})

export class ArticleInfoArticleTagInfoComponent extends AppComponentBase {
    @ViewChild('dataTable') dataTable: Table;
    @ViewChild('paginator') paginator: Paginator;
    @ViewChild('RelationModal') modal: ModalDirective;
    @ViewChild('createOrEditArticleInfoArticleTagInfoModal') createOrEditArticleTagInfoModal: ArticleInfoArticleTagInfoCreateOrEditModalComponent;

    advancedFiltersAreShown = false;
    public createDateRange: moment.Moment[] = [moment().startOf('day'), moment().endOf('day')];
    public updateDateRange: moment.Moment[] = [moment().startOf('day'), moment().endOf('day')];
    active = false;
    saving = false;

    model: {
        articleInfoId: number;
        //是否仅获取回收站数据
        isOnlyGetRecycleData: boolean;
        filterText: string;
        creationDateRangeActive: boolean;
        creationDateStart: moment.Moment;
        creationDateEnd: moment.Moment;
        subscriptionEndDateRangeActive: boolean;
        subscriptionEndDateStart: moment.Moment;
        subscriptionEndDateEnd: moment.Moment;
    } = <any>{};

    constructor(
        injector: Injector,
        private _activatedRoute: ActivatedRoute,
        private _fileDownloadService: FileDownloadService,

        private _articleTagInfoService: ArticleInfoArticleTagInfoServiceProxy,

    ) {
        super(injector);
        this.setFiltersFromRoute();
    }

    show(id?: number): void {
        this.active = true;
        this.model.articleInfoId = id;
        this.modal.show();
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

    onShown(): void {
    }

    ngOnInit(): void {
        const self = this;
        self.model.filterText = self._activatedRoute.snapshot.queryParams['filterText'] || '';
    }

    getArticleTagInfos(event?: LazyLoadEvent) {
        const self = this;
        self.active = true;
        if (self.primengTableHelper.shouldResetPaging(event)) {
            self.paginator.changePage(0);
            return;
        }
        self.primengTableHelper.showLoadingIndicator();

        self._articleTagInfoService.getArticleTagInfos(
            self.model.articleInfoId,
            self.model.isOnlyGetRecycleData ? self.model.isOnlyGetRecycleData : false,
            self.model.creationDateRangeActive ? self.model.creationDateStart : undefined,
            self.model.creationDateRangeActive ? self.model.creationDateEnd : undefined,

            self.model.subscriptionEndDateRangeActive ? self.model.subscriptionEndDateStart : undefined,
            self.model.subscriptionEndDateRangeActive ? self.model.subscriptionEndDateEnd : undefined,
            self.model.filterText,
            self.dataTable != undefined ? self.primengTableHelper.getSorting(self.dataTable) : undefined,
            self.dataTable != undefined ? self.primengTableHelper.getMaxResultCount(self.paginator, event) : undefined,
            self.dataTable != undefined ? self.primengTableHelper.getSkipCount(self.paginator, event) : undefined
        ).subscribe(result => {
            self.primengTableHelper.totalRecordsCount = result.totalCount;
            self.primengTableHelper.records = result.items;
            self.primengTableHelper.hideLoadingIndicator();
        });
    }

    //获取回收站数据
    getRecycleData(): void {
        this.model.isOnlyGetRecycleData = !this.model.isOnlyGetRecycleData;
        this.getArticleTagInfos();
    }
    //恢复数据
    restore(id: number): void {
        const self = this;
        self.message.confirm(
            self.l('AreYouSure'),
            self.l('RestoreWarningMessage'),
            isConfirmed => {
                if (isConfirmed) {
                    self._articleTagInfoService.restoreArticleTagInfo(id).subscribe(() => {
                        self.reloadPage();
                        self.notify.success(self.l('SuccessfullyRestore'));
                    });
                }
            }
        );
    }
    createArticleTagInfo(): void {
        const self = this;
        self.active = false;
        self.createOrEditArticleTagInfoModal.show();
    }

    editArticleTagInfo(id: number): void {
        const self = this;
        self.active = false;
        self.createOrEditArticleTagInfoModal.show(id);
    }

    deleteArticleTagInfo(id: number): void {
        const self = this;
        self.message.confirm(
            self.l('AreYouSure'),
            self.l('DeleteWarningMessage'),
            isConfirmed => {
                if (isConfirmed) {
                    self._articleTagInfoService.deleteArticleTagInfo(id).subscribe(() => {
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
        self._articleTagInfoService.getArticleTagInfosToExcel(
            self.model.articleInfoId,
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


    close(): void {
        this.modal.hide();
        this.active = false;
    }


}