import { NotifyService } from '@abp/notify/notify.service';
import { Component, Injector, ViewChild } from '@angular/core';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { AppComponentBase } from '@shared/common/app-component-base';
import { RoleListDto, RoleServiceProxy } from '@shared/service-proxies/service-proxies';
import { FileDownloadService } from '@shared/utils/file-download.service';
import { Table } from 'primeng/components/table/table';
import { CreateOrEditRoleModalComponent } from './create-or-edit-role-modal.component';

@Component({
    templateUrl: './roles.component.html',
    animations: [appModuleAnimation()]
})
export class RolesComponent extends AppComponentBase {

    @ViewChild('createOrEditRoleModal') createOrEditRoleModal: CreateOrEditRoleModalComponent;
    @ViewChild('dataTable') dataTable: Table;


    //Filters
    selectedPermission = '';

    constructor(
        injector: Injector,
        private _roleService: RoleServiceProxy,
        private _notifyService: NotifyService,
        private _fileDownloadService: FileDownloadService
    ) {
        super(injector);
    }

    getRoles(): void {
        this.primengTableHelper.showLoadingIndicator();
        let permission = this.permission ? this.selectedPermission : undefined;

        this._roleService.getRoles(permission).subscribe(result => {
            this.primengTableHelper.records = result.items;
            this.primengTableHelper.totalRecordsCount = result.items.length;
            this.primengTableHelper.hideLoadingIndicator();
        });
    }

    createRole(): void {
        this.createOrEditRoleModal.show();
    }

    deleteRole(role: RoleListDto): void {
        let self = this;
        self.message.confirm(
            self.l('RoleDeleteWarningMessage', role.displayName),
            this.l('AreYouSure'),
            isConfirmed => {
                if (isConfirmed) {
                    this._roleService.deleteRole(role.id).subscribe(() => {
                        this.getRoles();
                        abp.notify.success(this.l('SuccessfullyDeleted'));
                    });
                }
            }
        );
    }
}
