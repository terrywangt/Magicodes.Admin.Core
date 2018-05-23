import { Component, Injector, ViewChild } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import { AuditLogServiceProxy, EntityChangeListDto, EntityPropertyChangeDto } from '@shared/service-proxies/service-proxies';
import * as moment from 'moment';
import { ModalDirective } from 'ngx-bootstrap';

@Component({
    selector: 'entityChangeDetailModal',
    templateUrl: './entity-change-detail-modal.component.html'
})
export class EntityChangeDetailModalComponent extends AppComponentBase {

    @ViewChild('entityChangeDetailModal')
    modal: ModalDirective;

    active = false;
    entityPropertyChanges: EntityPropertyChangeDto[];
    entityChange: EntityChangeListDto;

    constructor(
        injector: Injector,
        private _auditLogService: AuditLogServiceProxy
    ) {
        super(injector);
    }

    getPropertyChangeValue(propertyChangeValue, propertyTypeFullName) {
        if (!propertyChangeValue) {
            return propertyChangeValue;
        }
        propertyChangeValue = propertyChangeValue.replace(/^['"]+/g, '').replace(/['"]+$/g, '');
        if (this.isDate(propertyChangeValue, propertyTypeFullName)) {
            return moment(propertyChangeValue).format('YYYY-MM-DD HH:mm:ss');
        }

        if (propertyChangeValue === 'null') {
            return '';
        }

        return propertyChangeValue;
    }

    isDate(date, propertyTypeFullName): boolean {
        return propertyTypeFullName.includes("DateTime") && !isNaN(Date.parse(date).valueOf());
    }

    show(record: EntityChangeListDto): void {
        const self = this;
        self.active = true;
        self.entityChange = record;

        this._auditLogService.getEntityPropertyChanges(record.id).subscribe((result) => {
            self.entityPropertyChanges = result;
        });

        self.modal.show();
    }

    close(): void {
        this.active = false;
        this.modal.hide();
    }
}
