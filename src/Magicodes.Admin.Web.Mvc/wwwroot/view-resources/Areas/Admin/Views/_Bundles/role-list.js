var PermissionsTree = (function ($) {
    return function() {
        var $tree;

        function init($treeContainer) {
            $tree = $treeContainer;
            $tree.jstree({
                "types": {
                    "default": {
                        "icon": "fa fa-folder m--font-warning"
                    },
                    "file": {
                        "icon": "fa fa-file  m--font-warning"
                    }
                },
                'checkbox': {
                    keep_selected_style: false,
                    three_state: false,
                    cascade: ''
                },
                plugins: ['checkbox', 'types']
            });

            $tree.on("changed.jstree", function (e, data) {
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
            });
        };

        function selectNodeAndAllParents(node) {
            $tree.jstree('select_node', node, true);
            var parent = $tree.jstree('get_parent', node);
            if (parent) {
                selectNodeAndAllParents(parent);
            }
        };

        function getSelectedPermissionNames() {
            var permissionNames = [];

            var selectedPermissions = $tree.jstree('get_selected', true);
            for (var i = 0; i < selectedPermissions.length; i++) {
                permissionNames.push(selectedPermissions[i].id);
            }

            return permissionNames;
        };

        return {
            init: init,
            getSelectedPermissionNames: getSelectedPermissionNames
        }
    }
})(jQuery);
(function () {
    app.modals.CreateOrEditRoleModal = function () {

        var _modalManager;
        var _roleService = abp.services.app.role;
        var _$roleInformationForm = null;
        var _permissionsTree;

        this.init = function (modalManager) {
            _modalManager = modalManager;

            _permissionsTree = new PermissionsTree();
            _permissionsTree.init(_modalManager.getModal().find('.permission-tree'));

            _$roleInformationForm = _modalManager.getModal().find('form[name=RoleInformationsForm]');
            _$roleInformationForm.validate({ ignore: "" });
        };

        this.save = function() {
            if (!_$roleInformationForm.valid()) {
                return;
            }

            var role = _$roleInformationForm.serializeFormToObject();

            _modalManager.setBusy(true);
            _roleService.createOrUpdateRole({
                role: role,
                grantedPermissionNames: _permissionsTree.getSelectedPermissionNames()
            }).done(function () {
                abp.notify.info(app.localize('SavedSuccessfully'));
                _modalManager.close();
                abp.event.trigger('app.createOrEditRoleModalSaved');
            }).always(function () {
                _modalManager.setBusy(false);
            });
        };
    };
})();
(function () {
    $(function () {

        var _$rolesTable = $('#RolesTable');
        var _roleService = abp.services.app.role;

        var _permissions = {
            create: abp.auth.hasPermission('Pages.Administration.Roles.Create'),
            edit: abp.auth.hasPermission('Pages.Administration.Roles.Edit'),
            'delete': abp.auth.hasPermission('Pages.Administration.Roles.Delete')
        };

        var _createOrEditModal = new app.ModalManager({
            viewUrl: abp.appPath + 'Admin/Roles/CreateOrEditModal',
            scriptUrl: abp.appPath + 'view-resources/Areas/Admin/Views/Roles/_CreateOrEditModal.js',
            modalClass: 'CreateOrEditRoleModal'
        });

        var dataTable = _$rolesTable.DataTable({
            paging: false,
            serverSide: false,
            processing: false,
            drawCallback: function (settings) {
                $('[data-toggle=m-tooltip]').tooltip();
            },
            listAction: {
                ajaxFunction: _roleService.getRoles,
                inputFilter: function () {
                    return {
                        permission: $('#PermissionSelectionCombo').val()
                    };
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
                        items: [{
                            text: app.localize('Edit'),
                            visible: function () {
                                return _permissions.edit;
                            },
                            action: function (data) {
                                _createOrEditModal.open({ id: data.record.id });
                            }
                        }, {
                            text: app.localize('Delete'),
                            visible: function (data) {
                                return !data.record.isStatic && _permissions.delete;
                            },
                            action: function (data) {
                                deleteRole(data.record);
                            }
                        }]
                    }
                },
                {
                    targets: 1,
                    data: "displayName",
                    render: function (displayName, type, row, meta) {
                        var $span = $('<span/>');
                        $span.append(displayName + " &nbsp;");

                        if (row.isStatic) {
                            $span.append(
                                $("<span/>")
                                    .addClass("m-badge m-badge--brand m-badge--wide")
                                    .attr("data-toggle", "m-tooltip")
                                    .attr("title", app.localize('StaticRole_Tooltip'))
                                    .attr("data-placement", "top")
                                    .text(app.localize('Static'))
                                    .css("margin-right", "5px")
                            );
                        }

                        if (row.isDefault) {
                            $span.append(
                                $("<span/>")
                                    .addClass("m-badge m-badge--metal m-badge--wide")
                                    .attr("data-toggle", "m-tooltip")
                                    .attr("title", app.localize('DefaultRole_Description'))
                                    .attr("data-placement", "top")
                                    .text(app.localize('Default'))
                                    .css("margin-right", "5px")
                            );
                        }

                        return $span[0].outerHTML;
                    }
                },
                {
                    targets: 2,
                    data: "creationTime",
                    render: function (creationTime) {
                        return moment(creationTime).format('L');
                    }
                }
            ]
        });

        function deleteRole(role) {
            abp.message.confirm(
                app.localize('RoleDeleteWarningMessage', role.displayName),
                function (isConfirmed) {
                    if (isConfirmed) {
                        _roleService.deleteRole({
                            id: role.id
                        }).done(function () {
                            getRoles();
                            abp.notify.success(app.localize('SuccessfullyDeleted'));
                        });
                    }
                }
            );
        };

        $('#CreateNewRoleButton').click(function () {
            _createOrEditModal.open();
        });

        $('#RefreshRolesButton').click(function (e) {
            e.preventDefault();
            getRoles();
        });

        function getRoles() {
            dataTable.ajax.reload();
        }

        abp.event.on('app.createOrEditRoleModalSaved', function () {
            getRoles();
        });

    });
})();