(function () {
    $(function () {

        var _$paymentHistoryTable = $('#PaymentHistoryTable');
        var _paymentService = abp.services.app.payment;
        var _dataTable;

        function createDatatable() {
            var dataTable = _$paymentHistoryTable.DataTable({
                paging: true,
                serverSide: true,
                processing: true,
                listAction: {
                    ajaxFunction: _paymentService.getPaymentHistory
                },
                columnDefs: [
                    {
                        targets: 0,
                        data: "creationTime",
                        render: function (creationTime) {
                            return moment(creationTime).format('L');
                        }
                    },
                    {
                        targets: 1,
                        data: "editionDisplayName"
                    },
                    {
                        targets: 2,
                        data: "gateway",
                        render: function (gateway) {
                            return app.localize("SubscriptionPaymentGatewayType_" + gateway);
                        }
                    },
                    {
                        targets: 3,
                        data: "amount",
                        render: $.fn.dataTable.render.number(',', '.', 2)
                    },
                    {
                        targets: 4,
                        data: "status",
                        render: function (status) {
                            return app.localize("SubscriptionPaymentStatus_" + status);
                        }
                    },
                    {
                        targets: 5,
                        data: "paymentPeriodType",
                        render: function (paymentPeriodType) {
                            return app.localize("PaymentPeriodType_" + paymentPeriodType);
                        }
                    },
                    {
                        targets: 6,
                        data: "dayCount"
                    },
                    {
                        targets: 7,
                        data: "paymentId"
                    }
                ]
            });

            return dataTable;
        }

        $('a[data-toggle="tab"]').on('shown.bs.tab', function (e) {
            var target = $(e.target).attr("href");
            if (target == '#SubscriptionManagementPaymentHistoryTab') {

                if (_dataTable) {
                    return;
                }

                _dataTable = createDatatable();
            }
        });
    });
})();