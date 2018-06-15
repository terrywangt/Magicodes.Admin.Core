import { AfterViewInit, Component, ElementRef, Injector } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import { HtmlHelper } from '@shared/helpers/HtmlHelper';
import { OrganizationUnitDto } from '@shared/service-proxies/service-proxies';
import * as _ from 'lodash';

export interface IOrganizationUnitsTreeComponentData {
    allOrganizationUnits: OrganizationUnitDto[];
    selectedOrganizationUnits: string[];
}

@Component({
    selector: 'organization-unit-tree',
    template:
    `<div class='form-group'>
        <input id='OrganizationUnitsTreeFilter' type='text' class='form-control' placeholder='{{l("SearchWithThreeDot")}}' >
    </div>
    <div class="organization-unit-tree"></div>
    `
})
export class OrganizationUnitsTreeComponent extends AppComponentBase implements AfterViewInit {

    set data(data: IOrganizationUnitsTreeComponentData) {
        this._allOrganizationUnits = data.allOrganizationUnits;
        this._selectedOrganizationUnits = data.selectedOrganizationUnits;
        this.refreshTree();
    }

    private _$tree: JQuery;
    private _createdTreeBefore;

    private _allOrganizationUnits: OrganizationUnitDto[];
    private _selectedOrganizationUnits: string[];

    private filter = '';

    constructor(private _element: ElementRef,
        injector: Injector
    ) {
        super(injector);
    }

    ngAfterViewInit(): void {
        this._$tree = $(this._element.nativeElement).find('.organization-unit-tree');
        this.refreshTree();
        this.initFiltering();
    }

    getSelectedOrganizations(): number[] {
        if (!this._$tree || !this._createdTreeBefore) {
            return [];
        }

        let organizationIds = [];

        let selectedOrganizations = this._$tree.jstree('get_selected', true);
        for (let i = 0; i < selectedOrganizations.length; i++) {
            organizationIds.push(selectedOrganizations[i].original.id);
        }

        return organizationIds;
    }

    refreshTree(): void {
        let self = this;

        if (this._createdTreeBefore) {
            this._$tree.jstree('destroy');
        }

        this._createdTreeBefore = false;

        if (!this._allOrganizationUnits || !this._$tree) {
            return;
        }

        let treeData = _.map(this._allOrganizationUnits, item => (<any>{
            id: item.id,
            parent: item.parentId ? item.parentId : '#',
            code: item.code,
            displayName: item.displayName,
            memberCount: item.memberCount,
            text: HtmlHelper.encodeText(item.displayName) ,
            dto: item,
            state: {
                opened: true,
                selected: _.includes(self._selectedOrganizationUnits, item.code)
            }
        }));

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
            'search': {
                'show_only_matches': true
            },
            plugins: ['checkbox', 'types', 'search']
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

        this._$tree.on('changed.jstree', (e, data) => {
            if (!data.node) {
                return;
            }

            let wasInTreeChangeEvent = inTreeChangeEvent;
            if (!wasInTreeChangeEvent) {
                inTreeChangeEvent = true;
            }

            let childrenNodes;

            if (data.node.state.selected) {
                selectNodeAndAllParents(this._$tree.jstree('get_parent', data.node));

                childrenNodes = $.makeArray(this._$tree.jstree('get_node', data.node).children);
                this._$tree.jstree('select_node', childrenNodes);

            } else {
                childrenNodes = $.makeArray(this._$tree.jstree('get_node', data.node).children);
                this._$tree.jstree('deselect_node', childrenNodes);
            }

            if (!wasInTreeChangeEvent) {
                inTreeChangeEvent = false;
            }
        });
    }

    initFiltering(): void {
        let to = false;
        let self = this;

        $('#OrganizationUnitsTreeFilter').keyup(() => {
            if (to) { (window as any).clearTimeout(to); }
            to = (window as any).setTimeout(() => {
                let v = $('#OrganizationUnitsTreeFilter').val() as string;
                self._$tree.jstree(true).search(v);
            }, 250);
        });
    }
}
