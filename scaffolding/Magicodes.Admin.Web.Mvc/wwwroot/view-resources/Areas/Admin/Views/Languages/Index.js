(function () {
    $(function () {

        var _$languagesTable = $('#LanguagesTable');
        var _languageService = abp.services.app.language;
        var _defaultLanguageName = null;

        var _permissions = {
            create: abp.auth.hasPermission('Pages.Administration.Languages.Create'),
            edit: abp.auth.hasPermission('Pages.Administration.Languages.Edit'),
            changeTexts: abp.auth.hasPermission('Pages.Administration.Languages.ChangeTexts'),
            'delete': abp.auth.hasPermission('Pages.Administration.Languages.Delete')
        };

        var _createOrEditModal = new app.ModalManager({
            viewUrl: abp.appPath + 'Admin/Languages/CreateOrEditModal',
            scriptUrl: abp.appPath + 'view-resources/Areas/Admin/Views/Languages/_CreateOrEditModal.js',
            modalClass: 'CreateOrEditLanguageModal'
        });

        var dataTable = _$languagesTable.DataTable({
            paging: false,
            serverSide: false,
            processing: false,
            listAction: {
                ajaxFunction: _languageService.getLanguages
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
                            visible: function (data) {
                                return _permissions.edit && data.record.tenantId === abp.session.tenantId;
                            },
                            action: function (data) {
                                _createOrEditModal.open({ id: data.record.id });
                            }
                        }, {
                            text: app.localize('ChangeTexts'),
                            visible: function () {
                                return _permissions.changeTexts;
                            },
                            action: function (data) {
                                document.location.href = abp.appPath + "Admin/Languages/Texts?languageName=" + data.record.name;
                            }
                        }, {
                            text: app.localize('SetAsDefaultLanguage'),
                            visible: function () {
                                return _permissions.edit;
                            },
                            action: function (data) {
                                setAsDefaultLanguage(data.record);
                            }
                        }, {
                            text: app.localize('Delete'),
                            visible: function (data) {
                                return _permissions.delete && data.record.tenantId === abp.session.tenantId;
                            },
                            action: function (data) {
                                deleteLanguage(data.record);
                            }
                        }]
                    }
                },
                {
                    targets: 1,
                    data: "displayName",
                    render: function (displayName, type, row, meta) {
                        var $span = $('<span/>')
                            .append($("<i/>").addClass(row.icon).css("margin-right", "5px"))
                            .append($("<span/>").attr("data-language-name", row.name).text(row.displayName)); 

                        if (meta.settings.rawServerResponse.defaultLanguageName === row.name) {
                            $span.addClass("text-bold").append(" (" + app.localize("Default") + ")");
                        }

                        return $span[0].outerHTML;
                    }
                },
                {
                    targets: 2,
                    data: "name"
                },
                {
                    targets: 3,
                    data: "tenantId",
                    visible: abp.session.tenantId ? true : false, //this field is visible only for tenants
                    render: function (tenantId, type, row, meta) {
                        var $span = $('<span/>').addClass("label");

                        if (tenantId !== abp.session.tenantId) {
                            $span.addClass("label-default").text(app.localize('Yes'));
                        } else {
                            $span.addClass("label-success").text(app.localize('No'));
                        }

                        return $span[0].outerHTML;
                    }
                },
                {
                    targets: 4,
                    data: "creationTime",
                    render: function (creationTime) {
                        return moment(creationTime).format('L');
                    }
                },
                {
                    targets: 5,
                    data: "isDisabled",
                    render: function (isDisabled) {
                        var isEnabled = !isDisabled;
                        var $span = $("<span/>").addClass("label");
                        if (isEnabled) {
                            $span.addClass("label-success").text(app.localize('Yes'));
                        } else {
                            $span.addClass("label-default").text(app.localize('No'));
                        }

                        return $span[0].outerHTML;
                    }
                }
            ]
        });


        function setAsDefaultLanguage(language) {
            _languageService.setDefaultLanguage({
                name: language.name
            }).done(function () {
                getLanguages();
                abp.notify.success(app.localize('SuccessfullySaved'));
            });
        };

        function deleteLanguage(language) {
            abp.message.confirm(
                app.localize('LanguageDeleteWarningMessage', language.displayName),
                function (isConfirmed) {
                    if (isConfirmed) {
                        _languageService.deleteLanguage({
                            id: language.id
                        }).done(function () {
                            getLanguages();
                            abp.notify.success(app.localize('SuccessfullyDeleted'));
                        });
                    }
                }
            );
        };

        $('#CreateNewLanguageButton').click(function () {
            _createOrEditModal.open();
        });

        function getLanguages() {
            dataTable.ajax.reload();
        }

        abp.event.on('app.createOrEditLanguageModalSaved', function () {
            getLanguages();
        });

    });
})();