import { AfterViewChecked, AfterViewInit, Component, ElementRef, Injector, OnInit } from '@angular/core';
import { PermissionTreeEditModel } from '@app/admin/shared/permission-tree-edit.model';
import { AppComponentBase } from '@shared/common/app-component-base';
import * as _ from 'lodash';

@Component({
    selector: 'permission-tree',
    template:
    `<div class="permission-tree"></div>`
})
export class PermissionTreeComponent extends AppComponentBase implements OnInit, AfterViewInit, AfterViewChecked {

    set editData(val: PermissionTreeEditModel) {
        this._editData = val;
        this.refreshTree();
    }

    private _$tree: JQuery;
    private _editData: PermissionTreeEditModel;
    private _createdTreeBefore;

    constructor(private _element: ElementRef,
        injector: Injector
    ) {
        super(injector);
    }

    ngOnInit(): void {
    }

    ngAfterViewInit(): void {
        this._$tree = $(this._element.nativeElement);

        this.refreshTree();
    }

    ngAfterViewChecked(): void {

    }

    getGrantedPermissionNames(): string[] {
        if (!this._$tree || !this._createdTreeBefore) {
            return [];
        }

        let permissionNames = [];

        let selectedPermissions = this._$tree.jstree('get_selected', true);
        for (let i = 0; i < selectedPermissions.length; i++) {
            permissionNames.push(selectedPermissions[i].original.id);
        }

        return permissionNames;
    }

    refreshTree(): void {
        let self = this;

        if (this._createdTreeBefore) {
            this._$tree.jstree('destroy');
        }

        this._createdTreeBefore = false;

        if (!this._editData || !this._$tree) {
            return;
        }

        let treeData = _.map(this._editData.permissions, function (item) {
            return {
                id: item.name,
                parent: item.parentName ? item.parentName : '#',
                text: item.displayName,
                state: {
                    opened: true,
                    selected: _.includes(self._editData.grantedPermissionNames, item.name)
                }
            };
        });

        this._$tree.jstree({
            'core': {
                data: treeData
            },
            'types': {
                'default': {
                    'icon': 'fa fa-folder m--font-warning'
                },
                'file': {
                    'icon': 'fa fa-file m--font-warning'
                }
            },
            'checkbox': {
                keep_selected_style: false,
                three_state: false,
                cascade: ''
            },
            plugins: ['checkbox', 'types']
        });

        this._createdTreeBefore = true;

        let inTreeChangeEvent = false;

        function selectNodeAndAllParents(node) {
            self._$tree.jstree('select_node', node, true);
            let parent = self._$tree.jstree('get_parent', node);
            if (parent) {
                selectNodeAndAllParents(parent);
            }
        }

        this._$tree.on('changed.jstree', function (e, data) {
            if (!data.node) {
                return;
            }

            let wasInTreeChangeEvent = inTreeChangeEvent;
            if (!wasInTreeChangeEvent) {
                inTreeChangeEvent = true;
            }

            let childrenNodes;

            if (data.node.state.selected) {
                selectNodeAndAllParents(self._$tree.jstree('get_parent', data.node));

                childrenNodes = $.makeArray(self._$tree.jstree('get_node', data.node).children);
                self._$tree.jstree('select_node', childrenNodes);

            } else {
                childrenNodes = $.makeArray(self._$tree.jstree('get_node', data.node).children);
                self._$tree.jstree('deselect_node', childrenNodes);
            }

            if (!wasInTreeChangeEvent) {
                inTreeChangeEvent = false;
            }
        });
    }
}
