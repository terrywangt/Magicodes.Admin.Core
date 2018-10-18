import { OnInit, Component, EventEmitter, Injector, Output, ViewChild } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import { HtmlHelper } from '@shared/helpers/HtmlHelper';
import { ListResultDtoOfOrganizationUnitDto, MoveOrganizationUnitInput, OrganizationUnitDto, OrganizationUnitServiceProxy } from '@shared/service-proxies/service-proxies';
import * as _ from 'lodash';
import { Observable } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { IBasicOrganizationUnitInfo } from './basic-organization-unit-info';
import { CreateOrEditUnitModalComponent } from './create-or-edit-unit-modal.component';
import { IUserWithOrganizationUnit } from './user-with-organization-unit';
import { IUsersWithOrganizationUnit } from './users-with-organization-unit';

import { TreeNode, MenuItem } from 'primeng/api';

import { ArrayToTreeConverterService } from '@shared/utils/array-to-tree-converter.service';
import { TreeDataHelperService } from '@shared/utils/tree-data-helper.service';

export interface IOrganizationUnitOnTree extends IBasicOrganizationUnitInfo {
    id: number;
    parent: string | number;
    code: string;
    displayName: string;
    memberCount: number;
    text: string;
    state: any;
}

@Component({
    selector: 'organization-tree',
    templateUrl: './organization-tree.component.html'
})
export class OrganizationTreeComponent extends AppComponentBase implements OnInit {

    @Output() ouSelected = new EventEmitter<IBasicOrganizationUnitInfo>();

    @ViewChild('createOrEditOrganizationUnitModal') createOrEditOrganizationUnitModal: CreateOrEditUnitModalComponent;


    treeData: any;
    selectedOu: TreeNode;
    ouContextMenuItems: MenuItem[];
    canManageOrganizationUnits = false;

    constructor(
        injector: Injector,
        private _organizationUnitService: OrganizationUnitServiceProxy,
        private _arrayToTreeConverterService: ArrayToTreeConverterService,
        private _treeDataHelperService: TreeDataHelperService
    ) {
        super(injector);
    }

    totalUnitCount = 0;

    ngOnInit(): void {
        this.canManageOrganizationUnits = this.isGranted('Pages.Administration.OrganizationUnits.ManageOrganizationTree');
        this.ouContextMenuItems = this.getContextMenuItems();
        this.getTreeDataFromServer();
    }

    nodeSelect(event) {
        this.ouSelected.emit(<IBasicOrganizationUnitInfo>{
            id: event.node.data.id,
            displayName: event.node.data.displayName
        });
    }

    nodeDrop(event) {
        this.message.confirm(
            this.l('OrganizationUnitMoveConfirmMessage', event.dragNode.data.displayName, event.dropNode.data.displayName),
            this.l('AreYouSure'),
            isConfirmed => {
                if (isConfirmed) {
                    const input = new MoveOrganizationUnitInput();
                    input.id = event.dragNode.data.id;
                    input.newParentId = event.originalEvent.target.nodeName === 'SPAN' ? event.dropNode.data.id : event.dropNode.parent.data.id;

                    this._organizationUnitService.moveOrganizationUnit(input)
                        .pipe(catchError(error => {
                            this.revertDragDrop();
                            return Observable.throw(error);
                        }))
                        .subscribe(() => {
                            this.notify.success(this.l('SuccessfullyMoved'));
                            this.reload();
                        });
                } else {
                    this.revertDragDrop();
                }
            }
        );
    }

    revertDragDrop() {
        this.reload();
    }

    reload(): void {
        this.getTreeDataFromServer();
    }

    private getTreeDataFromServer(): void {
        let self = this;
        this._organizationUnitService.getOrganizationUnits().subscribe((result: ListResultDtoOfOrganizationUnitDto) => {
            this.totalUnitCount = result.items.length;
            this.treeData = this._arrayToTreeConverterService.createTree(result.items, 'parentId', 'id', null, 'children',
                [{
                    target: 'label',
                    targetFunction(item) {
                        return self.generateTextOnTree(item);
                    }
                }, {
                    target: 'expandedIcon',
                    value: 'fa fa-folder-open m--font-warning'
                },
                {
                    target: 'collapsedIcon',
                    value: 'fa fa-folder m--font-warning'
                },
                {
                    target: 'selectable',
                    value: true
                }]);
        });
    }

    private generateTextOnTree(ou: IOrganizationUnitOnTree | OrganizationUnitDto) {
        return HtmlHelper.encodeText(ou.displayName) + '(' + ou.memberCount + ')';
    }

    private getContextMenuItems(): any[] {

        const canManageOrganizationTree = this.isGranted('Pages.Administration.OrganizationUnits.ManageOrganizationTree');

        let items = [
            {
                label: this.l('Edit'),
                disabled: !canManageOrganizationTree,
                command: (event) => {
                    this.createOrEditOrganizationUnitModal.show({
                        id: this.selectedOu.data.id,
                        displayName: this.selectedOu.data.displayName
                    });
                }
            },
            {
                label: this.l('AddSubUnit'),
                disabled: !canManageOrganizationTree,
                command: () => {
                    this.addUnit(this.selectedOu.data.id);
                }
            },
            {
                label: this.l('Delete'),
                disabled: !canManageOrganizationTree,
                command: () => {
                    this.message.confirm(
                        this.l('OrganizationUnitDeleteWarningMessage', this.selectedOu.data.displayName),
                        this.l('AreYouSure'),
                        isConfirmed => {
                            if (isConfirmed) {
                                this._organizationUnitService.deleteOrganizationUnit(this.selectedOu.data.id).subscribe(() => {
                                    this.deleteUnit(this.selectedOu.data.id);
                                    this.notify.success(this.l('SuccessfullyDeleted'));
                                    this.selectedOu = null;
                                });
                            }
                        }
                    );
                }
            }
        ];

        return items;
    }

    addUnit(parentId?: number): void {
        this.createOrEditOrganizationUnitModal.show({
            parentId: parentId
        });
    }

    unitCreated(ou: OrganizationUnitDto): void {
        if (ou.parentId) {
            let unit = this._treeDataHelperService.findNode(this.treeData, { data: { id: ou.parentId } });
            if (!unit) {
                return;
            }

            unit.children.push({
                label: this.generateTextOnTree(ou),
                expandedIcon: 'fa fa-folder-open m--font-warning',
                collapsedIcon: 'fa fa-folder m--font-warning',
                selected: true,
                children: [],
                data: ou
            });
        } else {
            this.treeData.push({
                label: this.generateTextOnTree(ou),
                expandedIcon: 'fa fa-folder-open m--font-warning',
                collapsedIcon: 'fa fa-folder m--font-warning',
                selected: true,
                children: [],
                data: ou
            });
        }

        this.totalUnitCount += 1;
    }

    deleteUnit(id) {
        let node = this._treeDataHelperService.findNode(this.treeData, { data: { id: id } });
        if (!node) {
            return;
        }

        if (!node.data.parentId) {
            _.remove(this.treeData, {
                data: {
                    id: id
                }
            });
        }

        let parentNode = this._treeDataHelperService.findNode(this.treeData, { data: { id: node.data.parentId } });
        if (!parentNode) {
            return;
        }

        _.remove(parentNode.children, {
            data: {
                id: id
            }
        });
    }

    unitUpdated(ou: OrganizationUnitDto): void {
        let item = this._treeDataHelperService.findNode(this.treeData, { data: { id: ou.id } });
        if (!item) {
            return;
        }

        item.label = this.generateTextOnTree(ou);
    }

    membersAdded(data: IUsersWithOrganizationUnit): void {
        this.incrementMemberCount(data.ouId, data.userIds.length);
    }

    memberRemoved(data: IUserWithOrganizationUnit): void {
        this.incrementMemberCount(data.ouId, -1);
    }

    incrementMemberCount(ouId: number, incrementAmount: number): void {
        let item = this._treeDataHelperService.findNode(this.treeData, { data: { id: ouId } });
        item.data.memberCount += incrementAmount;
        item.label = this.generateTextOnTree(item.data);
    }
}
