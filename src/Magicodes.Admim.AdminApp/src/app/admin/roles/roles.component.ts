import { Component,  Injector, ViewChild } from '@angular/core';
import { RoleServiceProxy, RoleListDto } from '@shared/service-proxies/service-proxies';
import { NotifyService } from '@abp/notify/notify.service';
import { AppComponentBase } from '@shared/common/app-component-base';
import { FileDownloadService } from '@shared/utils/file-download.service';
import { CreateOrEditRoleModalComponent } from './create-or-edit-role-modal.component';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { DataTable } from 'primeng/components/datatable/datatable';

@Component({
    templateUrl: './roles.component.html',
    animations: [appModuleAnimation()]
})
export class RolesComponent extends AppComponentBase {

    @ViewChild('createOrEditRoleModal') createOrEditRoleModal: CreateOrEditRoleModalComponent;
    @ViewChild('dataTable') dataTable: DataTable;


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
        this.primengDatatableHelper.showLoadingIndicator();
        let permission = this.permission ? this.selectedPermission : undefined;

        this._roleService.getRoles(permission).subscribe(result => {
            this.primengDatatableHelper.records = result.items;
            this.primengDatatableHelper.totalRecordsCount = result.items.length;
            this.primengDatatableHelper.hideLoadingIndicator();
        });
    }

    createRole(): void {
        this.createOrEditRoleModal.show();
    }

    deleteRole(role: RoleListDto): void {
        let self = this;
        self.message.confirm(
            self.l('RoleDeleteWarningMessage', role.displayName),
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
