﻿import {
    Component,
    ViewChild,
    Injector,
    Output,
    EventEmitter,
    ElementRef,
    ChangeDetectorRef
} from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap';
import {
    ColumnInfoServiceProxy,
    CreateOrUpdateColumnInfoDto,
    ColumnInfoEditDto,
    GetDataComboItemDtoOfInt64
} from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import * as _ from 'lodash';
import { finalize } from 'rxjs/operators';

@Component({
    selector: 'createOrEditColumnInfoModal',
    templateUrl: './create-or-edit-columnInfo-modal.component.html'
})
export class CreateOrEditColumnInfoModalComponent extends AppComponentBase {
    @ViewChild('createOrEditModal') modal: ModalDirective;
    @ViewChild('IsFooterNav') isFooterNav: ElementRef;
    @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();
    active = false;
    saving = false;
    formModel: ColumnInfoEditDto = new ColumnInfoEditDto();
    columnInfoComboItemDtoList: GetDataComboItemDtoOfInt64[];
    constructor(
        injector: Injector,
        private _columnInfoService: ColumnInfoServiceProxy,
        private _changeDetector: ChangeDetectorRef
    ) {
        super(injector);
    }

    onShown(): void {
        this._columnInfoService
            .getColumnInfoDataComboItems()
            .subscribe(result => {
                this.columnInfoComboItemDtoList = result;
            });
    }

    show(id?: number): void {
        this.formModel = new ColumnInfoEditDto();
        this.active = true;
        this._changeDetector.detectChanges();
        this.formModel.id = id;
        this._columnInfoService.getColumnInfoForEdit(id).subscribe(result => {
            this.formModel = result.columnInfo;
            this.modal.show();
        });
    }

    save(): void {
        const createOrEditInput = new CreateOrUpdateColumnInfoDto();
        createOrEditInput.columnInfo = this.formModel;
        this.saving = true;
        this._columnInfoService
            .createOrUpdateColumnInfo(createOrEditInput)
            .pipe(finalize(() => (this.saving = false)))
            .subscribe(() => {
                this.notify.info(this.l('SavedSuccessfully'));
                this.close();
                this.modalSave.emit(true);
            });
    }

    close(): void {
        this.modal.hide();
        this.active = false;
        this.modalSave.emit(true);
    }

    onCheckbox() {
        this.isFooterNav.nativeElement.disabled = !this.formModel.isHeaderNav;
    }
}
