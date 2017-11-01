(function () {
    $(function () {

        var _$paymentHistoryTable = $('#PaymentHistoryTable');
        var _paymentService = abp.services.app.payment;
        var _invoiceService = abp.services.app.invoice;
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
                        data: null,
                        orderable: false,
                        defaultContent: '',
                        rowAction: {
                            element: $("<button/>")
                                .addClass("btn btn-xs btn-primary blue")
                                .text(app.localize('ShowInvoice'))
                                .click(function () {
                                    createOrShowInvoice($(this).data());
                                })
                        }
                    },
                    {
                        targets: 1,
                        data: "creationTime",
                        render: function (creationTime) {
                            return moment(creationTime).format('L');
                        }
                    },
                    {
                        targets: 2,
                        data: "editionDisplayName"
                    },
                    {
                        targets: 3,
                        data: "gateway",
                        render: function (gateway) {
                            return app.localize("SubscriptionPaymentGatewayType_" + gateway);
                        }
                    },
                    {
                        targets: 4,
                        data: "amount",
                        render: $.fn.dataTable.render.number(',', '.', 2)
                    },
                    {
                        targets: 5,
                        data: "status",
                        render: function (status) {
                            return app.localize("SubscriptionPaymentStatus_" + status);
                        }
                    },
                    {
                        targets: 6,
                        data: "paymentPeriodType",
                        render: function (paymentPeriodType) {
                            return app.localize("PaymentPeriodType_" + paymentPeriodType);
                        }
                    },
                    {
                        targets: 7,
                        data: "dayCount"
                    },
                    {
                        targets: 8,
                        data: "paymentId"
                    },
                    {
                        targets: 9,
                        data: "invoiceNo"
                    },
                    {
                        targets: 10,
                        visible: false,
                        data: "id"
                    }
                ]
            });

            return dataTable;
        }

        $('a[data-toggle="tab"]').on('shown.bs.tab', function (e) {
            var target = $(e.target).attr("href");
            if (target === '#SubscriptionManagementPaymentHistoryTab') {

                if (_dataTable) {
                    return;
                }

                _dataTable = createDatatable();
            }
        });

        function createOrShowInvoice(data) {
            var invoiceNo = data["invoiceNo"];
            var paymentId = data["id"];

            if (invoiceNo) {
                window.open('/Admin/Invoice?paymentId=' + paymentId, '_blank');
            } else {
                _invoiceService.createInvoice({
                    subscriptionPaymentId: paymentId
                }).done(function () {
                    _dataTable.ajax.reload();
                    window.open('/Admin/Invoice?paymentId=' + paymentId, '_blank');
                });
            }
        }
    });
})();