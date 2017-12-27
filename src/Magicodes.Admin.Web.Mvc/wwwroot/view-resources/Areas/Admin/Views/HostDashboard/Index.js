(function () {
    $(function () {

        var _hostDashboardService = abp.services.app.hostDashboard;
        var _$container = $("#HostDashboard");
        var _$dateRangePicker = _$container.find(".dashboard-report-range");
        var _$incomeStatisticsDatePeriod = _$container.find("input[name='IncomeStatisticsDatePeriod']");
        var _$expiringTenantsTable = _$container.find(".expiring-tenants-table");
        var _$recentTenantsTable = _$container.find(".recent-tenants-table");
        var _$seeAllExpiringTenantsButton = _$container.find(".see-all-expiring-tenants");
        var _$seeAllRecentTenantsButton = _$container.find(".see-all-recent-tenants");
        var _$incomeStatisticsChartContainer = _$container.find(".income-statistics .income-statistics-chart");
        var _$editionStatisticsCaptionHelper = _$container.find(".edition-statistics .caption-helper");
        var _$editionStatisticsChartContainer = _$container.find(".edition-statistics .edition-statistics-chart");
        var _$incomeStatisticsCaptionHelper = _$container.find(".income-statistics .caption-helper");
        var _$newTenantsStatusTitle = _$container.find(".new-tenants-statistics .status-title");
        var _$newTenantsStatisticsCountPlaceholder = _$container.find(".new-tenants-statistics .new-tenants-count");
        var _$newSubscriptionAmountTitle = _$container.find(".new-subscription-statistics .status-title");
        var _$newSubscriptionAmountPlaceholder = _$container.find(".new-subscription-statistics .new-subscription-amount");
        var _$dashboardStatisticsPlaceholder1 = _$container.find(".dashboard-statistics1 .dashboard-placeholder1");
        var _$dashboardStatisticsPlaceholder2 = _$container.find(".dashboard-statistics2 .dashboard-placeholder2");
        var _$expiringTenantsCaptionHelper = _$container.find(".expiring-tenants .caption-helper");
        var _$recentTenantsCaptionHelper = _$container.find(".recent-tenants .caption-helper");
        var _$refreshButton = _$container.find("button[name='RefreshButton']");
        var _$counterUp = _$container.find(".counterup");
        var _expiringTenantsData = [], _recentTenantsData = [], _incomeStatisticsData = [];
        var _currency = "$";

        var chartDateInterval = {
            daily: 1,
            weekly: 2,
            monthly: 3
        };

        var _selectedDateRange = {
            startDate: moment().add(-7, 'days').startOf('day'),
            endDate: moment().endOf("day")
        };

        var showSelectedDate = function () {
            if (_$dateRangePicker.attr("data-display-range") !== "0") {
                _$dateRangePicker.find(".m-subheader__daterange-date").html(_selectedDateRange.startDate.format("LL") +
                    " - " +
                    _selectedDateRange.endDate.format("LL"));
            }
        };

        var populateExpiringTenantsTable = function (expiringTenants, subscriptionEndAlertDayCount, maxExpiringTenantsShownCount, subscriptionEndDateStart, subscriptionEndDateEnd) {
            _$expiringTenantsCaptionHelper.text(app.localize("ExpiringTenantsHelpText", subscriptionEndAlertDayCount, maxExpiringTenantsShownCount));
            _expiringTenantsData = expiringTenants;
            _expiringTenantsDataTable.ajax.reload();

            _$seeAllExpiringTenantsButton
                .data("subscriptionEndDateStart", subscriptionEndDateStart)
                .data("subscriptionEndDateEnd", subscriptionEndDateEnd)
                .click(function () {
                    window.open(abp.appPath + "Admin/Tenants?" +
                        "subscriptionEndDateStart=" + encodeURIComponent($(this).data("subscriptionEndDateStart")) + "&" +
                        "subscriptionEndDateEnd=" + encodeURIComponent($(this).data("subscriptionEndDateEnd")));
                });
        };

        var populateRecentTenantsTable = function (recentTenants, recentTenantsDayCount, maxRecentTenantsShownCount, creationDateStart) {
            _$recentTenantsCaptionHelper.text(app.localize("RecentTenantsHelpText", recentTenantsDayCount, maxRecentTenantsShownCount));
            _recentTenantsData = recentTenants;
            _recentTenantsDataTable.ajax.reload();
            
            _$seeAllRecentTenantsButton
                .data("creationDateStart", creationDateStart)
                .click(function () {
                    window.open(abp.appPath + "Admin/Tenants?" +
                        "creationDateStart=" + encodeURIComponent($(this).data("creationDateStart")));
                });
        };

        var getCurrentDateRangeText = function () {
            return _selectedDateRange.startDate.format("L") + " - " + _selectedDateRange.endDate.format("L");
        };

        var getNoDataInfo = function () {
            return $("<div/>")
                .addClass("note")
                .addClass("note-info")
                .addClass("text-center")
                .append(
                $("<small/>")
                    .addClass("text-muted")
                    .text("- " + app.localize("NoData") + " -")
                );
        };

        var getSelectedIncomeStatisticsDatePeriod = function () {
            return parseInt(_$incomeStatisticsDatePeriod.closest(":checked").val());
        };

        var drawIncomeStatisticsChart = function (data) {
            var incomeStatisticsChartLastTooltipIndex = null;
            _$incomeStatisticsCaptionHelper.text(getCurrentDateRangeText());
            var chartData = [];
            for (var i = 0; i < data.length; i++) {
                var point = new Array(2);
                point[0] = moment(data[i].date).utc().valueOf(); // data[i].label;
                point[1] = data[i].amount;
                chartData.push(point);
            }

            _incomeStatisticsData = chartData;
            if (!data || data.length === 0) {
                _$incomeStatisticsChartContainer.html(getNoDataInfo());
                return;
            }

            $.plot(_$incomeStatisticsChartContainer,
                [{
                    data: chartData,
                    lines: {
                        fill: 0.2,
                        lineWidth: 1
                    },
                    color: ["#BAD9F5"]
                }, {
                    data: chartData,
                    points: {
                        show: true,
                        fill: true,
                        radius: 4,
                        fillColor: "#9ACAE6",
                        lineWidth: 2
                    },
                    color: "#9ACAE6",
                    shadowSize: 1
                }, {
                    data: chartData,
                    lines: {
                        show: true,
                        fill: false,
                        lineWidth: 3
                    },
                    color: "#9ACAE6",
                    shadowSize: 0
                }],
                {
                    xaxis: {
                        mode: "time",
                        timeformat: app.localize("ChartDateFormat"),
                        minTickSize: [1, "day"],
                        font: {
                            lineHeight: 20,
                            style: "normal",
                            variant: "small-caps",
                            color: "#6F7B8A",
                            size: 10
                        }
                    },
                    yaxis: {
                        ticks: 5,
                        tickDecimals: 0,
                        tickColor: "#eee",
                        font: {
                            lineHeight: 14,
                            style: "normal",
                            variant: "small-caps",
                            color: "#6F7B8A"
                        }
                    },
                    grid: {
                        hoverable: true,
                        clickable: false,
                        tickColor: "#eee",
                        borderColor: "#eee",
                        borderWidth: 1,
                        margin: {
                            bottom: 20
                        }
                    }
                });


            var removeChartTooltipIfExists = function () {
                var $chartTooltip = $("#chartTooltip");
                if ($chartTooltip.length === 0) {
                    return;
                }

                $chartTooltip.remove();
            };

            var showChartTooltip = function (x, y, label, value) {
                removeChartTooltipIfExists();
                $("<div id='chartTooltip' class='chart-tooltip'>" + label + "<br/>" + value + "</div >")
                    .css({
                        position: "absolute",
                        display: "none",
                        top: y - 60,
                        left: x - 40,
                        border: "0",
                        padding: "2px 6px",
                        opacity: "0.9"
                    })
                    .appendTo("body")
                    .fadeIn(200);
            };

            _$incomeStatisticsChartContainer.bind("plothover", function (event, pos, item) {
                if (!item) {
                    return;
                }

                if (incomeStatisticsChartLastTooltipIndex !== item.dataIndex) {
                    var interval = getSelectedIncomeStatisticsDatePeriod();
                    var label = "";
                    var isSingleDaySelected = _selectedDateRange.startDate.format("L") === _selectedDateRange.endDate.format("L");

                    if (interval === chartDateInterval.daily || isSingleDaySelected) {
                        label = moment(item.datapoint[0]).format("dddd, DD MMMM YYYY");
                    }
                    else {
                        var isLastItem = item.dataIndex === item.series.data.length - 1;
                        label += moment(item.datapoint[0]).format("LL");
                        if (isLastItem) {
                            label += " - " + _selectedDateRange.endDate.format("LL");
                        } else {
                            var nextItem = item.series.data[item.dataIndex + 1];
                            label += " - " + moment(nextItem[0]).format("LL");
                        }
                    }

                    incomeStatisticsChartLastTooltipIndex = item.dataIndex;
                    var value = app.localize("IncomeWithAmount", "<strong>" + item.datapoint[1] + _currency + "</strong>");
                    showChartTooltip(item.pageX, item.pageY, label, value);
                }
            });

            _$incomeStatisticsChartContainer.bind("mouseleave", function () {
                incomeStatisticsChartLastTooltipIndex = null;
                removeChartTooltipIfExists();
            });
        };

        var drawEditionStatisticsData = function (data) {
            if (!data || data.length === 0) {
                _$editionStatisticsChartContainer.html(getNoDataInfo());
                return;
            }

            var colorPalette = ["#81A17E", "#BA9B7C", "#569BC6", "#e08283", "#888888"];
            var chartData = [];
            for (var i = 0; i < data.length; i++) {
                var pie = {
                    label: data[i].label,
                    data: data[i].value
                };

                if (colorPalette[i]) {
                    pie.color = colorPalette[i];
                }

                chartData.push(pie);
            }

            $.plot(_$editionStatisticsChartContainer, chartData, {
                series: {
                    pie: {
                        show: true,
                        innerRadius: 0.3,
                        radius: 1,
                        label: {
                            show: true,
                            radius: 1,
                            formatter: function (label, series) {
                                return "<div class='pie-chart-label'>" + label + " : " + Math.round(series.percent) + "%</div>";
                            },
                            background: {
                                opacity: 0.8
                            }
                        }
                    }
                },
                legend: {
                    show: false
                },
                grid: {
                    hoverable: true,
                    clickable: true
                }
            });


            _$editionStatisticsCaptionHelper.text(getCurrentDateRangeText());
        };

        var writeNewTenantsCount = function (newTenantsCount) {
            _$newTenantsStatusTitle.text(getCurrentDateRangeText());
            _$newTenantsStatisticsCountPlaceholder.text(newTenantsCount);
        };

        var writeNewSubscriptionsAmount = function (newSubscriptionAmount) {
            _$newSubscriptionAmountTitle.text(getCurrentDateRangeText());
            _$newSubscriptionAmountPlaceholder.text(newSubscriptionAmount);
        };

        //this is a sample placeholder. You can put your own statistics here.
        var writeDashboardPlaceholder1 = function (value) {
            _$dashboardStatisticsPlaceholder1.text(value);
        };

        //this is a sample placeholder. You can put your own statistics here.
        var writeDashboardPlaceholder2 = function (value) {
            _$dashboardStatisticsPlaceholder2.text(value);
        };

        var animateCounterUpNumbers = function () {
            _$counterUp.counterUp();
        };

        var getAllDataAndDrawCharts = function () {
            abp.ui.setBusy(_$container);
            _hostDashboardService
                .getDashboardStatisticsData({
                    startDate: _selectedDateRange.startDate.format("YYYY-MM-DDT00:00:00Z"),
                    endDate: _selectedDateRange.endDate.format("YYYY-MM-DDT23:59:59.999Z"),
                    incomeStatisticsDateInterval: getSelectedIncomeStatisticsDatePeriod()
                })
                .done(function (result) {
                    //counts
                    writeNewTenantsCount(result.newTenantsCount);
                    writeNewSubscriptionsAmount(result.newSubscriptionAmount);
                    writeDashboardPlaceholder1(result.dashboardPlaceholder1);
                    writeDashboardPlaceholder2(result.dashboardPlaceholder2);
                    animateCounterUpNumbers();

                    //charts
                    drawIncomeStatisticsChart(result.incomeStatistics);
                    drawEditionStatisticsData(result.editionStatistics);

                    //tables
                    populateExpiringTenantsTable(
                        result.expiringTenants,
                        result.subscriptionEndAlertDayCount,
                        result.maxExpiringTenantsShownCount,
                        result.subscriptionEndDateStart,
                        result.subscriptionEndDateEnd
                    );

                    populateRecentTenantsTable(
                        result.recentTenants,
                        result.recentTenantsDayCount,
                        result.maxRecentTenantsShownCount,
                        result.tenantCreationStartDate
                    );
                }).always(function () {
                    abp.ui.clearBusy(_$container);
                });
        };

        var refreshAllData = function () {
            showSelectedDate();
            getAllDataAndDrawCharts();
        };

        var refreshIncomeStatisticsData = function () {
            abp.ui.setBusy(_$incomeStatisticsChartContainer);
            _hostDashboardService.getIncomeStatistics({
                startDate: _selectedDateRange.startDate.format("YYYY-MM-DDT00:00:00Z"),
                endDate: _selectedDateRange.endDate.format("YYYY-MM-DDT23:59:59.999Z"),
                incomeStatisticsDateInterval: getSelectedIncomeStatisticsDatePeriod()
            }).done(function (result) {
                drawIncomeStatisticsChart(result.incomeStatistics);
            }).always(function () {
                abp.ui.clearBusy(_$incomeStatisticsChartContainer);
            });
        };

        var initIncomeStatisticsDatePeriod = function () {
            _$container.find("input[name='IncomeStatisticsDatePeriod']").change(function () {
                if (!this.checked) {
                    return;
                }

                refreshIncomeStatisticsData();
            });
        };


        var _expiringTenantsDataTable = null;
        var initExpiringTenantsTable = function () {
            _expiringTenantsDataTable = _$expiringTenantsTable.DataTable({
                paging: false,
                serverSide: false,
                processing: false,
                info: false,
                listAction: {
                    ajaxFunction: function () {
                        return $.Deferred(function ($dfd) {
                            $dfd.resolve({
                                "items": _expiringTenantsData,
                                "totalCount": _expiringTenantsData.length
                            });
                        });
                    }
                },
                columnDefs: [
                    {
                        targets: 0,
                        data: "tenantName"
                    },
                    {
                        targets: 1,
                        data: "remainingDayCount"
                    }
                ]
            });
        };

        var _recentTenantsDataTable = null;
        var initRecentTenantsTable = function () {
            _recentTenantsDataTable = _$recentTenantsTable.DataTable({
                paging: false,
                serverSide: false,
                processing: false,
                info: false,
                listAction: {
                    ajaxFunction: function () {
                        return $.Deferred(function ($dfd) {
                            $dfd.resolve({
                                "items": _recentTenantsData,
                                "totalCount": _recentTenantsData.length
                            });
                        });
                    }
                },
                columnDefs: [
                    {
                        targets: 0,
                        data: "name"
                    },
                    {
                        targets: 1,
                        data: "creationTime",
                        render: function (creationTime) {
                            return moment(creationTime).format("L LT");
                        }
                    }
                ]
            });
        };

        var initialize = function () {
            initIncomeStatisticsDatePeriod();
            initExpiringTenantsTable();
            initRecentTenantsTable();
            refreshAllData();
        };

        _$dateRangePicker.daterangepicker(
            $.extend(true, app.createDateRangePickerOptions(), _selectedDateRange), function (start, end, label) {
                _selectedDateRange.startDate = start;
                _selectedDateRange.endDate = end;
                refreshAllData();
            });

        _$refreshButton.click(function () {
            refreshAllData();
        });

        initialize();
    });
})();