(function () {
    $(function () {

        var _tenantService = abp.services.app.tenant;
        var _$tenantsTable = $("#TenantsTable");
        var _$tenantsTableFilter = $('#TenantsTableFilter');
        var _$tenantsFormFilter = $('#TenantsFormFilter');
        var _$subscriptionEndDateRangeActive = $("#TenantsTable_SubscriptionEndDateRangeActive");
        var _$subscriptionEndDateRange = _$tenantsFormFilter.find("input[name='SubscriptionEndDateRange']");
        var _$creationDateRangeActive = $("#TenantsTable_CreationDateRangeActive");
        var _$creationDateRange = _$tenantsFormFilter.find("input[name='CreationDateRange']");
        var _$refreshButton = _$tenantsFormFilter.find("button[name='RefreshButton']");
        var _$editionDropdown = _$tenantsFormFilter.find("#EditionDropdown");

        var _permissions = {
            create: abp.auth.hasPermission('Pages.Tenants.Create'),
            edit: abp.auth.hasPermission('Pages.Tenants.Edit'),
            changeFeatures: abp.auth.hasPermission('Pages.Tenants.ChangeFeatures'),
            impersonation: abp.auth.hasPermission('Pages.Tenants.Impersonation'),
            'delete': abp.auth.hasPermission('Pages.Tenants.Delete')
        };

        var _urlParams = {
            creationDateStart: $.url().param('creationDateStart'),
            creationDateEnd: $.url().param('creationDateEnd'),
            subscriptionEndDateStart: $.url().param('subscriptionEndDateStart'),
            subscriptionEndDateEnd: $.url().param('subscriptionEndDateEnd')
        }

        var _selectedSubscriptionEndDateRange = {
            startDate: _urlParams.subscriptionEndDateStart ? moment(_urlParams.subscriptionEndDateStart) : moment().startOf('day'),
            endDate: _urlParams.subscriptionEndDateEnd ? moment(_urlParams.subscriptionEndDateEnd) : moment().add(30, 'days').endOf('day')
        };

        var _selectedCreationDateRange = {
            startDate: _urlParams.creationDateStart ? moment(_urlParams.creationDateStart) : moment().add(-7, 'days').startOf('day'),
            endDate: _urlParams.creationDateEnd ? moment(_urlParams.creationDateEnd) : moment().endOf('day')
        };

        _$subscriptionEndDateRange.daterangepicker(
            $.extend(true, app.createDateRangePickerOptions({
                allowFutureDate: true 
            }), _selectedSubscriptionEndDateRange),
            function (start, end, label) {
                _selectedSubscriptionEndDateRange.startDate = start; 
                _selectedSubscriptionEndDateRange.endDate = end; 
            });

        _$creationDateRange.daterangepicker(
            $.extend(true, app.createDateRangePickerOptions(), _selectedCreationDateRange),
            function (start, end, label) {
                _selectedCreationDateRange.startDate = start; 
                _selectedCreationDateRange.endDate = end;
            });

        var _createModal = new app.ModalManager({
            viewUrl: abp.appPath + "Admin/Tenants/CreateModal",
            scriptUrl: abp.appPath + "view-resources/Areas/Admin/Views/Tenants/_CreateModal.js",
            modalClass: "CreateTenantModal"
        });

        var _editModal = new app.ModalManager({
            viewUrl: abp.appPath + "Admin/Tenants/EditModal",
            scriptUrl: abp.appPath + "view-resources/Areas/Admin/Views/Tenants/_EditModal.js",
            modalClass: "EditTenantModal"
        });

        var _featuresModal = new app.ModalManager({
            viewUrl: abp.appPath + "Admin/Tenants/FeaturesModal",
            scriptUrl: abp.appPath + "view-resources/Areas/Admin/Views/Tenants/_FeaturesModal.js",
            modalClass: "TenantFeaturesModal"
        });

        var _userLookupModal = app.modals.LookupModal.create({
            title: app.localize("SelectAUser"),
            serviceMethod: abp.services.app.commonLookup.findUsers
        });

        _$tenantsTable.jtable({
            title: app.localize('Tenants'),
            paging: true,
            sorting: true,
            multiSorting: true,
            actions: {
                listAction: {
                    method: _tenantService.getTenants
                }
            },
            fields: {
                id: {
                    key: true,
                    list: false
                },
                actions: {
                    title: app.localize('Actions'),
                    width: '14%',
                    sorting: false,
                    type: 'record-actions',
                    cssClass: 'btn btn-xs btn-primary blue',
                    text: '<i class="fa fa-cog"></i> ' + app.localize('Actions') + ' <span class="caret"></span>',
                    list: _permissions.edit || _permissions.delete,

                    items: [
                        {
                            text: app.localize('LoginAsThisTenant'),
                            visible: function () {
                                return _permissions.impersonation;
                            },
                            action: function (data) {
                                _userLookupModal.open({
                                    extraFilters: {
                                        tenantId: data.record.id
                                    },
                                    title: app.localize('SelectAUser')
                                },
                                    function (selectedItem) {
                                        abp.ajax({
                                            url: abp.appPath + 'Account/Impersonate',
                                            data: JSON.stringify({
                                                tenantId: data.record.id,
                                                userId: parseInt(selectedItem.value)
                                            }),
                                            success: function () {
                                                if (!app.supportsTenancyNameInUrl) {
                                                    abp.multiTenancy.setTenantIdCookie(data.record.id);
                                                }
                                            }
                                        });
                                    });
                            }
                        }, {
                            text: app.localize('Edit'),
                            visible: function () {
                                return _permissions.edit;
                            },
                            action: function (data) {
                                _editModal.open({ id: data.record.id });
                            }
                        }, {
                            text: app.localize('Features'),
                            visible: function () {
                                return _permissions.changeFeatures;
                            },
                            action: function (data) {
                                _featuresModal.open({ id: data.record.id });
                            }
                        }, {
                            text: app.localize('Delete'),
                            visible: function () {
                                return _permissions.delete;
                            },
                            action: function (data) {
                                deleteTenant(data.record);
                            }
                        }, {
                            text: app.localize('Unlock'),
                            action: function (data) {
                                _tenantService.unlockTenantAdmin({
                                    id: data.record.id
                                }).done(function () {
                                    abp.notify.success(app.localize('UnlockedTenandAdmin', data.record.name));
                                });
                            }
                        }
                    ]
                },
                tenancyName: {
                    title: app.localize('TenancyCodeName'),
                    display: function (data) {
                        var $div = $('<div> ' + data.record.tenancyName + '</div>');
                        if (data.record.connectionString) {
                            $div.prepend($("<i class='fa fa-database' title=\"" +
                                app.localize('HasOwnDatabase') +
                                "\"></i>"));
                        }

                        return $div;
                    },
                    width: '16%'
                },
                name: {
                    title: app.localize('Name'),
                    width: '20%'
                },
                editionDisplayName: {
                    title: app.localize('Edition'),
                    width: '18%'
                },
                subscriptionEndDateUtc: {
                    title: app.localize('SubscriptionEndDateUtc'),
                    width: '15%',
                    display: function (data) {
                        if (data.record.subscriptionEndDateUtc) {
                            return moment(data.record.subscriptionEndDateUtc).format('L');
                        }

                        return "";
                    }
                },
                isActive: {
                    title: app.localize('Active'),
                    width: '8%',
                    display: function (data) {
                        if (data.record.isActive) {
                            return '<span class="label label-success">' + app.localize('Yes') + '</span>';
                        } else {
                            return '<span class="label label-default">' + app.localize('No') + '</span>';
                        }
                    }
                },
                creationTime: {
                    title: app.localize('CreationTime'),
                    width: '20%',
                    display: function (data) {
                        return moment(data.record.creationTime).format('L');
                    }
                }
            }

        });

        var getFilter = function () {
            var editionId = _$editionDropdown.find(":selected").val();

            var filter = {
                filter: _$tenantsTableFilter.val(),
                editionId: editionId,
                editionIdSpecified: editionId !== "-1"
            };

            if (_$creationDateRangeActive.prop("checked")) {
                filter.creationDateStart = _selectedCreationDateRange.startDate;
                filter.creationDateEnd = _selectedCreationDateRange.endDate;
            }

            if (_$subscriptionEndDateRangeActive.prop("checked")) {
                filter.subscriptionEndDateStart = _selectedSubscriptionEndDateRange.startDate;
                filter.subscriptionEndDateEnd = _selectedSubscriptionEndDateRange.endDate;
            }

            return filter;
        };

        function getTenants(reload) {
            if (reload) {
                _$tenantsTable.jtable("reload");
            } else {
                _$tenantsTable.jtable("load", getFilter());
            }
        }

        function deleteTenant(tenant) {
            abp.message.confirm(
                app.localize("TenantDeleteWarningMessage", tenant.tenancyName),
                function (isConfirmed) {
                    if (isConfirmed) {
                        _tenantService.deleteTenant({
                            id: tenant.id
                        }).done(function () {
                            getTenants();
                            abp.notify.success(app.localize("SuccessfullyDeleted"));
                        });
                    }
                }
            );
        }

        $('#CreateNewTenantButton').click(function () {
            _createModal.open();
        });

        $('#GetTenantsButton').click(function (e) {
            e.preventDefault();
            getTenants();
        });

        abp.event.on("app.editTenantModalSaved",
            function () {
                getTenants(true);
            });

        abp.event.on("app.createTenantModalSaved",
            function () {
                getTenants(true);
            });

        _$subscriptionEndDateRangeActive.change(function () {
            _$subscriptionEndDateRange.prop("disabled", !$(this).prop("checked"));
        });

        if (_urlParams.subscriptionEndDateStart || _urlParams.subscriptionEndDateEnd) {
            _$subscriptionEndDateRangeActive.prop("checked", true);
        } else {
            _$subscriptionEndDateRange.prop("disabled", true);
        }

        _$creationDateRangeActive.change(function () {
            _$creationDateRange.prop("disabled", !$(this).prop("checked"));
        });

        if (_urlParams.creationDateStart || _urlParams.creationDateEnd) {
            _$creationDateRangeActive.prop("checked", true);
        } else {
            _$creationDateRange.prop("disabled", true);
        }

        _$refreshButton.click(function (e) {
            e.preventDefault();
            getTenants();
        });

        _$tenantsTableFilter.focus();
        getTenants();
    });
})();