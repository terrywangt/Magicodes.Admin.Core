import { Component, ElementRef, EventEmitter, Injector, Output, ViewChild } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import { LinkToUserInput, UserLinkServiceProxy } from '@shared/service-proxies/service-proxies';
import { ModalDirective } from 'ngx-bootstrap';
import { finalize } from 'rxjs/operators';

@Component({
    selector: 'linkAccountModal',
    templateUrl: './link-account-modal.component.html'
})
export class LinkAccountModalComponent extends AppComponentBase {

    @ViewChild('linkAccountModal') modal: ModalDirective;

    @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

    active = false;
    saving = false;

    linkUser: LinkToUserInput = new LinkToUserInput();

    constructor(
        injector: Injector,
        private _userLinkService: UserLinkServiceProxy
    ) {
        super(injector);
    }

    show(): void {
        this.active = true;
        this.linkUser = new LinkToUserInput();
        this.linkUser.tenancyName = this.appSession.tenancyName;
        this.modal.show();
    }

    onShown(): void {
        document.getElementById('TenancyName').focus();
    }

    save(): void {
        this.saving = true;
        this._userLinkService.linkToUser(this.linkUser)
            .pipe(finalize(() => { this.saving = false; }))
            .subscribe(() => {
                this.notify.info(this.l('SavedSuccessfully'));
                this.close();
                this.modalSave.emit(null);
            });
    }

    close(): void {
        this.active = false;
        this.modal.hide();
    }
}
