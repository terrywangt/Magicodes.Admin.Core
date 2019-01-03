import {
    Component,
    Injector,
    OnInit,
    ViewChild,
    ViewEncapsulation
} from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { AppComponentBase } from '@shared/common/app-component-base';
import { SwitchEntityInputDtoOfInt64 } from '@shared/service-proxies/service-proxies';
import * as moment from 'moment';
import { LazyLoadEvent } from 'primeng/components/common/lazyloadevent';
import { Paginator } from 'primeng/components/paginator/paginator';
import { Table } from 'primeng/components/table/table';
import { FileDownloadService } from '@shared/utils/file-download.service';
import {
    ArticleInfoServiceProxy,
    GetDataComboItemDtoOfInt64
} from '@shared/service-proxies/service-proxies';
import { CreateOrEditArticleInfoModalComponent } from './create-or-edit-articleInfo-modal.component';
import { ArticleInfoArticleTagInfoComponent } from './articleTagInfo.component';

@Component({
    templateUrl: './articleInfo.component.html',
    encapsulation: ViewEncapsulation.None,
    animations: [appModuleAnimation()]
})
export class ArticleInfosComponent extends AppComponentBase implements OnInit {
    @ViewChild('dataTable')
    dataTable: Table;
    @ViewChild('paginator')
    paginator: Paginator;
    @ViewChild('createOrEditArticleInfoModal')
    createOrEditArticleInfoModal: CreateOrEditArticleInfoModalComponent;
    @ViewChild('ArticleTagInfoModal')
    articleTagInfoModal: ArticleInfoArticleTagInfoComponent;
    columnInfoComboItemDtoList: GetDataComboItemDtoOfInt64[];
    advancedFiltersAreShown = false;
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
        //是否仅获取回收站数据
        isOnlyGetRecycleData: boolean;
        columnInfoId: number;
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
        private _articleInfoService: ArticleInfoServiceProxy
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

        this._articleInfoService
            .getColumnInfoDataComboItems()
            .subscribe(result => {
                this.columnInfoComboItemDtoList = result;
            });
    }

    getArticleInfos(event?: LazyLoadEvent) {
        moment.locale('cn');
        if (this.primengTableHelper.shouldResetPaging(event)) {
            this.paginator.changePage(0);
            return;
        }
        this.primengTableHelper.showLoadingIndicator();

        this._articleInfoService
            .getArticleInfos(
                this.filters.isOnlyGetRecycleData
                    ? this.filters.isOnlyGetRecycleData
                    : false,
                this.filters.columnInfoId,
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
        this.getArticleInfos();
    }
    //恢复数据
    restore(id: number): void {
        this.message.confirm(
            this.l('AreYouSure'),
            this.l('RestoreWarningMessage'),
            isConfirmed => {
                if (isConfirmed) {
                    this._articleInfoService
                        .restoreArticleInfo(id)
                        .subscribe(() => {
                            this.reloadPage();
                            this.notify.success(this.l('SuccessfullyRestore'));
                        });
                }
            }
        );
    }
    createArticleInfo(): void {
        this.createOrEditArticleInfoModal.show();
    }

    editArticleInfo(id: number): void {
        this.createOrEditArticleInfoModal.show(id);
    }

    deleteArticleInfo(id: number): void {
        this.message.confirm(
            this.l('AreYouSure'),
            this.l('DeleteWarningMessage'),
            isConfirmed => {
                if (isConfirmed) {
                    this._articleInfoService
                        .deleteArticleInfo(id)
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
        this._articleInfoService
            .getArticleInfosToExcel(
                this.filters.isOnlyGetRecycleData,
                this.filters.columnInfoId,
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

    handleIsActiveSwitch(event, id: number) {
        const input = new SwitchEntityInputDtoOfInt64();
        input.id = id;
        input.switchValue = event.checked;
        this._articleInfoService
            .updateIsActiveSwitchAsync(input)
            .subscribe(result => {
                this.notify.success(this.l('SuccessfulOperation'));
            });
    }
    handleIsNeedAuthorizeAccessSwitch(event, id: number) {
        const input = new SwitchEntityInputDtoOfInt64();
        input.id = id;
        input.switchValue = event.checked;
        this._articleInfoService
            .updateIsNeedAuthorizeAccessSwitchAsync(input)
            .subscribe(result => {
                this.notify.success(this.l('SuccessfulOperation'));
            });
    }

    getRecommendedTypeText(value: number) {
        return this.l(RecommendedTypeEnum[value]);
    }
}

//定义列表枚举字段，以便友好化展示
enum RecommendedTypeEnum {
    Top = 0,
    Hot = 1,
    Recommend = 2,
    Common = 3
}
