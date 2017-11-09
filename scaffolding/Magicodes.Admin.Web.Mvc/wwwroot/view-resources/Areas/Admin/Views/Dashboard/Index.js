(function () {
    $(function () {
        var _tenantDashboardService = abp.services.app.tenantDashboard;
        var colors = d3.scale.category10();
        var salesSummaryDatePeriod = {
            daily: 1,
            weekly: 2,
            monthly: 3
        };

        var initDashboardTopStats = function (totalProfit, newFeedbacks, newOrders, newUsers) {
            //Dashboard top stats => CounterUp: https://github.com/bfintal/Counter-Up

            $("#totalProfit").text(totalProfit);
            $("#newFeedbacks").text(newFeedbacks);
            $("#newOrders").text(newOrders);
            $("#newUsers").text(newUsers);
        };

        var initSalesSummaryChart = function (salesSummaryData, totalSales, revenue, expenses, growth) {
            //Sales summary => MorrisJs: https://github.com/morrisjs/morris.js/

            var SalesSummaryChart = function (element) {
                var instance = null;

                var init = function (data) {
                    return new Morris.Area({
                        element: element,
                        padding: 0,
                        behaveLikeLine: false,
                        gridEnabled: false,
                        gridLineColor: false,
                        axes: false,
                        fillOpacity: 1,
                        data: data,
                        lineColors: ['#399a8c', '#92e9dc'],
                        xkey: 'period',
                        ykeys: ['sales', 'profit'],
                        labels: ['Sales', 'Profit'],
                        pointSize: 0,
                        lineWidth: 0,
                        hideHover: 'auto',
                        resize: true
                    });
                }

                var refresh = function (datePeriod) {
                    var self = this;
                    _tenantDashboardService
                        .getSalesSummary({
                            salesSummaryDatePeriod: datePeriod
                        })
                        .done(function (result) {
                            self.graph.setData(result.salesSummary);
                            self.graph.redraw();
                        });
                };

                var draw = function (data) {
                    if (!this.graph) {
                        this.graph = init(data);
                    } else {
                        this.graph.setData(data);
                        this.graph.redraw();
                    }
                };

                return {
                    draw: draw,
                    refresh: refresh,
                    graph: instance
                }
            };

            $("#salesStatistics").show();
            var salesSummary = new SalesSummaryChart("salesStatistics");
            salesSummary.draw(salesSummaryData);

            $("input[name='SalesSummaryDateInterval'").change(function () {
                salesSummary.refresh(this.value);
            });

            $("#totalSales").text(totalSales);
            $("#revenue").text(revenue);
            $("#expenses").text(expenses);
            $("#growth").text(growth);
            $("#salesStatisticsLoading").hide();
        };

        var initGeneralStats = function (transactionPercent, newVisitPercent, bouncePercent) {
            //General stats =>  EasyPieChart: https://rendro.github.io/easy-pie-chart/

            var init = function (transactionPercent, newVisitPercent, bouncePercent) {
                $("#transactionPercent").attr("data-percent", transactionPercent);
                $("#transactionPercent span").text(transactionPercent);
                $("#newVisitPercent").attr("data-percent", newVisitPercent);
                $("#newVisitPercent span").text(newVisitPercent);
                $("#bouncePercent").attr("data-percent", bouncePercent);
                $("#bouncePercent span").text(bouncePercent);
                $(".easy-pie-chart-loading").hide();
            }

            var refreshGeneralStats = function (transactionPercent, newVisitPercent, bouncePercent) {
                $('#transactionPercent').data('easyPieChart').update(transactionPercent);
                $("#transactionPercent span").text(transactionPercent);
                $('#newVisitPercent').data('easyPieChart').update(newVisitPercent);
                $("#newVisitPercent span").text(newVisitPercent);
                $('#bouncePercent').data('easyPieChart').update(bouncePercent);
                $("#bouncePercent span").text(bouncePercent);
            };

            var createPieCharts = function () {
                $('.easy-pie-chart .number.transactions').easyPieChart({
                    animate: 1000,
                    size: 75,
                    lineWidth: 3,
                    barColor: App.getBrandColor('yellow')
                });

                $('.easy-pie-chart .number.visits').easyPieChart({
                    animate: 1000,
                    size: 75,
                    lineWidth: 3,
                    barColor: App.getBrandColor('green')
                });

                $('.easy-pie-chart .number.bounce').easyPieChart({
                    animate: 1000,
                    size: 75,
                    lineWidth: 3,
                    barColor: App.getBrandColor('red')
                });
            }

            $("#generalStatsReload").click(function () {
                _tenantDashboardService
                    .getGeneralStats({})
                    .done(function (result) {
                        refreshGeneralStats(result.transactionPercent, result.newVisitPercent, result.bouncePercent);
                    });
            });

            init(transactionPercent, newVisitPercent, bouncePercent);
            createPieCharts();
        };

        var initServerStatCharts = function (networkLoad, cpuLoad, loadRate) {
            //Server stats => Sparklines: https://github.com/imsky/jquery.sparkline

            var draw = function (networkLoad, cpuLoad, loadRate) {
                $("#network").sparkline(loadRate,
                    {
                        type: 'bar',
                        width: '100',
                        barWidth: 5,
                        height: '55',
                        barColor: '#35aa47',
                        negBarColor: '#e02222'
                    });

                $("#cpu-load").sparkline(cpuLoad,
                    {
                        type: 'bar',
                        width: '100',
                        barWidth: 5,
                        height: '55',
                        barColor: '#ffb848',
                        negBarColor: '#e02222'
                    });

                $("#load-rate").sparkline(loadRate,
                    {
                        type: 'line',
                        width: '100',
                        height: '55',
                        lineColor: '#ffb848'
                    });
            };

            var refreshServerStats = function () {
                _tenantDashboardService
                    .getServerStats({})
                    .done(function (result) {
                        draw(result.networkLoad, result.cpuLoad, result.loadRate);
                    });
            };

            $("#serverStatsReload").click(function () {
                refreshServerStats();
            });

            draw(networkLoad, cpuLoad, loadRate);
            $(".sparkline-chart-loading").hide();
        };

        var initWorldMap = function () {
            //World map => DataMaps: https://github.com/markmarkoh/datamaps/

            var WorldMap = function (element) {
                var instance = null;

                var init = function (data) {

                    return new Datamap({
                        element: document.getElementById(element),
                        projection: 'mercator',
                        fills: {
                            defaultFill: "#ABDDA4",
                            key: "#fa0fa0"
                        },
                        data: data,
                        done: function (datamap) {
                            function redraw() {
                                datamap.svg.selectAll("g").attr("transform", "translate(" + d3.event.translate + ")scale(" + d3.event.scale + ")");
                            }

                            datamap.svg.call(d3.behavior.zoom().on("zoom", redraw));
                        }
                    });
                };

                var redraw = function () {
                    var self = this;
                    _tenantDashboardService
                        .getWorldMap({})
                        .done(function (result) {
                            var mapData = {};
                            for (var i = 0; i < result.countries.length; i++) {
                                var country = result.countries[i];
                                mapData[country.countryName] = colors(Math.random() * country.color);
                            }

                            self.graph.updateChoropleth(mapData);
                        });
                };



                var draw = function (data) {
                    if (!this.graph) {
                        this.graph = init(data);
                    } else {
                        this.redraw();
                    }
                };

                return {
                    draw: draw,
                    redraw: redraw,
                    graph: instance
                }
            };

            var containerElement = "worldmap";

            var init = function () {
                var _worldMap = new WorldMap(containerElement);
                _worldMap.draw({
                    USA: { fillKey: "key" },
                    JPN: { fillKey: "key" },
                    ITA: { fillKey: "key" },
                    CRI: { fillKey: "key" },
                    KOR: { fillKey: "key" },
                    DEU: { fillKey: "key" },
                    TUR: { fillKey: "key" },
                    RUS: { fillKey: "key" }
                });

                return _worldMap;
            };

            var worldMap = init();

            setInterval(function () {
                worldMap.redraw();
            }, 5000);

            $(window).resize(function () {
                $("#" + containerElement).empty();
                worldMap = init();
            });
        };

        var initActivitiesTimeline = function (records) {
            //Activities => Horizontal timeline: https://github.com/CodyHouse/horizontal-timeline

            var $timelineDetails = $("#timelineDetails");
            var $timelineDateList = $("#timelineDateList");
            var $horizontalTimelineContainer = $("#horizontalTimelineContainer");
            var $horizontalTimelineLoading = $("#horizontalTimelineLoading");

            $timelineDetails.empty();
            $timelineDateList.empty();

            for (var i = 0; i < records.length; i++) {
                var record = records[i];

                var timeLineHeader = abp.utils.formatString(
                    "<li><a href='#0' data-date='{0}' class='border-after-red bg-after-red {2}'>{1}</a></li>",
                    record.shortDate,
                    record.titleDate,
                    i === 0 ? "selected" : "");

                $timelineDateList.append(timeLineHeader);

                var timeLineDetail = abp.utils.formatString("<li class='{0}' data-date='{1}'>" +
                    "<div class='mt-title'>" +
                    "<h2 class='mt-content-title'>{2}</h2>" +
                    "</div>" +
                    "<div class='mt-author' > " +
                    "<div class='mt-avatar' > " +
                    "<img src='{3}'/>" +
                    "</div> " +
                    "<div class='mt-author-name' > " +
                    "<a href= 'javascript:;' class='font-blue-madison'>{4}</a > " +
                    "</div>" +
                    "<div class='mt-author-datetime font-grey-mint'>{5}</div>" +
                    "</div> " +
                    "<div class='clearfix'></div > " +
                    "<div class='mt-content border-grey-steel' style='max-height: 70px;overflow: auto;'> " +
                    "<p>{6}</p> " +
                    "</div > " +
                    "</li>",
                    (i === 0 ? "selected" : ""),
                    record.shortDate,
                    record.title,
                    abp.appPath + 'metronic/assets/admin/layout4/media/users/' + record.image,
                    record.autherName,
                    record.longDate,
                    record.text);

                $timelineDetails.append(timeLineDetail);
            }

            $horizontalTimelineContainer.addClass("cd-horizontal-timeline mt-timeline-horizontal");
            if (records) {
                $('.cd-horizontal-timeline').horizontalTimeline();
            }

            $horizontalTimelineLoading.hide();
            $horizontalTimelineContainer.show();
        };

        var initMemberActivity = function () {
            var refreshMemberActivity = function () {
                _tenantDashboardService
                    .getMemberActivity({})
                    .done(function (result) {
                        $("#memberActivityTable tbody>tr").each(function (index) {
                            var cells = $(this).find("td");
                            var $link = $("<a/>")
                                .attr("href", "javascript:;")
                                .addClass("primary-link")
                                .text(result.memberActivities[index].name);

                            $(cells[1]).empty().append($link);
                            $(cells[2]).html(result.memberActivities[index].cases);
                            $(cells[3]).html(result.memberActivities[index].closed);
                            $(cells[4]).html(result.memberActivities[index].rate);
                            $(cells[5]).html(result.memberActivities[index].rate);
                            $(cells[6]).html(result.memberActivities[index].earnings);
                        });
                    });
            };

            $("#refreshMemberActivityButton").click(function () {
                refreshMemberActivity();
            });

            refreshMemberActivity();
        };

        var getDashboardData = function () {
            _tenantDashboardService
                .getDashboardData({
                    salesSummaryDatePeriod: salesSummaryDatePeriod.daily
                })
                .done(function (result) {
                    initSalesSummaryChart(result.salesSummary, result.totalSales, result.revenue, result.expenses, result.growth);
                    initDashboardTopStats(result.totalProfit, result.newFeedbacks, result.newOrders, result.newUsers);
                    initServerStatCharts(result.networkLoad, result.cpuLoad, result.loadRate);
                    initGeneralStats(result.transactionPercent, result.newVisitPercent, result.bouncePercent);
                    initActivitiesTimeline(result.timeLineItems);
                    $(".counterup").counterUp();
                });
        };

        initWorldMap();
        initMemberActivity();
        getDashboardData();
    });
})();