(function () {
    $(function () {

        var _$editionsTable = $('#EditionsTable');
        var _editionService = abp.services.app.edition;

        var _permissions = {
            create: abp.auth.hasPermission('Pages.Editions.Create'),
            edit: abp.auth.hasPermission('Pages.Editions.Edit'),
            'delete': abp.auth.hasPermission('Pages.Editions.Delete')
        };

        var _createOrEditModal = new app.ModalManager({
            viewUrl: abp.appPath + 'Admin/Editions/CreateOrEditModal',
            scriptUrl: abp.appPath + 'view-resources/Areas/Admin/Views/Editions/_CreateOrEditModal.js',
            modalClass: 'CreateOrEditEditionModal'
        });

        function deleteEdition(edition) {
            abp.message.confirm(
                app.localize('EditionDeleteWarningMessage', edition.displayName),
                function (isConfirmed) {
                    if (isConfirmed) {
                        _editionService.deleteEdition({
                            id: edition.id
                        }).done(function () {
                            getEditions();
                            abp.notify.success(app.localize('SuccessfullyDeleted'));
                        });
                    }
                }
            );
        };

        $('#CreateNewEditionButton').click(function () {
            _createOrEditModal.open();
        });

        abp.event.on('app.createOrEditEditionModalSaved', function () {
            getEditions();
        });

        var dataTable = _$editionsTable.DataTable({
            paging: false,
            listAction: {
                ajaxFunction: _editionService.getEditions
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
                                text: app.localize('Edit'),
                                visible: function () {
                                    return _permissions.edit;
                                },
                                action: function (data) {
                                    _createOrEditModal.open({ id: data.record.id });
                                }
                            }, {
                                text: app.localize('Delete'),
                                visible: function () {
                                    return _permissions.delete;
                                },
                                action: function (data) {
                                    deleteEdition(data.record);
                                }
                            }
                        ]
                    }
                },
                {
                    targets: 1,
                    data: "displayName"
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

        function getEditions() {
            dataTable.ajax.reload();
        }
    });
})();