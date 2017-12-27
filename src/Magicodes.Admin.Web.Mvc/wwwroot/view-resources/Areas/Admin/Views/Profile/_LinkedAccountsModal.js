(function ($) {
    app.modals.LinkedAccountsModal = function () {

        var _modalManager;
        var _$linkedAccountsTable = $('#LinkedAccountsTable');
        var _userLinkService = abp.services.app.userLink;

        this.init = function (modalManager) {
            _modalManager = modalManager;
        };

        var _linkNewAccountModal = new app.ModalManager({
            viewUrl: abp.appPath + 'Admin/Profile/LinkAccountModal',
            scriptUrl: abp.appPath + 'view-resources/Areas/Admin/Views/Profile/_LinkAccountModal.js',
            modalClass: 'LinkAccountModal'
        });

        $('#LinkNewAccountButton').click(function () {
            _linkNewAccountModal.open({}, function () {
                getLinkedUsers();
            });
        });

        var dataTable = _$linkedAccountsTable.DataTable({
            paging: true,
            serverSide: true,
            processing: true,
            listAction: {
                ajaxFunction: _userLinkService.getLinkedUsers
            },
            columnDefs: [
                {
                    targets: 0,
                    data: null,
                    orderable: false,
                    defaultContent: '',
                    rowAction: {
                        element: $("<button/>")
                            .addClass("btn btn-primary btn-sm m-btn--icon")
                            .text(app.localize('LogIn'))
                            .prepend($("<i/>").addClass("la la-sign-in"))
                            .click(function () {
                                switchToUser($(this).data());
                            })
                    }
                },
                {
                    targets: 1,
                    data: "username",
                    orderable: false,
                    render: function (userName, type, row, meta) {
                        return $('<div/>').append($("<span/>").text(app.getShownLinkedUserName(row)))[0].outerHTML;
                    }
                },
                {
                    targets: 2,
                    data: null,
                    orderable: false,
                    defaultContent: '',
                    rowAction: {
                        element: $("<button/>")
                            .addClass("btn btn-outline-danger m-btn m-btn--icon btn-sm m-btn--icon-only m-btn--pill m-btn--air")
                            .attr("title", app.localize('Delete'))
                            .append($("<i/>").addClass("la la-trash"))
                            .click(function () {
                                deleteLinkedUser($(this).data());
                            })
                    }
                }
            ]
        });

        function switchToUser(linkedUser) {
            abp.ajax({
                url: abp.appPath + 'Account/SwitchToLinkedAccount',
                data: JSON.stringify({
                    targetUserId: linkedUser.id,
                    targetTenantId: linkedUser.tenantId
                }),
                success: function () {
                    if (!app.supportsTenancyNameInUrl) {
                        abp.multiTenancy.setTenantIdCookie(linkedUser.tenantId);
                    }
                }
            });
        }

        function deleteLinkedUser(linkedUser) {
            abp.message.confirm(
                app.localize('LinkedUserDeleteWarningMessage', linkedUser.username),
                function (isConfirmed) {
                    if (isConfirmed) {
                        _userLinkService.unlinkUser({
                            userId: linkedUser.id,
                            tenantId: linkedUser.tenantId
                        }).done(function () {
                            getLinkedUsers();
                            abp.notify.success(app.localize('SuccessfullyUnlinked'));
                        });
                    }
                }
            );
        }

        function getLinkedUsers() {
            dataTable.ajax.reload();
        }
    };
})(jQuery);