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
                    barColor: "#ffb822"
                });

                $('.easy-pie-chart .number.visits').easyPieChart({
                    animate: 1000,
                    size: 75,
                    lineWidth: 3,
                    barColor: "#36a3f7"
                });

                $('.easy-pie-chart .number.bounce').easyPieChart({
                    animate: 1000,
                    size: 75,
                    lineWidth: 3,
                    barColor: "#f4516c"
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

        //== Daily Sales chart.
        //** Based on Chartjs plugin - http://www.chartjs.org/
        var initDailySales = function (data) {
            var dayLabels = [];
            for (var day = 1; day <= data.length; day++) {
                dayLabels.push("Day " + day);
            }

            var chartData = {
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

            var chartContainer = $('#m_chart_daily_sales');

            if (chartContainer.length === 0) {
                return;
            }

            var chart = new Chart(chartContainer, {
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
        }


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

        //== Profit Share Chart.
        //** Based on Chartist plugin - https://gionkunz.github.io/chartist-js/index.html
        var profitShare = function (data) {
            var $chart = $('#m_chart_profit_share');
            if ($chart.length === 0) {
                return;
            }

            var $chartItems = $chart.closest('.m-widget14').find('.m-widget14__legend-text');

            $($chartItems[0]).text(data[0] + '% Product Sales');
            $($chartItems[1]).text(data[1] + '% Online Courses');
            $($chartItems[2]).text(data[2] + '% Custom Development');

            var chart = new Chartist.Pie('#m_chart_profit_share', {
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

            chart.on('draw', function (data) {
                if (data.type === 'slice') {
                    // Get the total path length in order to use for dash array animation
                    var pathLength = data.element._node.getTotalLength();

                    // Set a dasharray that matches the path length as prerequisite to animate dashoffset
                    data.element.attr({
                        'stroke-dasharray': pathLength + 'px ' + pathLength + 'px'
                    });

                    // Create animation definition while also assigning an ID to the animation for later sync usage
                    var animationDefinition = {
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
                        animationDefinition['stroke-dashoffset'].begin = 'anim' + (data.index - 1) + '.end';
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
        }

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
                    initDailySales(result.dailySales);
                    initGeneralStats(result.transactionPercent, result.newVisitPercent, result.bouncePercent);
                    profitShare(result.profitShares);
                    $(".counterup").counterUp();
                });
        };

        initWorldMap();
        initMemberActivity();
        getDashboardData();
    });
})();