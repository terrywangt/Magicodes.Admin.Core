import { Component, ElementRef, EventEmitter, Injector, Input, OnInit, Output, ViewChild } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import { RoleListDto, RoleServiceProxy } from '@shared/service-proxies/service-proxies';

@Component({
    selector: 'role-combo',
    template:
    `<select #RoleCombobox
        class="form-control"
        [(ngModel)]="selectedRole"
        (ngModelChange)="selectedRoleChange.emit($event)"
        [attr.data-live-search]="true"
        jq-plugin="selectpicker">
            <option value="">{{emptyText}}</option>
            <option *ngFor="let role of roles" [value]="role.id">{{role.displayName}}</option>
    </select>`
})
export class RoleComboComponent extends AppComponentBase implements OnInit {

    @ViewChild('RoleCombobox') roleComboboxElement: ElementRef;

    roles: RoleListDto[] = [];

    @Input() selectedRole: string = undefined;
    @Output() selectedRoleChange: EventEmitter<string> = new EventEmitter<string>();

    @Input() emptyText = '';

    constructor(
        private _roleService: RoleServiceProxy,
        injector: Injector) {
        super(injector);
    }

    ngOnInit(): void {
        const self = this;
        this._roleService.getRoles(undefined).subscribe(result => {
            this.roles = result.items;
            setTimeout(() => {
                $(self.roleComboboxElement.nativeElement).selectpicker('refresh');
            }, 0);
        });
    }
}
