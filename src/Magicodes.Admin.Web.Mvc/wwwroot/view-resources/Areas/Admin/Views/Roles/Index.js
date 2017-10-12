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
                                    .addClass("label label-info")
                                    .attr("data-toggle", "tooltip")
                                    .attr("title", app.localize('StaticRole_Tooltip'))
                                    .attr("data-placement", "top")
                                    .text(app.localize('Static'))
                                    .css("margin-right", "5px")
                            );
                        }

                        if (row.isDefault) {
                            $span.append(
                                $("<span/>")
                                    .addClass("label label-default")
                                    .attr("data-toggle", "tooltip")
                                    .attr("title", app.localize('DefaultRole_Description'))
                                    .attr("data-placement", "top")
                                    .text(app.localize('Default'))
                                    .css("margin-right", "5px")
                            );
                        }

                        $span.find('[data-toggle=tooltip]').tooltip();
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