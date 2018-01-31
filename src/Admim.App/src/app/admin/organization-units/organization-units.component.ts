import { Component, Injector, ViewChild } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import { OrganizationUnitMembersComponent } from './organization-unit-members.component';
import { OrganizationTreeComponent } from './organization-tree.component';
import { appModuleAnimation } from '@shared/animations/routerTransition';

@Component({
    templateUrl: './organization-units.component.html',
    animations: [appModuleAnimation()]
})
export class OrganizationUnitsComponent extends AppComponentBase {

    @ViewChild('ouMembers') ouMembers: OrganizationUnitMembersComponent;
    @ViewChild('ouTree') ouTree: OrganizationTreeComponent;

    constructor(
        injector: Injector
    ) {
        super(injector);
    }
}
