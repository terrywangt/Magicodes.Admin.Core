import { Component, ViewChild, Injector, Output, EventEmitter, ElementRef, ChangeDetectorRef } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap';
import { TransactionLogServiceProxy, CreateOrUpdateTransactionLogDto, TransactionLogEditDto } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import * as _ from 'lodash';
import { Data } from '@angular/router/src/config';
import * as moment from 'moment';
import { finalize } from 'rxjs/operators';

@Component({
    selector: 'createOrEditTransactionLogModal',
    templateUrl: './create-or-edit-transactionLog-modal.component.html'
})
export class CreateOrEditTransactionLogModalComponent extends AppComponentBase {
    @ViewChild('PayTimeDatePicker') PayTimeDatePicker: ElementRef;
    @ViewChild('createOrEditModal') modal: ModalDirective;
    @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();
    active = false;
    saving = false;


    formModel: TransactionLogEditDto = new TransactionLogEditDto();

    constructor(
        injector: Injector,
        private _transactionLogService: TransactionLogServiceProxy,
        private _changeDetector: ChangeDetectorRef,
    ) {
        super(injector);
    }

    onShown(): void {
        // $(this.PayTimeDatePicker.nativeElement).datetimepicker({
        // 	locale: abp.localization.currentLanguage.name,
        // 	format: 'L LT',
        // 	date: this.formModel.id == null ? '' : this.formModel.payTime.format()
        // });
    }

    show(id?: number): void {
        this.formModel = new TransactionLogEditDto();
        this.active = true;
        this._changeDetector.detectChanges();
        this.formModel.id = id;
        this._transactionLogService.getTransactionLogForEdit(id).subscribe(result => {
            this.formModel = result.transactionLog;
            this.modal.show();
        });
    }

    save(): void {
        const createOrEditInput = new CreateOrUpdateTransactionLogDto();
        createOrEditInput.transactionLog = this.formModel;
        //createOrEditInput.transactionLog.payTime = $(this.PayTimeDatePicker.nativeElement).data('DateTimePicker').date();
        this.saving = true;
        this._transactionLogService.createOrUpdateTransactionLog(createOrEditInput)
            .pipe(finalize(() => this.saving = false))
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
}