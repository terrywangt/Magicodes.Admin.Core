import { Component, ElementRef, EventEmitter, Injector, Output, ViewChild } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import { ProfileServiceProxy, VerifySmsCodeInputDto } from '@shared/service-proxies/service-proxies';
import { ModalDirective } from 'ngx-bootstrap';
import { finalize } from 'rxjs/operators';

@Component({
    selector: 'smsVerificationModal',
    templateUrl: './sms-verification-modal.component.html'
})
export class SmsVerificationModalComponent extends AppComponentBase {
    @ViewChild('nameInput') nameInput: ElementRef;
    @ViewChild('smsVerificationModal') modal: ModalDirective;

    @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

    public active = false;
    public saving = false;
    public verifyCode: VerifySmsCodeInputDto = new VerifySmsCodeInputDto();

    constructor(
        injector: Injector,
        private _profileService: ProfileServiceProxy
    ) {
        super(injector);
    }

    show(): void {
        this.active = true;
        this.modal.show();
    }

    close(): void {
        this.active = false;
        this.modal.hide();
    }

    save(): void {
        this.saving = true;
        this._profileService.verifySmsCode(this.verifyCode)
            .pipe(finalize(() => { this.saving = false; }))
            .subscribe(() => {
                this.close();
                this.modalSave.emit(null);
            });
    }
}
