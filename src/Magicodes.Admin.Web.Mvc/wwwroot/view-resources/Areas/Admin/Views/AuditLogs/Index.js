(function ($) {
    $(function () {

        var _$auditLogsTable = $('#AuditLogsTable');
        var _$filterForm = $('#AuditLogFilterForm');
        var _auditLogService = abp.services.app.auditLog;

        var _selectedDateRange = {
            startDate: moment().startOf('day'),
            endDate: moment().endOf('day')
        };

        _$filterForm.find('input.date-range-picker').daterangepicker(
            $.extend(true, app.createDateRangePickerOptions(), _selectedDateRange),
            function (start, end, label) {
                _selectedDateRange.startDate = start.format('YYYY-MM-DDT00:00:00Z');
                _selectedDateRange.endDate = end.format('YYYY-MM-DDT23:59:59.999Z');
            });


        var dataTable = _$auditLogsTable.DataTable({
            paging: true,
            serverSide: true,
            processing: true,
            listAction: {
                ajaxFunction: _auditLogService.getAuditLogs,
                inputFilter: function () {
                    return createRequestParams();
                }
            },
            columnDefs: [
                {
                    targets: 0,
                    data: null,
                    orderable: false,
                    defaultContent: '',
                    rowAction: {
                        element: $("<div/>")
                            .addClass("text-center")
                            .append($("<button/>")
                                .addClass("btn m-btn m-btn--hover-accent m-btn--icon m-btn--icon-only m-btn--pill")
                                .attr("title", app.localize("AuditLogDetail"))
                                .append($("<i/>").addClass("la la-search"))
                            ).click(function () {
                                showDetails($(this).data());
                            })
                    }
                },
                {
                    targets: 1,
                    data: "exception",
                    orderable: false,
                    render: function (exception) {
                        var $div = $("<div/>").addClass("text-center");
                        if (exception) {
                            $div.append($("<i/>").addClass("fa fa-warning m--font-warning").attr("title", app.localize("HasError")));
                        } else {
                            $div.append($("<i/>").addClass("fa fa-check-circle m--font-success").attr("title", app.localize("Success")));
                        }

                        return $div[0].outerHTML;
                    }
                },
                {
                    targets: 2,
                    data: "executionTime",
                    render: function (executionTime) {
                        return moment(executionTime).format('YYYY-MM-DD HH:mm:ss');
                    }
                },
                {
                    targets: 3,
                    data: "userName"
                },
                {
                    targets: 4,
                    data: "serviceName"
                },
                {
                    targets: 5,
                    data: "methodName"
                },
                {
                    targets: 6,
                    data: "executionDuration",
                    render: function (executionDuration) {
                        return app.localize('Xms', executionDuration);
                    }
                },
                {
                    targets: 7,
                    data: "clientIpAddress",
                    orderable: false
                },
                {
                    targets: 8,
                    data: "clientName"
                },
                {
                    targets: 9,
                    data: "browserInfo",
                    render: function (browserInfo) {
                        return $("<span/>").text(abp.utils.truncateStringWithPostfix(browserInfo, 32))
                            .attr("title", browserInfo)[0].outerHTML;
                    }
                }
            ]
        });

        function createRequestParams() {
            var prms = {};
            _$filterForm.serializeArray().map(function (x) { prms[abp.utils.toCamelCase(x.name)] = x.value; });
            return $.extend(prms, _selectedDateRange);
        }

        function getAuditLogs() {
            dataTable.ajax.reload();
        }

        function getFormattedParameters(parameters) {
            try {
                var json = JSON.parse(parameters);
                return JSON.stringify(json, null, 4);
            } catch (e) {
                return parameters;
            }
        }

        function showDetails(auditLog) {
            $('#AuditLogDetailModal_UserName').html(auditLog.userName);
            $('#AuditLogDetailModal_ClientIpAddress').html(auditLog.clientIpAddress);
            $('#AuditLogDetailModal_ClientName').html(auditLog.clientName);
            $('#AuditLogDetailModal_BrowserInfo').html(auditLog.browserInfo);
            $('#AuditLogDetailModal_ServiceName').html(auditLog.serviceName);
            $('#AuditLogDetailModal_MethodName').html(auditLog.methodName);
            $('#AuditLogDetailModal_ExecutionTime').html(moment(auditLog.executionTime).fromNow() + ' (' + moment(auditLog.executionTime).format('YYYY-MM-DD hh:mm:ss') + ')');
            $('#AuditLogDetailModal_Duration').html(app.localize('Xms', auditLog.executionDuration));
            $('#AuditLogDetailModal_Parameters').html(getFormattedParameters(auditLog.parameters));

            if (auditLog.impersonatorUserId) {
                $('#AuditLogDetailModal_ImpersonatorInfo').show();
            } else {
                $('#AuditLogDetailModal_ImpersonatorInfo').hide();
            }

            if (auditLog.exception) {
                $('#AuditLogDetailModal_Success').hide();
                $('#AuditLogDetailModal_Exception').show();
                $('#AuditLogDetailModal_Exception').html(auditLog.exception);
            } else {
                $('#AuditLogDetailModal_Exception').hide();
                $('#AuditLogDetailModal_Success').show();
            }

            if (auditLog.customData) {
                $('#AuditLogDetailModal_CustomData_None').hide();
                $('#AuditLogDetailModal_CustomData').show();
                $('#AuditLogDetailModal_CustomData').html(auditLog.customData);
            } else {
                $('#AuditLogDetailModal_CustomData').hide();
                $('#AuditLogDetailModal_CustomData_None').show();
            }

            $('#AuditLogDetailModal').modal('show');
        }

        $('#RefreshAuditLogsButton').click(function (e) {
            e.preventDefault();
            getAuditLogs();
        });

        $('#ExportAuditLogsToExcelButton').click(function (e) {
            e.preventDefault();
            _auditLogService.getAuditLogsToExcel(createRequestParams())
                .done(function (result) {
                    app.downloadTempFile(result);
                });
        });

        $('#ShowAdvancedFiltersSpan').click(function () {
            $('#ShowAdvancedFiltersSpan').hide();
            $('#HideAdvancedFiltersSpan').show();
            $('#AdvacedAuditFiltersArea').slideDown();
        });

        $('#HideAdvancedFiltersSpan').click(function () {
            $('#HideAdvancedFiltersSpan').hide();
            $('#ShowAdvancedFiltersSpan').show();
            $('#AdvacedAuditFiltersArea').slideUp();
        });

        _$filterForm.keydown(function (e) {
            if (e.which === 13) {
                e.preventDefault();
                getAuditLogs();
            }
        });
    });
})(jQuery);