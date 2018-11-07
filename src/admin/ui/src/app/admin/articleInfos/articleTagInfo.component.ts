import { Component, Injector, ViewChild } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { AppComponentBase } from '@shared/common/app-component-base';
import * as moment from 'moment';
import { LazyLoadEvent } from 'primeng/components/common/lazyloadevent';
import { Paginator } from 'primeng/components/paginator/paginator';
import { Table } from 'primeng/components/table/table';
import { FileDownloadService } from '@shared/utils/file-download.service';
import { ModalDirective } from 'ngx-bootstrap';

import { ArticleInfoArticleTagInfoServiceProxy } from '@shared/service-proxies/service-proxies';
import { ArticleInfoArticleTagInfoCreateOrEditModalComponent } from './create-or-edit-articleTagInfo-modal.component';

@Component({
    selector: 'ArticleTagInfoModal',
    templateUrl: './articleTagInfo.component.html'
})
export class ArticleInfoArticleTagInfoComponent extends AppComponentBase {
    @ViewChild('dataTable')
    dataTable: Table;
    @ViewChild('paginator')
    paginator: Paginator;
    @ViewChild('RelationModal')
    modal: ModalDirective;
    @ViewChild('createOrEditArticleInfoArticleTagInfoModal')
    createOrEditArticleTagInfoModal: ArticleInfoArticleTagInfoCreateOrEditModalComponent;
    advancedFiltersAreShown = false;
    active = false;
    saving = false;
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
        articleInfoId: number;
        isOnlyGetRecycleData: boolean;
        filterText: string;
        creationDateRangeActive: boolean;
        updateDateRangeActive: boolean;
    } = <any>{};

    constructor(
        injector: Injector,
        private _activatedRoute: ActivatedRoute,
        private _fileDownloadService: FileDownloadService,
        private _articleTagInfoService: ArticleInfoArticleTagInfoServiceProxy
    ) {
        super(injector);
        this.setFiltersFromRoute();
    }

    show(id?: number): void {
        this.active = true;
        this.filters.articleInfoId = id;
        this.modal.show();
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

    onShown(): void { }

    ngOnInit(): void {
        this.filters.filterText =
            this._activatedRoute.snapshot.queryParams['filterText'] || '';
    }

    getArticleTagInfos(event?: LazyLoadEvent) {
        this.active = true;
        if (this.primengTableHelper.shouldResetPaging(event)) {
            this.paginator.changePage(0);
            return;
        }
        this.primengTableHelper.showLoadingIndicator();

        this._articleTagInfoService
            .getArticleTagInfos(
                this.filters.articleInfoId,
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
                this.dataTable != undefined
                    ? this.primengTableHelper.getSorting(this.dataTable)
                    : undefined,
                this.dataTable != undefined
                    ? this.primengTableHelper.getMaxResultCount(
                        this.paginator,
                        event
                    )
                    : undefined,
                this.dataTable != undefined
                    ? this.primengTableHelper.getSkipCount(
                        this.paginator,
                        event
                    )
                    : undefined
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
        this.getArticleTagInfos();
    }
    //恢复数据
    restore(id: number): void {
        this.message.confirm(
            this.l('AreYouSure'),
            this.l('RestoreWarningMessage'),
            isConfirmed => {
                if (isConfirmed) {
                    this._articleTagInfoService
                        .restoreArticleTagInfo(id)
                        .subscribe(() => {
                            this.reloadPage();
                            this.notify.success(this.l('SuccessfullyRestore'));
                        });
                }
            }
        );
    }
    createArticleTagInfo(): void {
        this.active = false;
        this.createOrEditArticleTagInfoModal.show();
    }

    editArticleTagInfo(id: number): void {
        this.active = false;
        this.createOrEditArticleTagInfoModal.show(id);
    }

    deleteArticleTagInfo(id: number): void {
        this.message.confirm(
            this.l('AreYouSure'),
            this.l('DeleteWarningMessage'),
            isConfirmed => {
                if (isConfirmed) {
                    this._articleTagInfoService
                        .deleteArticleTagInfo(id)
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
        this._articleTagInfoService
            .getArticleTagInfosToExcel(
                this.filters.articleInfoId,
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

    close(): void {
        this.modal.hide();
        this.active = false;
    }
}
