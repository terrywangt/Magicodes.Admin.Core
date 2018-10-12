import { AfterViewInit, Component, Injector, ViewEncapsulation } from '@angular/core';
import { AppSalesSummaryDatePeriod } from '@shared/AppEnums';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { AppComponentBase } from '@shared/common/app-component-base';
import { TenantDashboardServiceProxy } from '@shared/service-proxies/service-proxies';
import { curveBasis } from 'd3-shape';

import * as _ from 'lodash';
declare let d3, Datamap: any;


abstract class DashboardChartBase {
    loading = true;

    showLoading() {
        setTimeout(() => { this.loading = true; });
    }

    hideLoading() {
        setTimeout(() => { this.loading = false; });
    }
}

class SalesSummaryChart extends DashboardChartBase {
    totalSales = 0; totalSalesCounter = 0;
    revenue = 0; revenuesCounter = 0;
    expenses = 0; expensesCounter = 0;
    growth = 0; growthCounter = 0;

    selectedDatePeriod: AppSalesSummaryDatePeriod = AppSalesSummaryDatePeriod.Daily;

    data = [];

    constructor(
        private _dashboardService: TenantDashboardServiceProxy,
        private _containerElement: any) {
        super();
    }

    init(salesSummaryData, totalSales, revenue, expenses, growth) {
        this.totalSales = totalSales;
        this.totalSalesCounter = totalSales;

        this.revenue = revenue;
        this.expenses = expenses;
        this.growth = growth;

        this.setChartData(salesSummaryData);

        this.hideLoading();
    }

    setChartData(items): void {
        let sales = [];
        let profit = [];

        _.forEach(items, (item) => {

            sales.push({
                'name': item['period'],
                'value': item['sales']
            });

            profit.push({
                'name': item['period'],
                'value': item['profit']
            });
        });

        this.data = [
            {
                'name': 'Sales',
                'series': sales
            }, {
                'name': 'Profit',
                'series': profit
            }
        ];
    }

    reload(datePeriod) {
        this.selectedDatePeriod = datePeriod;

        this.showLoading();
        this._dashboardService
            .getSalesSummary(datePeriod)
            .subscribe(result => {
                this.setChartData(result.salesSummary);
                this.hideLoading();
            });
    }
}

class RegionalStatsTable extends DashboardChartBase {
    stats: Array<any>;
    colors = ['#00c5dc', '#f4516c', '#34bfa3', '#ffb822'];
    customColors = [
        { name: '1', value: '#00c5dc' },
        { name: '2', value: '#f4516c' },
        { name: '3', value: '#34bfa3' },
        { name: '4', value: '#ffb822' },
        { name: '5', value: '#00c5dc' }
    ];

    curve: any = curveBasis;

    constructor(private _dashboardService: TenantDashboardServiceProxy) {
        super();
    }

    init() {
        this.reload();
    }

    formatData(): any {
        for (let j = 0; j < this.stats.length; j++) {
            let stat = this.stats[j];

            let series = [];
            for (let i = 0; i < stat.change.length; i++) {
                series.push({
                    name: i + 1,
                    value: stat.change[i]
                });
            }

            stat.changeData = [
                {
                    'name': j + 1,
                    'series': series
                }
            ];

        }
    }

    reload() {
        this.showLoading();
        this._dashboardService
            .getRegionalStats({})
            .subscribe(result => {
                this.stats = result.stats;
                this.formatData();
                this.hideLoading();
            });
    }
}

class GeneralStatsPieChart extends DashboardChartBase {

    public data = [];

    constructor(private _dashboardService: TenantDashboardServiceProxy) {
        super();
    }

    init(transactionPercent, newVisitPercent, bouncePercent) {
        this.data = [
            {
                'name': 'Operations',
                'value': transactionPercent
            }, {
                'name': 'New Visits',
                'value': newVisitPercent
            }, {
                'name': 'Bounce',
                'value': bouncePercent
            }];

        this.hideLoading();
    }

    reload() {
        this.showLoading();
        this._dashboardService
            .getGeneralStats({})
            .subscribe(result => {
                this.init(result.transactionPercent, result.newVisitPercent, result.bouncePercent);
            });
    }
}

class DailySalesLineChart extends DashboardChartBase {

    chartData: any[];
    scheme: any = {
        name: 'green',
        selectable: true,
        group: 'Ordinal',
        domain: [
            '#34bfa3'
        ]
    };

    constructor(private _dashboardService: TenantDashboardServiceProxy) {
        super();
    }

    init(data) {
        this.chartData = [];
        for (let i = 0; i < data.length; i++) {
            this.chartData.push({
                name: i + 1,
                value: data[i]
            });
        }
    }

    reload() {
        this.showLoading();
        this._dashboardService
            .getSalesSummary(AppSalesSummaryDatePeriod.Monthly)
            .subscribe(result => {
                this.init(result.salesSummary);
                this.hideLoading();
            });
    }
}

class ProfitSharePieChart extends DashboardChartBase {

    chartData: any[] = [];
    scheme: any = {
        name: 'custom',
        selectable: true,
        group: 'Ordinal',
        domain: [
            '#00c5dc', '#ffb822', '#716aca'
        ]
    };

    constructor(private _dashboardService: TenantDashboardServiceProxy) {
        super();
    }

    init(data: number[]) {

        let formattedData = [];
        for (let i = 0; i < data.length; i++) {
            formattedData.push({
                'name': this.getChartItemName(i),
                'value': data[i]
            });
        }

        this.chartData = formattedData;
    }

    getChartItemName(index: number) {
        if (index === 0) {
            return 'Product Sales';
        }

        if (index === 1) {
            return 'Online Courses';
        }

        if (index === 2) {
            return 'Custom Development';
        }

        return 'Other';
    }
}

class DashboardHeaderStats extends DashboardChartBase {

    totalProfit = 0; totalProfitCounter = 0;
    newFeedbacks = 0; newFeedbacksCounter = 0;
    newOrders = 0; newOrdersCounter = 0;
    newUsers = 0; newUsersCounter = 0;

    totalProfitChange = 76; totalProfitChangeCounter = 0;
    newFeedbacksChange = 85; newFeedbacksChangeCounter = 0;
    newOrdersChange = 45; newOrdersChangeCounter = 0;
    newUsersChange = 57; newUsersChangeCounter = 0;

    init(totalProfit, newFeedbacks, newOrders, newUsers) {
        this.totalProfit = totalProfit;
        this.newFeedbacks = newFeedbacks;
        this.newOrders = newOrders;
        this.newUsers = newUsers;
        this.hideLoading();
    }
}

class MemberActivityTable extends DashboardChartBase {

    memberActivities: Array<any>;

    constructor(private _dashboardService: TenantDashboardServiceProxy) {
        super();
    }

    init() {
        this.reload();
    }

    reload() {
        this.showLoading();
        this._dashboardService
            .getMemberActivity()
            .subscribe(result => {
                this.memberActivities = result.memberActivities;
                this.hideLoading();
            });
    }
}


@Component({
    templateUrl: './dashboard.component.html',
    styleUrls: ['./dashboard.component.less'],
    encapsulation: ViewEncapsulation.None,
    animations: [appModuleAnimation()]
})
export class DashboardComponent extends AppComponentBase implements AfterViewInit {

    appSalesSummaryDateInterval = AppSalesSummaryDatePeriod;
    selectedSalesSummaryDatePeriod: any = AppSalesSummaryDatePeriod.Daily;
    dashboardHeaderStats: DashboardHeaderStats;
    salesSummaryChart: SalesSummaryChart;
    regionalStatsTable: RegionalStatsTable;
    generalStatsPieChart: GeneralStatsPieChart;
    dailySalesLineChart: DailySalesLineChart;
    profitSharePieChart: ProfitSharePieChart;
    memberActivityTable: MemberActivityTable;


    constructor(
        injector: Injector,
        private _dashboardService: TenantDashboardServiceProxy
    ) {
        super(injector);
        this.dashboardHeaderStats = new DashboardHeaderStats();
        this.salesSummaryChart = new SalesSummaryChart(this._dashboardService, 'salesStatistics');
        this.regionalStatsTable = new RegionalStatsTable(this._dashboardService);
        this.generalStatsPieChart = new GeneralStatsPieChart(this._dashboardService);
        this.dailySalesLineChart = new DailySalesLineChart(this._dashboardService);
        this.profitSharePieChart = new ProfitSharePieChart(this._dashboardService);
        this.memberActivityTable = new MemberActivityTable(this._dashboardService);
    }

    getDashboardStatisticsData(datePeriod): void {
        this.salesSummaryChart.showLoading();
        this.generalStatsPieChart.showLoading();

        this._dashboardService
            .getDashboardData(datePeriod)
            .subscribe(result => {
                this.dashboardHeaderStats.init(result.totalProfit, result.newFeedbacks, result.newOrders, result.newUsers);
                this.generalStatsPieChart.init(result.transactionPercent, result.newVisitPercent, result.bouncePercent);
                this.dailySalesLineChart.init(result.dailySales);
                this.profitSharePieChart.init(result.profitShares);
                this.salesSummaryChart.init(result.salesSummary, result.totalSales, result.revenue, result.expenses, result.growth);
            });
    }

    ngAfterViewInit(): void {
        this.getDashboardStatisticsData(AppSalesSummaryDatePeriod.Daily);
        this.regionalStatsTable.init();
        this.memberActivityTable.init();
    }
}
