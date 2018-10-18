import { Component, ElementRef, Injector, OnInit, ViewChild, ViewEncapsulation } from '@angular/core';
import { AppIncomeStatisticsDateInterval } from '@shared/AppEnums';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { AppComponentBase } from '@shared/common/app-component-base';
import { HostDashboardData, HostDashboardServiceProxy } from '@shared/service-proxies/service-proxies';
import * as moment from 'moment';
import * as _ from 'lodash';
import { Table } from 'primeng/components/table/table';

@Component({
    templateUrl: './host-dashboard.component.html',
    styleUrls: ['./host-dashboard.component.less'],
    encapsulation: ViewEncapsulation.None,
    animations: [appModuleAnimation()]
})
export class HostDashboardComponent extends AppComponentBase implements OnInit {
    @ViewChild('EditionStatisticsChart') editionStatisticsChart: ElementRef;
    @ViewChild('IncomeStatisticsChart') incomeStatisticsChart: ElementRef;

    @ViewChild('RecentTenantsTable') recentTenantsTable: Table;
    @ViewChild('ExpiringTenantsTable') expiringTenantsTable: Table;

    loading = false;
    loadingIncomeStatistics = false;
    isInitialized: boolean;
    hostDashboardData: HostDashboardData;
    currency = '$';
    appIncomeStatisticsDateInterval = AppIncomeStatisticsDateInterval;
    selectedIncomeStatisticsDateInterval: number;

    editionStatisticsHasData = false;
    incomeStatisticsHasData = false;

    incomeStatisticsData: any = [];
    editionStatisticsData: any = [];

    selectedDateRange: Date[] = [moment().add(-7, 'days').startOf('day').toDate(), moment().endOf('day').toDate()];

    expiringTenantsData = [];
    recentTenantsData = [];

    constructor(
        injector: Injector,
        private _hostDashboardService: HostDashboardServiceProxy
    ) {
        super(injector);
    }

    init(): void {
        this.selectedIncomeStatisticsDateInterval = AppIncomeStatisticsDateInterval.Daily;
        this.getDashboardStatisticsData(this.selectedDateRange);
    }

    ngOnInit(): void {
        this.init();
    }

    getDashboardStatisticsData(dates: any): void {

        if (!dates || !dates[0] || !dates[1]) {
            return;
        }

        this.loading = true;

        this._hostDashboardService
            .getDashboardStatisticsData(
                this.selectedIncomeStatisticsDateInterval,
                dates[0],
                dates[1]
            )
            .subscribe(result => {
                this.hostDashboardData = result;

                this.incomeStatisticsData = this.normalizeIncomeStatisticsData(result.incomeStatistics);
                this.incomeStatisticsHasData = _.filter(this.incomeStatisticsData[0].series, data => data.value > 0).length > 0;

                this.editionStatisticsData = this.normalizeEditionStatisticsData(result.editionStatistics);
                this.editionStatisticsHasData = _.filter(this.editionStatisticsData, data => data.value > 0).length > 0;

                this.recentTenantsData = result.recentTenants;
                this.expiringTenantsData = result.expiringTenants;

                this.loading = false;
            });
    }

    /*
    * Edition statistics pie chart
    */

    normalizeEditionStatisticsData(data): Array<any> {
        const chartData = new Array(data.length);

        for (let i = 0; i < data.length; i++) {
            chartData[i] = {
                name: data[i].label,
                value: data[i].value
            };
        }

        return chartData;
    }

    /*
     * Income statistics line chart
     */


    normalizeIncomeStatisticsData(data): any {
        const chartData = [];
        for (let i = 0; i < data.length; i++) {
            chartData.push({
                'name': moment(moment(data[i].date).utc().valueOf()).format('L'),
                'value': data[i].amount
            });
        }

        return [{
            name: '',
            series: chartData
        }];
    }

    incomeStatisticsDateIntervalChange(interval: number) {
        this.selectedIncomeStatisticsDateInterval = interval;
        this.refreshIncomeStatisticsData();
    }

    refreshIncomeStatisticsData(): void {
        this.loadingIncomeStatistics = true;
        this._hostDashboardService.getIncomeStatistics(
            this.selectedIncomeStatisticsDateInterval,
            moment(this.selectedDateRange[0]),
            moment(this.selectedDateRange[1]))
            .subscribe(result => {
                this.incomeStatisticsData = this.normalizeIncomeStatisticsData(result.incomeStatistics);
                this.incomeStatisticsHasData = _.filter(this.incomeStatisticsData[0].series, data => data.value > 0).length > 0;
                this.loadingIncomeStatistics = false;
            });
    }

    gotoAllRecentTenants(): void {
        window.open(abp.appPath + 'app/admin/tenants?' +
            'creationDateStart=' + encodeURIComponent(this.hostDashboardData.tenantCreationStartDate.format()));
    }

    gotoAllExpiringTenants(): void {
        const url = abp.appPath +
            'app/admin/tenants?' +
            'subscriptionEndDateStart=' +
            encodeURIComponent(this.hostDashboardData.subscriptionEndDateStart.format()) +
            '&' +
            'subscriptionEndDateEnd=' +
            encodeURIComponent(this.hostDashboardData.subscriptionEndDateEnd.format());

        window.open(url);
    }
}
