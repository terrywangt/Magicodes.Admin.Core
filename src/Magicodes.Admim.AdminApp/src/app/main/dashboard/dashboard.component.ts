import { AfterViewInit, Component, Injector, ViewEncapsulation } from '@angular/core';
import { AppSalesSummaryDatePeriod } from '@shared/AppEnums';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { AppComponentBase } from '@shared/common/app-component-base';
import { TenantDashboardServiceProxy } from '@shared/service-proxies/service-proxies';
declare let d3, Datamap: any;

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
        this.dailySalesLineChart = new DailySalesLineChart(this._dashboardService, '#m_chart_daily_sales');
        this.profitSharePieChart = new ProfitSharePieChart(this._dashboardService, '#m_chart_profit_share');
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
    //Sales summary => MorrisJs: https://github.com/morrisjs/morris.js/

    instance: morris.GridChart;
    totalSales = 0; totalSalesCounter = 0;
    revenue = 0; revenuesCounter = 0;
    expenses = 0; expensesCounter = 0;
    growth = 0; growthCounter = 0;

    constructor(private _dashboardService: TenantDashboardServiceProxy, private _containerElement: any) {
        super();
    }

    init(salesSummaryData, totalSales, revenue, expenses, growth) {
        this.instance = Morris.Area({
            element: this._containerElement,
            padding: 0,
            behaveLikeLine: false,
            gridEnabled: false,
            gridLineColor: 'transparent',
            axes: false,
            fillOpacity: 1,
            data: salesSummaryData,
            lineColors: ['#399a8c', '#92e9dc'],
            xkey: 'period',
            ykeys: ['sales', 'profit'],
            labels: ['Sales', 'Profit'],
            pointSize: 0,
            lineWidth: 0,
            hideHover: 'auto',
            resize: true
        });

        this.totalSales = totalSales;
        this.totalSalesCounter = totalSales;

        this.revenue = revenue;
        this.expenses = expenses;
        this.growth = growth;

        this.hideLoading();
    }

    reload(datePeriod) {
        this.showLoading();
        this._dashboardService
            .getSalesSummary(datePeriod)
            .subscribe(result => {
                this.instance.setData(result.salesSummary, true);
                this.hideLoading();
            });
    }
}

class RegionalStatsTable extends DashboardChartBase {
    stats: Array<any>;

    constructor(private _dashboardService: TenantDashboardServiceProxy) {
        super();
    }

    init() {
        this.reload();
    }

    _initSparklineChart(src, data, color, border) {
        if (src.length === 0) {
            return;
        }

        let config = {
            type: 'line',
            data: {
                labels: ["January", "February", "March", "April", "May", "June", "July", "August", "September", "October"],
                datasets: [{
                    label: "",
                    borderColor: color,
                    borderWidth: border,

                    pointHoverRadius: 4,
                    pointHoverBorderWidth: 12,
                    pointBackgroundColor: Chart.helpers.color('#000000').alpha(0).rgbString(),
                    pointBorderColor: Chart.helpers.color('#000000').alpha(0).rgbString(),
                    pointHoverBackgroundColor: mUtil.getColor('danger'),
                    pointHoverBorderColor: Chart.helpers.color('#000000').alpha(0.1).rgbString(),
                    fill: false,
                    data: data,
                }]
            },
            options: {
                title: {
                    display: false,
                },
                tooltips: {
                    enabled: false,
                    intersect: false,
                    mode: 'nearest',
                    xPadding: 10,
                    yPadding: 10,
                    caretPadding: 10
                },
                legend: {
                    display: false,
                    labels: {
                        usePointStyle: false
                    }
                },
                responsive: true,
                maintainAspectRatio: true,
                hover: {
                    mode: 'index'
                },
                scales: {
                    xAxes: [{
                        display: false,
                        gridLines: false,
                        scaleLabel: {
                            display: true,
                            labelString: 'Month'
                        }
                    }],
                    yAxes: [{
                        display: false,
                        gridLines: false,
                        scaleLabel: {
                            display: true,
                            labelString: 'Value'
                        },
                        ticks: {
                            beginAtZero: true
                        }
                    }]
                },

                elements: {
                    point: {
                        radius: 4,
                        borderWidth: 12
                    },
                },

                layout: {
                    padding: {
                        left: 0,
                        right: 10,
                        top: 5,
                        bottom: 0
                    }
                }
            }
        };

        return new Chart(src, config);
    }

    reload() {
        let self = this;
        this.showLoading();
        this._dashboardService
            .getRegionalStats({})
            .subscribe(result => {
                this.stats = result.stats;
                this.hideLoading();
                var colors = ['accent', 'danger', 'success', 'warning'];
                setTimeout(() => {
                    var $canvasItems = $('canvas.m_chart_sales_by_region');
                    for (var i = 0; i < $canvasItems.length; i++) {
                        var $canvas = $canvasItems[i];
                        self._initSparklineChart($canvas, this.stats[i].change, mUtil.getColor(colors[i % 4]), 2);
                    }
                });
            });
    }
}

class GeneralStatsPieChart extends DashboardChartBase {
    //General stats =>  EasyPieChart: https://rendro.github.io/easy-pie-chart/

    transactionPercent = {
        value: 0,
        options: {
            barColor: '#F8CB00',
            trackColor: '#f9f9f9',
            scaleColor: '#dfe0e0',
            scaleLength: 5,
            lineCap: 'round',
            lineWidth: 3,
            size: 75,
            rotate: 0,
            animate: {
                duration: 1000,
                enabled: true
            }
        }
    };
    newVisitPercent = {
        value: 0,
        options: {
            barColor: '#1bbc9b',
            trackColor: '#f9f9f9',
            scaleColor: '#dfe0e0',
            scaleLength: 5,
            lineCap: 'round',
            lineWidth: 3,
            size: 75,
            rotate: 0,
            animate: {
                duration: 1000,
                enabled: true
            }
        }
    };
    bouncePercent = {
        value: 0,
        options: {
            barColor: '#F3565D',
            trackColor: '#f9f9f9',
            scaleColor: '#dfe0e0',
            scaleLength: 5,
            lineCap: 'round',
            lineWidth: 3,
            size: 75,
            rotate: 0,
            animate: {
                duration: 1000,
                enabled: true
            }
        }
    };

    constructor(private _dashboardService: TenantDashboardServiceProxy) {
        super();
    }

    init(transactionPercent, newVisitPercent, bouncePercent) {
        this.transactionPercent.value = transactionPercent;
        this.newVisitPercent.value = newVisitPercent;
        this.bouncePercent.value = bouncePercent;
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
    //== Daily Sales chart.
    //** Based on Chartjs plugin - http://www.chartjs.org/

    _canvasId: string;

    constructor(private _dashboardService: TenantDashboardServiceProxy, canvasId: string) {
        super();
        this._canvasId = canvasId;
    }

    init(data) {
        let dayLabels = [];
        for (let day = 1; day <= data.length; day++) {
            dayLabels.push('Day ' + day);
        }

        let chartData = {
            labels: dayLabels,
            datasets: [{
                //label: 'Dataset 1',
                backgroundColor: mUtil.getColor('success'),
                data: data
            }, {
                //label: 'Dataset 2',
                backgroundColor: '#f3f3fb',
                data: data
            }]
        };

        let chartContainer = $(this._canvasId);

        if (chartContainer.length === 0) {
            return;
        }

        let chart = new Chart(chartContainer, {
            type: 'bar',
            data: chartData,
            options: {
                title: {
                    display: false,
                },
                tooltips: {
                    intersect: false,
                    mode: 'nearest',
                    xPadding: 10,
                    yPadding: 10,
                    caretPadding: 10
                },
                legend: {
                    display: false
                },
                responsive: true,
                maintainAspectRatio: false,
                barRadius: 4,
                scales: {
                    xAxes: [{
                        display: false,
                        gridLines: false,
                        stacked: true
                    }],
                    yAxes: [{
                        display: false,
                        stacked: true,
                        gridLines: false
                    }]
                },
                layout: {
                    padding: {
                        left: 0,
                        right: 0,
                        top: 0,
                        bottom: 0
                    }
                }
            }
        });

        this.hideLoading();
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
    //== Profit Share Chart.
    //** Based on Chartist plugin - https://gionkunz.github.io/chartist-js/index.html

    _canvasId: string;
    data: number[];

    constructor(private _dashboardService: TenantDashboardServiceProxy, canvasId: string) {
        super();
        this._canvasId = canvasId;
    }

    init(data: number[]) {
        this.data = data;
        if ($(this._canvasId).length === 0) {
            return;
        }

        let chart = new Chartist.Pie(this._canvasId, {
            series: [{
                value: data[0],
                className: 'custom',
                meta: {
                    color: mUtil.getColor('brand')
                }
            },
            {
                value: data[1],
                className: 'custom',
                meta: {
                    color: mUtil.getColor('accent')
                }
            },
            {
                value: data[2],
                className: 'custom',
                meta: {
                    color: mUtil.getColor('warning')
                }
            }
            ],
            labels: [1, 2, 3]
        }, {
                donut: true,
                donutWidth: 17,
                showLabel: false
            });

        chart.on('draw', (data) => {
            if (data.type === 'slice') {
                // Get the total path length in order to use for dash array animation
                let pathLength = data.element._node.getTotalLength();

                // Set a dasharray that matches the path length as prerequisite to animate dashoffset
                data.element.attr({
                    'stroke-dasharray': pathLength + 'px ' + pathLength + 'px'
                });

                // Create animation definition while also assigning an ID to the animation for later sync usage
                let animationDefinition = {
                    'stroke-dashoffset': {
                        id: 'anim' + data.index,
                        dur: 1000,
                        from: -pathLength + 'px',
                        to: '0px',
                        easing: Chartist.Svg.Easing.easeOutQuint,
                        // We need to use `fill: 'freeze'` otherwise our animation will fall back to initial (not visible)
                        fill: 'freeze',
                        'stroke': data.meta.color
                    }
                };

                // If this was not the first slice, we need to time the animation so that it uses the end sync event of the previous animation
                if (data.index !== 0) {
                    (animationDefinition['stroke-dashoffset'] as any).begin = 'anim' + (data.index - 1) + '.end';
                }

                // We need to set an initial value before the animation starts as we are not in guided mode which would do that for us

                data.element.attr({
                    'stroke-dashoffset': -pathLength + 'px',
                    'stroke': data.meta.color
                });

                // We can't use guided mode as the animations need to rely on setting begin manually
                // See http://gionkunz.github.io/chartist-js/api-documentation.html#chartistsvg-function-animate
                data.element.animate(animationDefinition, false);
            }
        });

        this.hideLoading();
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
