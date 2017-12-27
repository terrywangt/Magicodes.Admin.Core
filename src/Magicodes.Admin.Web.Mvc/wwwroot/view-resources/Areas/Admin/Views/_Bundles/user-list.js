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
var OrganizationTree = (function ($) {
    return function () {
        var $tree;

        function initFiltering() {
            var to = false;
            $('#OrganizationTreeFilter').keyup(function () {
                if (to) { clearTimeout(to); }
                to = setTimeout(function () {
                    var v = $('#OrganizationTreeFilter').val();
                    $tree.jstree(true).search(v);
                }, 250);
            });
        }

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
                'search': {
                    'show_only_matches': true
                },
                plugins: ['checkbox', 'types', 'search']
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

            initFiltering();
        };

        function selectNodeAndAllParents(node) {
            $tree.jstree('select_node', node, true);
            var parent = $tree.jstree('get_parent', node);
            if (parent) {
                selectNodeAndAllParents(parent);
            }
        };

        function getSelectedOrganizations() {
            var organizationIds = [];

            var selectedOrganizations = $tree.jstree('get_selected', true);
            for (var i = 0; i < selectedOrganizations.length; i++) {
                organizationIds.push(selectedOrganizations[i].id);
            }

            return organizationIds;
        };

        return {
            init: init,
            getSelectedOrganizations: getSelectedOrganizations
        }
    }
})(jQuery);
(function ($) {
    app.modals.CreateOrEditUserModal = function () {

        var _userService = abp.services.app.user;

        var _modalManager;
        var _$userInformationForm = null;
        var _passwordComplexityHelper = new app.PasswordComplexityHelper();
        var _organizationTree;

        function _findAssignedRoleNames() {
            var assignedRoleNames = [];

            _modalManager.getModal()
                .find('.user-role-checkbox-list input[type=checkbox]')
                .each(function () {
                    if ($(this).is(':checked')) {
                        assignedRoleNames.push($(this).attr('name'));
                    }
                });

            return assignedRoleNames;
        }

        this.init = function (modalManager) {
            _modalManager = modalManager;

            _organizationTree = new OrganizationTree();
            _organizationTree.init(_modalManager.getModal().find('.organization-tree'));

            _$userInformationForm = _modalManager.getModal().find('form[name=UserInformationsForm]');
            _$userInformationForm.validate();

            var passwordInputs = _modalManager.getModal().find('input[name=Password],input[name=PasswordRepeat]');
            var passwordInputGroups = passwordInputs.closest('.form-group');

            _passwordComplexityHelper.setPasswordComplexityRules(passwordInputs, window.passwordComplexitySetting);

            $('#EditUser_SetRandomPassword').change(function () {
                if ($(this).is(':checked')) {
                    passwordInputGroups.slideUp('fast');
                    if (!_modalManager.getArgs().id) {
                        passwordInputs.removeAttr('required');
                    }
                } else {
                    passwordInputGroups.slideDown('fast');
                    if (!_modalManager.getArgs().id) {
                        passwordInputs.attr('required', 'required');
                    }
                }
            });

            _modalManager.getModal()
                .find('.user-role-checkbox-list input[type=checkbox]')
                .change(function () {
                    $('#assigned-role-count').text(_findAssignedRoleNames().length);
                });

            _modalManager.getModal().find('[data-toggle=tooltip]').tooltip();
        };

        this.save = function () {
            if (!_$userInformationForm.valid()) {
                return;
            }

            var assignedRoleNames = _findAssignedRoleNames();
            var user = _$userInformationForm.serializeFormToObject();

            if (user.SetRandomPassword) {
                user.Password = null;
            }

            _modalManager.setBusy(true);
            _userService.createOrUpdateUser({
                user: user,
                assignedRoleNames: assignedRoleNames,
                sendActivationEmail: user.SendActivationEmail,
                SetRandomPassword: user.SetRandomPassword,
                organizationUnits: _organizationTree.getSelectedOrganizations()
            }).done(function () {
                abp.notify.info(app.localize('SavedSuccessfully'));
                _modalManager.close();
                abp.event.trigger('app.createOrEditUserModalSaved');
            }).always(function () {
                _modalManager.setBusy(false);
            });
        };
    };
})(jQuery);
(function () {
    app.modals.UserPermissionsModal = function() {

        var _userService = abp.services.app.user;

        var _modalManager;
        var _permissionsTree;

        function _resetUserSpecificPermissions() {
            _modalManager.setBusy(true);
            _userService.resetUserSpecificPermissions({
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

            _permissionsTree = new PermissionsTree();
            _permissionsTree.init(_modalManager.getModal().find('.permission-tree'));

            _modalManager.getModal().find('[data-toggle=tooltip]').tooltip();

            _modalManager.getModal().find('.reset-permissions-button').click(function () {
                _resetUserSpecificPermissions();
            });
        };

        this.save = function() {
            _modalManager.setBusy(true);
            _userService.updateUserPermissions({
                id: _modalManager.getArgs().id,
                grantedPermissionNames: _permissionsTree.getSelectedPermissionNames()
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

        var _$usersTable = $('#UsersTable');
        var _userService = abp.services.app.user;

        var _permissions = {
            create: abp.auth.hasPermission('Pages.Administration.Users.Create'),
            edit: abp.auth.hasPermission('Pages.Administration.Users.Edit'),
            changePermissions: abp.auth.hasPermission('Pages.Administration.Users.ChangePermissions'),
            impersonation: abp.auth.hasPermission('Pages.Administration.Users.Impersonation'),
            'delete': abp.auth.hasPermission('Pages.Administration.Users.Delete')
        };

        var _createOrEditModal = new app.ModalManager({
            viewUrl: abp.appPath + 'Admin/Users/CreateOrEditModal',
            scriptUrl: abp.appPath + 'view-resources/Areas/Admin/Views/Users/_CreateOrEditModal.js',
            modalClass: 'CreateOrEditUserModal'
        });

        var _userPermissionsModal = new app.ModalManager({
            viewUrl: abp.appPath + 'Admin/Users/PermissionsModal',
            scriptUrl: abp.appPath + 'view-resources/Areas/Admin/Views/Users/_PermissionsModal.js',
            modalClass: 'UserPermissionsModal'
        });

        var dataTable = _$usersTable.DataTable({
            paging: true,
            serverSide: true,
            processing: true,
            listAction: {
                ajaxFunction: _userService.getUsers,
                inputFilter: function () {
                    return {
                        filter: $('#UsersTableFilter').val(),
                        permission: $("#PermissionSelectionCombo").val(),
                        role: $("#RoleSelectionCombo").val()
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
                        cssClass: 'btn btn-brand dropdown-toggle',
                        text: '<i class="fa fa-cog"></i> ' + app.localize('Actions') + ' <span class="caret"></span>',
                        items: [{
                            text: app.localize('LoginAsThisUser'),
                            visible: function (data) {
                                return _permissions.impersonation && data.record.id !== abp.session.userId;
                            },
                            action: function (data) {
                                abp.ajax({
                                    url: abp.appPath + 'Account/Impersonate',
                                    data: JSON.stringify({
                                        tenantId: abp.session.tenantId,
                                        userId: data.record.id
                                    })
                                });
                            }
                        }, {
                            text: app.localize('Edit'),
                            visible: function () {
                                return _permissions.edit;
                            },
                            action: function (data) {
                                _createOrEditModal.open({ id: data.record.id });
                            }
                        }, {
                            text: app.localize('Permissions'),
                            visible: function () {
                                return _permissions.changePermissions;
                            },
                            action: function (data) {
                                _userPermissionsModal.open({ id: data.record.id });
                            }
                        }, {
                            text: app.localize('Unlock'),
                            visible: function () {
                                return _permissions.changePermissions;
                            },
                            action: function (data) {
                                _userService.unlockUser({
                                    id: data.record.id
                                }).done(function () {
                                    abp.notify.success(app.localize('UnlockedTheUser', data.record.userName));
                                });
                            }
                        }, {
                            text: app.localize('Delete'),
                            visible: function () {
                                return _permissions.delete;
                            },
                            action: function (data) {
                                deleteUser(data.record);
                            }
                        }]
                    }
                },
                {
                    targets: 1,
                    data: "userName",
                    render: function (userName, type, row, meta) {
                        var $container = $("<span/>");
                        if (row.profilePictureId) {
                            var profilePictureUrl = "/Profile/GetProfilePictureById?id=" + row.profilePictureId;
                            var $link = $("<a/>").attr("href", profilePictureUrl).attr("target", "_blank");
                            var $img = $("<img/>")
                                .addClass("img-circle")
                                .attr("src", profilePictureUrl);

                            $link.append($img);
                            $container.append($link);
                        }

                        $container.append(userName);
                        return $container[0].outerHTML;
                    }
                },
                {
                    targets: 2,
                    data: "name"
                },
                {
                    targets: 3,
                    data: "surname"
                },
                {
                    targets: 4,
                    data: "roles",
                    render: function (roles) {
                        var roleNames = '';
                        for (var j = 0; j < roles.length; j++) {
                            if (roleNames.length) {
                                roleNames = roleNames + ', ';
                            }

                            roleNames = roleNames + roles[j].roleName;
                        };

                        return roleNames;
                    }
                },
                {
                    targets: 5,
                    data: "emailAddress"
                },
                {
                    targets: 6,
                    data: "isEmailConfirmed",
                    render: function (isEmailConfirmed) {
                        if (isEmailConfirmed) {
                            return '<span class="label label-success">' + app.localize('Yes') + '</span>';
                        } else {
                            return '<span class="label label-default">' + app.localize('No') + '</span>';
                        }
                    }
                },
                {
                    targets: 7,
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
                    targets: 8,
                    data: "lastLoginTime",
                    render: function (lastLoginTime) {
                        if (lastLoginTime) {
                            return moment(lastLoginTime).format('L');
                        }

                        return "";
                    }
                },
                {
                    targets: 9,
                    data: "creationTime",
                    render: function (creationTime) {
                        return moment(creationTime).format('L');
                    }
                }
            ]
        });


        function getUsers() {
            dataTable.ajax.reload();
        }

        function deleteUser(user) {
            if (user.userName === app.consts.userManagement.defaultAdminUserName) {
                abp.message.warn(app.localize("{0}UserCannotBeDeleted", app.consts.userManagement.defaultAdminUserName));
                return;
            }

            abp.message.confirm(
                app.localize('UserDeleteWarningMessage', user.userName),
                function (isConfirmed) {
                    if (isConfirmed) {
                        _userService.deleteUser({
                            id: user.id
                        }).done(function () {
                            getUsers(true);
                            abp.notify.success(app.localize('SuccessfullyDeleted'));
                        });
                    }
                }
            );
        }

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

        $('#CreateNewUserButton').click(function () {
            _createOrEditModal.open();
        });

        $('#ExportUsersToExcelButton').click(function () {
            _userService
                .getUsersToExcel({})
                .done(function (result) {
                    app.downloadTempFile(result);
                });
        });

        $('#GetUsersButton, #RefreshUserListButton').click(function (e) {
            e.preventDefault();
            getUsers();
        });

        $('#UsersTableFilter').on('keydown', function (e) {
            if (e.keyCode !== 13) {
                return;
            }

            e.preventDefault();
            getUsers();
        });

        abp.event.on('app.createOrEditUserModalSaved', function () {
            getUsers();
        });

        $('#UsersTableFilter').focus();
    });
})();