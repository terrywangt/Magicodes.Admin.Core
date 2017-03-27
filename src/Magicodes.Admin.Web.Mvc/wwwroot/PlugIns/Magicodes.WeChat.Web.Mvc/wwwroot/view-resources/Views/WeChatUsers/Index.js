(function () {
    $(function () {

        var _$weChatUsersTable = $('#WeChatUsersTable');
        var _weChatUsersService = abp.services.wechat.weChatUser;

        var _permissions = {
            create: abp.auth.hasPermission('Pages.Administration.WeChatUsers.Create'),
            edit: abp.auth.hasPermission('Pages.Administration.WeChatUsers.Edit'),
            'delete': abp.auth.hasPermission('Pages.Administration.WeChatUsers.Delete')
        };

        var _createOrEditModal = new app.ModalManager({
            viewUrl: abp.appPath + 'PlugIns/Magicodes.WeChat.Web.Mvc/Views/WeChatUsers/CreateOrEditModal',
            scriptUrl: abp.appPath + 'PlugIns/Magicodes.WeChat.Web.Mvc/wwwroot/view-resources/Areas/Admin/Views/WeChatUsers/_CreateOrEditModal.js',
            modalClass: 'CreateOrEditRoleModal'
        });

        _$weChatUsersTable.jtable({

            title: app.localize('WeChatUsers'),
            paging: true,
            sorting: true,
            multiSorting: true,

            actions: {
                listAction: {
                    method: _weChatUsersService.getWeChatUsers
                }
            },

            fields: {
                id: {
                    key: true,
                    list: false
                },
                actions: {
                    title: app.localize('Actions'),
                    width: '30%',
                    display: function (data) {
                        var $span = $('<span></span>');

                        if (_permissions.edit) {
                            $('<button class="btn btn-default btn-xs" title="' + app.localize('Edit') + '"><i class="fa fa-edit"></i></button>')
                                .appendTo($span)
                                .click(function () {
                                    _createOrEditModal.open({ id: data.record.id });
                                });
                        }

                        if (!data.record.isStatic && _permissions.delete) {
                            $('<button class="btn btn-default btn-xs" title="' + app.localize('Delete') + '"><i class="fa fa-trash-o"></i></button>')
                                .appendTo($span)
                                .click(function () {
                                    deleteRole(data.record);
                                });
                        }

                        return $span;
                    }
                },
                nickName: {
                    title: "昵称",
                    width: '35%',
                    display: function (data) {
                        var $span = $('<span></span>');

                        $span.append(data.record.displayName + " &nbsp; ");

                        if (data.record.isStatic) {
                            $span.append('<span class="label label-info" data-toggle="tooltip" title="' + app.localize('StaticRole_Tooltip') + '" data-placement="top">' + app.localize('Static') + '</span>&nbsp;');
                        }

                        if (data.record.isDefault) {
                            $span.append('<span class="label label-default" data-toggle="tooltip" title="' + app.localize('DefaultRole_Description') + '" data-placement="top">' + app.localize('Default') + '</span>&nbsp;');
                        }

                        $span.find('[data-toggle=tooltip]').tooltip();

                        return $span;
                    }
                },
                creationTime: {
                    title: app.localize('CreationTime'),
                    width: '35%',
                    display: function (data) {
                        return moment(data.record.creationTime).format('L');
                    }
                }
            }

        });

        function deleteRole(weChatUsers) {
            abp.message.confirm(
                app.localize('RoleDeleteWarningMessage', weChatUsers.displayName),
                function (isConfirmed) {
                    if (isConfirmed) {
                        _weChatUsersService.deleteRole({
                            id: weChatUsers.id
                        }).done(function () {
                            getWeChatUsers();
                            abp.notify.success(app.localize('SuccessfullyDeleted'));
                        });
                    }
                }
            );
        };

        function getWeChatUsers() {
            _$weChatUsersTable.jtable('load', { filter: '' });
        }

        $('#RefreshWeChatUsersButton').click(function (e) {
            e.preventDefault();
        });



        abp.event.on('app.createOrEditRoleModalSaved', function () {

        });
        getWeChatUsers();
    });
})();