(function () {
    app.modals.LookupModal = function () {

        var _modalManager;
        var _dataTable;
        var _$table;
        var _$filterInput;

        var _options = {
            serviceMethod: null, //Required
            title: app.localize('SelectAnItem'),
            loadOnStartup: true,
            showFilter: true,
            filterText: '',
            pageSize: app.consts.grid.defaultPageSize,
            canSelect: function (item) {
                /* This method can return boolean or a promise which returns boolean.
                 * A false value is used to prevent selection.
                 */
                return true;
            }
        };

        function refreshTable() {
            _dataTable.ajax.reload();
        };

        function selectItem(item) {
            var boolOrPromise = _options.canSelect(item);
            if (!boolOrPromise) {
                return;
            }

            if (boolOrPromise === true) {
                _modalManager.setResult(item);
                _modalManager.close();
                return;
            }

            //assume as promise
            boolOrPromise.then(function (result) {
                if (result) {
                    _modalManager.setResult(item);
                    _modalManager.close();
                }
            });
        }

        this.init = function (modalManager) {
            _modalManager = modalManager;
            _options = $.extend(_options, _modalManager.getOptions().lookupOptions);
            _$table = _modalManager.getModal().find('.lookup-modal-table');

            _dataTable = _$table.DataTable({
                paging: true,
                serverSide: true,
                processing: true,
                lengthChange: false,
                pageLength: _options.pageSize,
                deferLoading: _options.loadOnStartup ? null : 0,
                listAction: {
                    ajaxFunction: _options.serviceMethod,
                    inputFilter: function () {
                        return $.extend({
                            filter: _$filterInput.val()
                        }, _modalManager.getArgs().extraFilters);
                    }
                },
                columnDefs: [
                    {
                        targets: 0,
                        data: null,
                        orderable: false,
                        defaultContent: '',
                        className:'text-center',
                        rowAction: {
                            element: $("<button/>")
                                .addClass("btn btn-metal m-btn m-btn--icon m-btn--icon-only")
                                .attr("title", app.localize('Select'))
                                .append($("<i/>").addClass("la la-chevron-circle-right")).click(function () {
                                    var record = $(this).data();
                                    selectItem(record);
                                })
                        }
                    },
                    {
                        targets: 1,
                        data: "name"
                    }
                ]
            });

            _modalManager.getModal()
                .find('.lookup-filter-button')
                .click(function (e) {
                    e.preventDefault();
                    refreshTable();
                });

            _modalManager.getModal()
                .find('.modal-body')
                .keydown(function (e) {
                    if (e.which === 13) {
                        e.preventDefault();
                        refreshTable();
                    }
                });

            _$filterInput = _modalManager.getModal().find('.lookup-filter-text');
            _$filterInput.val(_options.filterText);
        };
    };

    app.modals.LookupModal.create = function (lookupOptions) {
        return new app.ModalManager({
            viewUrl: abp.appPath + 'Admin/Common/LookupModal',
            scriptUrl: abp.appPath + 'view-resources/Areas/Admin/Views/Common/Modals/_LookupModal.js',
            modalClass: 'LookupModal',
            lookupOptions: lookupOptions
        });
    };
})();