(function () {
    $(function () {

        var _organizationUnitService = abp.services.app.organizationUnit;

        var _permissions = {
            manageOrganizationTree: abp.auth.hasPermission('Pages.Administration.OrganizationUnits.ManageOrganizationTree'),
            manageMembers: abp.auth.hasPermission('Pages.Administration.OrganizationUnits.ManageMembers')
        };

        var _createModal = new app.ModalManager({
            viewUrl: abp.appPath + 'Admin/OrganizationUnits/CreateModal',
            scriptUrl: abp.appPath + 'view-resources/Areas/Admin/Views/OrganizationUnits/_CreateModal.js',
            modalClass: 'CreateOrganizationUnitModal'
        });

        var _editModal = new app.ModalManager({
            viewUrl: abp.appPath + 'Admin/OrganizationUnits/EditModal',
            scriptUrl: abp.appPath + 'view-resources/Areas/Admin/Views/OrganizationUnits/_EditModal.js',
            modalClass: 'EditOrganizationUnitModal'
        });

        var _addUserModal = new app.ModalManager({
            viewUrl: abp.appPath + 'Admin/OrganizationUnits/AddMemberModal',
            scriptUrl: abp.appPath + 'view-resources/Areas/Admin/Views/OrganizationUnits/_AddMemberModal.js',
            modalClass: 'AddMemberModal',
            addMemberOptions: {
                title: app.localize('SelectAUser'),
                serviceMethod: abp.services.app.organizationUnit.findUsers
            }
        });

        var organizationTree = {

            $tree: $('#OrganizationUnitEditTree'),

            $emptyInfo: $('#OrganizationUnitTreeEmptyInfo'),

            show: function () {
                organizationTree.$emptyInfo.hide();
                organizationTree.$tree.show();
            },

            hide: function () {
                organizationTree.$emptyInfo.show();
                organizationTree.$tree.hide();
            },

            unitCount: 0,

            setUnitCount: function (unitCount) {
                organizationTree.unitCount = unitCount;
                if (unitCount) {
                    organizationTree.show();
                } else {
                    organizationTree.hide();
                }
            },

            refreshUnitCount: function () {
                organizationTree.setUnitCount(organizationTree.$tree.jstree('get_json').length);
            },

            selectedOu: {
                id: null,
                displayName: null,
                code: null,

                set: function (ouInTree) {
                    if (!ouInTree) {
                        organizationTree.selectedOu.id = null;
                        organizationTree.selectedOu.displayName = null;
                        organizationTree.selectedOu.code = null;
                    } else {
                        organizationTree.selectedOu.id = ouInTree.id;
                        organizationTree.selectedOu.displayName = ouInTree.original.displayName;
                        organizationTree.selectedOu.code = ouInTree.original.code;
                    }

                    members.load();
                }
            },

            contextMenu: function (node) {

                var items = {
                    editUnit: {
                        label: app.localize('Edit'),
                        _disabled: !_permissions.manageOrganizationTree,
                        action: function (data) {
                            var instance = $.jstree.reference(data.reference);

                            _editModal.open({
                                id: node.id
                            }, function (updatedOu) {
                                node.original.displayName = updatedOu.displayName;
                                instance.rename_node(node, organizationTree.generateTextOnTree(updatedOu));
                            });
                        }
                    },

                    addSubUnit: {
                        label: app.localize('AddSubUnit'),
                        _disabled: !_permissions.manageOrganizationTree,
                        action: function () {
                            organizationTree.addUnit(node.id);
                        }
                    },

                    addMember: {
                        label: app.localize('AddMember'),
                        _disabled: !_permissions.manageMembers,
                        action: function () {
                            members.openAddModal();
                        }
                    },

                    'delete': {
                        label: app.localize("Delete"),
                        _disabled: !_permissions.manageOrganizationTree,
                        action: function (data) {
                            var instance = $.jstree.reference(data.reference);

                            abp.message.confirm(
                                app.localize('OrganizationUnitDeleteWarningMessage', node.original.displayName),
                                function (isConfirmed) {
                                    if (isConfirmed) {
                                        _organizationUnitService.deleteOrganizationUnit({
                                            id: node.id
                                        }).done(function () {
                                            abp.notify.success(app.localize('SuccessfullyDeleted'));
                                            instance.delete_node(node);
                                            organizationTree.refreshUnitCount();
                                        }).fail(function (err) {
                                            setTimeout(function () { abp.message.error(err.message); }, 500);
                                        });;
                                    }
                                }
                            );
                        }
                    }
                }

                return items;
            },

            addUnit: function (parentId) {
                var instance = $.jstree.reference(organizationTree.$tree);

                _createModal.open({
                    parentId: parentId
                }, function (newOu) {
                    instance.create_node(
                        parentId ? instance.get_node(parentId) : '#',
                        {
                            id: newOu.id,
                            parent: newOu.parentId ? newOu.parentId : '#',
                            code: newOu.code,
                            displayName: newOu.displayName,
                            memberCount: 0,
                            text: organizationTree.generateTextOnTree(newOu),
                            state: {
                                opened: true
                            }
                        });

                    organizationTree.refreshUnitCount();
                });
            },

            generateTextOnTree: function (ou) {
                var itemClass = ou.memberCount > 0 ? ' ou-text-has-members' : ' ou-text-no-members';
                return '<span title="' + ou.code + '" class="ou-text' + itemClass + '" data-ou-id="' + ou.id + '">' + ou.displayName + ' (<span class="ou-text-member-count">' + ou.memberCount + '</span>) <i class="fa fa-caret-down text-muted"></i></span>';
            },

            incrementMemberCount: function (ouId, incrementAmount) {
                var treeNode = organizationTree.$tree.jstree('get_node', ouId);
                treeNode.original.memberCount = treeNode.original.memberCount + incrementAmount;
                organizationTree.$tree.jstree('rename_node', treeNode, organizationTree.generateTextOnTree(treeNode.original));
            },

            getTreeDataFromServer: function (callback) {
                _organizationUnitService.getOrganizationUnits({}).done(function (result) {
                    var treeData = _.map(result.items, function (item) {
                        return {
                            id: item.id,
                            parent: item.parentId ? item.parentId : '#',
                            code: item.code,
                            displayName: item.displayName,
                            memberCount: item.memberCount,
                            text: organizationTree.generateTextOnTree(item),
                            state: {
                                opened: true
                            }
                        };
                    });

                    callback(treeData);
                });
            },

            init: function () {
                organizationTree.getTreeDataFromServer(function (treeData) {

                    organizationTree.setUnitCount(treeData.length);

                    organizationTree.$tree
                        .on('changed.jstree', function (e, data) {
                            if (data.selected.length != 1) {
                                organizationTree.selectedOu.set(null);
                            } else {
                                var selectedNode = data.instance.get_node(data.selected[0]);
                                organizationTree.selectedOu.set(selectedNode);
                            }
                        })
                        .on('move_node.jstree', function (e, data) {

                            var parentNodeName = (!data.parent || data.parent == '#')
                                ? app.localize('Root')
                                : organizationTree.$tree.jstree('get_node', data.parent).original.displayName;

                            abp.message.confirm(
                                app.localize('OrganizationUnitMoveConfirmMessage', data.node.original.displayName, parentNodeName),
                                function (isConfirmed) {
                                    if (isConfirmed) {
                                        _organizationUnitService.moveOrganizationUnit({
                                            id: data.node.id,
                                            newParentId: data.parent
                                        }).done(function () {
                                            abp.notify.success(app.localize('SuccessfullyMoved'));
                                            organizationTree.reload();
                                        }).fail(function (err) {
                                            organizationTree.$tree.jstree('refresh'); //rollback
                                            setTimeout(function () { abp.message.error(err.message); }, 500);
                                        });
                                    } else {
                                        organizationTree.$tree.jstree('refresh'); //rollback
                                    }
                                }
                            );
                        })
                        .jstree({
                            'core': {
                                data: treeData,
                                multiple: false,
                                check_callback: function (operation, node, node_parent, node_position, more) {
                                    return true;
                                }
                            },
                            types: {
                                "default": {
                                    "icon": "fa fa-folder tree-item-icon-color icon-lg"
                                },
                                "file": {
                                    "icon": "fa fa-file tree-item-icon-color icon-lg"
                                }
                            },
                            contextmenu: {
                                items: organizationTree.contextMenu
                            },
                            sort: function (node1, node2) {
                                if (this.get_node(node2).original.displayName < this.get_node(node1).original.displayName) {
                                    return 1;
                                }

                                return -1;
                            },
                            plugins: [
                                'types',
                                'contextmenu',
                                'wholerow',
                                'sort',
                                'dnd'
                            ]
                        });

                    $('#AddRootUnitButton').click(function (e) {
                        e.preventDefault();
                        organizationTree.addUnit(null);
                    });

                    organizationTree.$tree.on('click', '.ou-text .fa-caret-down', function (e) {
                        e.preventDefault();

                        var ouId = $(this).closest('.ou-text').attr('data-ou-id');
                        setTimeout(function () {
                            organizationTree.$tree.jstree('show_contextmenu', ouId);
                        }, 100);
                    });
                });
            },

            reload: function () {
                organizationTree.getTreeDataFromServer(function (treeData) {
                    organizationTree.setUnitCount(treeData.length);
                    organizationTree.$tree.jstree(true).settings.core.data = treeData;
                    organizationTree.$tree.jstree('refresh');
                });
            }
        };

        var members = {

            $table: $('#OuMembersTable'),
            $emptyInfo: $('#OuMembersEmptyInfo'),
            $addUserToOuButton: $('#AddUserToOuButton'),
            $selectedOuRightTitle: $('#SelectedOuRightTitle'),
            dataTable: null,

            showTable: function () {
                members.$emptyInfo.hide();
                members.$table.show();
                members.$addUserToOuButton.show();
                members.$selectedOuRightTitle.text(': ' + organizationTree.selectedOu.displayName).show();
            },

            hideTable: function () {
                members.$selectedOuRightTitle.hide();
                members.$addUserToOuButton.hide();
                members.$table.hide();
                members.$emptyInfo.show();
            },

            load: function () {
                if (!organizationTree.selectedOu.id) {
                    members.hideTable();
                    return;
                }

                members.showTable();
                this.dataTable.ajax.reload();
            },

            add: function (users) {
                var ouId = organizationTree.selectedOu.id;
                if (!ouId) {
                    return;
                }

                var userIds = _.pluck(users, "value");
                _organizationUnitService.addUsersToOrganizationUnit({
                    organizationUnitId: ouId,
                    userIds: userIds
                }).done(function () {
                    abp.notify.success(app.localize('SuccessfullyAdded'));
                    organizationTree.incrementMemberCount(ouId, userIds.length);
                    members.load();
                });
            },

            remove: function (user) {
                var ouId = organizationTree.selectedOu.id;
                if (!ouId) {
                    return;
                }

                abp.message.confirm(
                    app.localize('RemoveUserFromOuWarningMessage', user.userName, organizationTree.selectedOu.displayName),
                    function (isConfirmed) {
                        if (isConfirmed) {
                            _organizationUnitService.removeUserFromOrganizationUnit({
                                organizationUnitId: parseInt(ouId),
                                userId: user.id
                            }).done(function () {
                                abp.notify.success(app.localize('SuccessfullyRemoved'));
                                organizationTree.incrementMemberCount(ouId, -1);
                                members.load();
                            });
                        }
                    }
                );
            },

            openAddModal: function () {
                var ouId = organizationTree.selectedOu.id;
                if (!ouId) {
                    return;
                }
                
                _addUserModal.open({
                    title: app.localize('SelectAUser'),
                    organizationUnitId: ouId
                }, function (selectedItems) {
                    members.add(selectedItems);
                });
            },

            init: function () {
                this.dataTable = members.$table.find(".organization-members-table").DataTable({
                    paging: true,
                    serverSide: true,
                    processing: true,
                    deferLoading: 0, //prevents table for ajax request on initialize
                    listAction: {
                        ajaxFunction: _organizationUnitService.getOrganizationUnitUsers,
                        inputFilter: function () {
                            return { id: organizationTree.selectedOu.id }
                        }
                    },
                    columnDefs: [
                        {
                            targets: 0,
                            data: null,
                            orderable: false,
                            defaultContent: '',
                            rowAction: {
                                targets: 0,
                                data: null,
                                orderable: false,
                                defaultContent: '',
                                element: $("<button/>")
                                    .addClass("btn btn-default btn-xs")
                                    .attr("title", app.localize('Delete'))
                                    .append($("<i/>").addClass("fa fa-times")).click(function () {
                                        var record = $(this).data();
                                        members.remove(record);
                                    }),
                                visible: function () {
                                    return _permissions.manageMembers;
                                }
                            }
                        },
                        {
                            targets: 1,
                            data: "userName"
                        },
                        {
                            targets: 2,
                            data: "addedTime",
                            render: function (addedTime) {
                                return moment(addedTime).format('L');
                            }
                        }
                    ]
                });


                $('#AddUserToOuButton').click(function (e) {
                    e.preventDefault();
                    members.openAddModal();
                });

                members.hideTable();
            }
        }

        members.init();
        organizationTree.init();

    });
})();