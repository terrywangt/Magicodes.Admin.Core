var FeaturesTree = (function ($) {
    return function() {
        var $tree;

        function init($treeContainer) {
            $tree = $treeContainer;

            $tree.on('ready.jstree', function() {
                    customizeTreeNodes();
                })
                .on('redraw.jstree', function() {
                    customizeTreeNodes();
                })
                .on('after_open.jstree', function() {
                    customizeTreeNodes();
                })
                .on('create_node.jstree', function() {
                    customizeTreeNodes();
                }).on("changed.jstree", function(e, data) {
                    if (!data.node) {
                        return;
                    }

                    var childrenNodes;

                    if (data.node.state.selected) {
                        selectNodeAndAllParents($tree.jstree('get_parent', data.node));

                        childrenNodes = $.makeArray($tree.jstree('get_children_dom', data.node));
                        $tree.jstree('select_node', childrenNodes);

                    } else {
                        childrenNodes = $.makeArray($tree.jstree('get_children_dom', data.node));
                        $tree.jstree('deselect_node', childrenNodes);
                    }
                }).jstree({
                    "types": {
                        "default": {
                            "icon": "fa fa-folder tree-item-icon-color icon-lg"
                        },
                        "file": {
                            "icon": "fa fa-file tree-item-icon-color icon-lg"
                        }
                    },
                    'checkbox': {
                        keep_selected_style: false,
                        three_state: false,
                        cascade: ''
                    },
                    plugins: ['checkbox', 'types']
                });

            function customizeTreeNodes() {
                $tree.find('.jstree-node').each(function () {
                    var $nodeLi = $(this);
                    var $nodeA = $nodeLi.find('.jstree-anchor');
                    var feature = JSON.parse($nodeLi.attr('data-feature'));
                    var featureValue = $nodeLi.attr('data-feature-value');

                    if (!feature || !feature.inputType) {
                        return;
                    }

                    if (feature.inputType.name == 'CHECKBOX') {
                        //no change for checkbox
                    } else if (feature.inputType.name == 'SINGLE_LINE_STRING') {
                        if (!$nodeLi.find('.feature-tree-textbox').length) {
                            $nodeA.find('.jstree-checkbox').hide();

                            var inputType = 'text';
                            if (feature.inputType.validator) {
                                if (feature.inputType.validator.name == 'NUMERIC') {
                                    inputType = 'number';
                                }
                            }

                            var $textbox = $('<input class="feature-tree-textbox" type="' + inputType + '" />')
                                .val(featureValue);

                            if (inputType == 'number') {
                                $textbox.attr('min', feature.inputType.validator.minValue);
                                $textbox.attr('max', feature.inputType.validator.maxValue);
                            } else {
                                if (feature.inputType.validator && feature.inputType.validator.name == 'STRING') {
                                    if (feature.inputType.validator.maxLength > 0) {
                                        $textbox.attr('maxlength', feature.inputType.validator.maxLength);
                                    }
                                    if (feature.inputType.validator.minLength > 0) {
                                        $textbox.attr('required', 'required');
                                    }
                                    if (feature.inputType.validator.regularExpression) {
                                        $textbox.attr('pattern', feature.inputType.validator.regularExpression);
                                    }
                                }
                            }

                            $textbox.on('input propertychange paste', function () {
                                if (isFeatureValueValid(feature, $textbox.val())) {
                                    $nodeLi.attr('data-feature-value', $textbox.val());
                                    $textbox.removeClass('feature-tree-textbox-invalid');
                                } else {
                                    $textbox.addClass('feature-tree-textbox-invalid');
                                }
                            });

                            $textbox.appendTo($nodeLi);
                        }
                    } else if (feature.inputType.name == 'COMBOBOX') {
                        if (!$nodeLi.find('.feature-tree-combobox').length) {
                            $nodeA.find('.jstree-checkbox').hide();

                            var $combobox = $('<select class="feature-tree-combobox" />');
                            _.each(feature.inputType.itemSource.items, function (opt) {
                                $('<option></option>')
                                    .attr('value', opt.value)
                                    .text(opt.displayText)
                                    .appendTo($combobox);
                            });

                            $combobox
                                .val(featureValue)
                                .on('change', function () {
                                    $nodeLi.attr('data-feature-value', $combobox.val());
                                })
                                .appendTo($nodeLi);
                        }
                    }
                });
            }
        };

        function selectNodeAndAllParents(node) {
            $tree.jstree('select_node', node, true);
            var parent = $tree.jstree('get_parent', node);
            if (parent) {
                selectNodeAndAllParents(parent);
            }
        };
        
        function isFeatureValueValid(feature, value) {
            if (!feature || !feature.inputType || !feature.inputType.validator) {
                return true;
            }

            var validator = feature.inputType.validator;
            if (validator.name == 'STRING') {
                if (value == undefined || value == null) {
                    return validator.allowNull;
                }

                if (typeof value != 'string') {
                    return false;
                }

                if (validator.minLength > 0 && value.length < validator.minLength) {
                    return false;
                }

                if (validator.maxLength > 0 && value.length > validator.maxLength) {
                    return false;
                }

                if (validator.regularExpression) {
                    return (new RegExp(validator.regularExpression)).test(value);
                }
            } else if (validator.name == 'NUMERIC') {
                var numValue = parseInt(value);

                if (isNaN(numValue)) {
                    return false;
                }

                var minValue = validator.minValue;
                if (minValue > numValue) {
                    return false;
                }

                var maxValue = validator.maxValue;
                if (maxValue > 0 && numValue > maxValue) {
                    return false;
                }
            }

            return true;
        }

        function isValid() {
            return $tree.find('.feature-tree-textbox-invalid').length <= 0;
        }

        function getFeatureValues() {
            var featureValues = [];

            $tree.find('.jstree-node').each(function() {
                var $nodeLi = $(this);
                var feature = JSON.parse($nodeLi.attr('data-feature'));
                if (!feature.inputType || feature.inputType.name == 'CHECKBOX') {
                    featureValues.push({
                        name: feature.name,
                        value: $tree.jstree('is_checked', $nodeLi) ? 'true' : 'false'
                    });
                } else {
                    featureValues.push({
                        name: feature.name,
                        value: $nodeLi.attr('data-feature-value')
                    });
                }
            });

            return featureValues;
        };

        return {
            init: init,
            getFeatureValues: getFeatureValues,
            isValid: isValid
        }
    }
})(jQuery);
(function ($) {
    //Custom validation type for tenancy name
    $.validator.addMethod("tenancyNameRegex", function (value, element, regexpr) {
        return regexpr.test(value);
    }, app.localize('TenantName_Regex_Description'));

    app.modals.CreateTenantModal = function () {
        var _tenantService = abp.services.app.tenant;
        var _$tenantInformationForm = null;
        var _passwordComplexityHelper = new app.PasswordComplexityHelper();

        var _modalManager;

        this.init = function (modalManager) {
            _modalManager = modalManager;
            var modal = _modalManager.getModal();

            _$tenantInformationForm = modal.find('form[name=TenantInformationsForm]');
            _$tenantInformationForm.validate({
                rules: {
                    TenancyName: {
                        tenancyNameRegex: new RegExp(_$tenantInformationForm.find('input[name=TenancyName]').attr('regex'))
                    }
                }
            });

            //Show/Hide password inputs when "random password" checkbox is changed.

            var passwordInputs = modal.find('input[name=AdminPassword],input[name=AdminPasswordRepeat]');
            var passwordInputGroups = passwordInputs.closest('.form-group');

            _passwordComplexityHelper.setPasswordComplexityRules(passwordInputs, window.passwordComplexitySetting);

            $('#CreateTenant_SetRandomPassword').change(function () {
                if ($(this).is(':checked')) {
                    passwordInputGroups.slideUp('fast');
                    passwordInputs.removeAttr('required');
                } else {
                    passwordInputGroups.slideDown('fast');
                    passwordInputs.attr('required', 'required');
                }
            });

            //Show/Hide connection string input when "use host db" checkbox is changed.

            var connStringInput = modal.find('input[name=ConnectionString]');
            var connStringInputGroup = connStringInput.closest('.form-group');

            $('#CreateTenant_UseHostDb').change(function () {
                if ($(this).is(':checked')) {
                    connStringInputGroup.slideUp('fast');
                    connStringInput.removeAttr('required');
                } else {
                    connStringInputGroup.slideDown('fast');
                    connStringInput.attr('required', 'required');
                }
            });

            modal.find('.date-time-picker').datetimepicker({
                locale: abp.localization.currentLanguage.name,
                format: 'L'
            });

            var $subscriptionEndDateDiv = modal.find('input[name=SubscriptionEndDateUtc]').parent('div');
            var isUnlimitedInput = modal.find('#CreateTenant_IsUnlimited');
            var subscriptionEndDateUtcInput = modal.find('input[name=SubscriptionEndDateUtc]');
            function toggleSubscriptionEndDateDiv() {
                if (isUnlimitedInput.is(':checked')) {
                    $subscriptionEndDateDiv.slideUp('fast');
                    subscriptionEndDateUtcInput.removeAttr('required');
                } else {
                    $subscriptionEndDateDiv.slideDown('fast');
                    subscriptionEndDateUtcInput.attr('required', 'required');
                }
            }

            isUnlimitedInput.change(function () {
                toggleSubscriptionEndDateDiv();
            });

            toggleSubscriptionEndDateDiv();

            var $editionCombobox = modal.find('#EditionId');
            var $isInTrialCheckbox = modal.find('#CreateTenant_IsInTrialPeriod');
            $editionCombobox.change(function () {

                var isFree = $('option:selected', this).attr('data-isfree') === "True";
                var selectedValue = $('option:selected', this).val();
                if (isFree) {
                    $isInTrialCheckbox.closest('div').slideUp('fast');
                } else {
                    $isInTrialCheckbox.closest('div').slideDown('fast');
                }

                if (selectedValue <= 0) {
                    modal.find('.subscription-component').slideUp('fast');
                } else {
                    modal.find('.subscription-component').slideDown('fast');
                }
            });

            $editionCombobox.trigger('change');
        };

        this.save = function () {
            if (!_$tenantInformationForm.valid()) {
                return;
            }

            var tenant = _$tenantInformationForm.serializeFormToObject();

            //take selected date as UTC
            if ($('#CreateTenant_IsUnlimited').is(':visible') && !$('#CreateTenant_IsUnlimited').is(':checked')) {
                tenant.SubscriptionEndDateUtc = $('.date-time-picker').data("DateTimePicker").date().format("YYYY-MM-DDTHH:mm:ss") + 'Z';
            } else {
                tenant.SubscriptionEndDateUtc = null;
            }

            if (tenant.SetRandomPassword) {
                tenant.Password = null;
            }

            if (tenant.UseHostDb) {
                tenant.ConnectionString = null;
            }

            _modalManager.setBusy(true);
            _tenantService.createTenant(
                tenant
            ).done(function () {
                abp.notify.info(app.localize('SavedSuccessfully'));
                _modalManager.close();
                abp.event.trigger('app.createTenantModalSaved');
            }).always(function () {
                _modalManager.setBusy(false);
            });
        };
    };
})(jQuery);
var EditTenantModal = (function ($) {
    app.modals.EditTenantModal = function () {

        var _modalManager;
        var _tenantService = abp.services.app.tenant;
        var _$tenantInformationForm = null;


        this.init = function (modalManager) {
            _modalManager = modalManager;
            var modal = _modalManager.getModal();

            _$tenantInformationForm = modal.find('form[name=TenantInformationsForm]');
            _$tenantInformationForm.validate();

            modal.find('.date-time-picker').datetimepicker({
                locale: abp.localization.currentLanguage.name,
                format: 'L'
            });

            var $subscriptionEndDateDiv = modal.find('input[name=SubscriptionEndDateUtc]').parent('div');
            var isUnlimitedInput = modal.find('#CreateTenant_IsUnlimited');
            var subscriptionEndDateUtcInput = modal.find('input[name=SubscriptionEndDateUtc]');
            function toggleSubscriptionEndDateDiv() {
                if (isUnlimitedInput.is(':checked')) {
                    $subscriptionEndDateDiv.slideUp('fast');
                    subscriptionEndDateUtcInput.removeAttr('required');
                } else {
                    $subscriptionEndDateDiv.slideDown('fast');
                    subscriptionEndDateUtcInput.attr('required', 'required');
                }
            }

            isUnlimitedInput.change(function () {
                toggleSubscriptionEndDateDiv();
            });

            toggleSubscriptionEndDateDiv();

            var $editionCombobox = modal.find('#EditionId');
            var $isInTrialCheckbox = modal.find('#EditTenant_IsInTrialPeriod');
            $editionCombobox.change(function () {
                var isFree = $('option:selected', this).attr('data-isfree') === "True";
                var selectedValue = $('option:selected', this).val();
                if (isFree) {
                    $isInTrialCheckbox.closest('div').slideUp('fast');
                } else {
                    $isInTrialCheckbox.closest('div').slideDown('fast');
                }

                if (selectedValue <= 0) {
                    modal.find('.subscription-component').slideUp('fast');
                } else {
                    modal.find('.subscription-component').slideDown('fast');
                }
            });

            $editionCombobox.trigger('change');
        };

        this.save = function () {
            if (!_$tenantInformationForm.valid()) {
                return;
            }

            var tenant = _$tenantInformationForm.serializeFormToObject();
            
            //take selected date as UTC
            if ($('#CreateTenant_IsUnlimited').is(':visible') && !$('#CreateTenant_IsUnlimited').is(':checked')) {
                tenant.SubscriptionEndDateUtc = $('.date-time-picker').data("DateTimePicker").date().format("YYYY-MM-DDTHH:mm:ss") + 'Z';    
            } else {
                tenant.SubscriptionEndDateUtc = null;
            }

            _modalManager.setBusy(true);
            _tenantService.updateTenant(
                tenant
            ).done(function () {
                abp.notify.info(app.localize('SavedSuccessfully'));
                _modalManager.close();
                abp.event.trigger('app.editTenantModalSaved');
            }).always(function () {
                _modalManager.setBusy(false);
            });
        };
    };
})(jQuery);
(function () {
    app.modals.TenantFeaturesModal = function() {

        var _tenantService = abp.services.app.tenant;

        var _modalManager;
        var _featuresTree;

        function _resetTenantSpecificFeatures() {
            _modalManager.setBusy(true);
            _tenantService.resetTenantSpecificFeatures({
                id: _modalManager.getArgs().id
            }).done(function () {
                abp.notify.info(app.localize('ResetSuccessfully'));
                _modalManager.getModal().on('hidden.bs.modal', function (e) {
                    _modalManager.reopen();
                });
                _modalManager.close();
            }).always(function () {
                _modalManager.setBusy(false);
            });
        }

        this.init = function(modalManager) {
            _modalManager = modalManager;

            _featuresTree = new FeaturesTree();
            _featuresTree.init(_modalManager.getModal().find('.feature-tree'));

            _modalManager.getModal().find('[data-toggle=tooltip]').tooltip();

            _modalManager.getModal().find('.reset-features-button').click(function () {
                _resetTenantSpecificFeatures();
            });
        };

        this.save = function () {
            if (!_featuresTree.isValid()) {
                abp.message.warn(app.localize('InvalidFeaturesWarning'));
                return;
            }

            _modalManager.setBusy(true);
            _tenantService.updateTenantFeatures({
                id: _modalManager.getArgs().id,
                featureValues: _featuresTree.getFeatureValues()
            }).done(function () {
                abp.notify.info(app.localize('SavedSuccessfully'));
                _modalManager.close();
            }).always(function () {
                _modalManager.setBusy(false);
            });
        };
    };
})();
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

        var dataTable = _$tenantsTable.DataTable({
            paging: true,
            serverSide: true,
            processing: true,
            listAction: {
                ajaxFunction: _tenantService.getTenants,
                inputFilter: function () {
                    return getFilter();
                }
            },
            columnDefs: [
                {
                    targets: 0,
                    data: null,
                    orderable: false,
                    autoWidth: false,
                    defaultContent: '',
                    rowAction: {
                        cssClass: 'btn btn-xs btn-primary blue',
                        text: '<i class="fa fa-cog"></i> ' + app.localize('Actions') + ' <span class="caret"></span>',
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
                            },
                            {
                                text: app.localize('Edit'),
                                visible: function () {
                                    return _permissions.edit;
                                },
                                action: function (data) {
                                    _editModal.open({ id: data.record.id });
                                }
                            },
                            {
                                text: app.localize('Features'),
                                visible: function () {
                                    return _permissions.changeFeatures;
                                },
                                action: function (data) {
                                    _featuresModal.open({ id: data.record.id });
                                }
                            },
                            {
                                text: app.localize('Delete'),
                                visible: function () {
                                    return _permissions.delete;
                                },
                                action: function (data) {
                                    deleteTenant(data.record);
                                }
                            },
                            {
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
                    }
                },
                {
                    targets: 1,
                    data: "tenancyName"
                },
                {
                    targets: 2,
                    data: "name"
                },
                {
                    targets: 3,
                    data: "editionDisplayName"
                },
                {
                    targets: 4,
                    data: "subscriptionEndDateUtc",
                    render: function (subscriptionEndDateUtc) {
                        if (subscriptionEndDateUtc) {
                            return moment(subscriptionEndDateUtc).format('L');
                        }

                        return "";
                    }
                },
                {
                    targets: 5,
                    data: "isActive",
                    render: function (isActive) {
                        if (isActive) {
                            return '<span class="label label-success">' + app.localize('Yes') + '</span>';
                        } else {
                            return '<span class="label label-default">' + app.localize('No') + '</span>';
                        }
                    }
                },
                {
                    targets: 6,
                    data: "creationTime",
                    render: function (creationTime) {
                        return moment(creationTime).format('L');
                    }
                }
            ]
        });

        function getTenants() {
            dataTable.ajax.reload();
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
    });
})();