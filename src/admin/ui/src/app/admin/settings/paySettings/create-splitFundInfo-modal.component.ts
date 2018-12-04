import {
    Component,
    ViewChild,
    Injector,
    Output,
    EventEmitter
} from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap';
import { SplitFundSettingDto } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import * as _ from 'lodash';

@Component({
    selector: 'createSplitFundInfoModal',
    templateUrl: './create-splitFundInfo-modal.component.html'
})
export class CreateSplitFundInfoModalComponent extends AppComponentBase {
    @ViewChild('createModal') modal: ModalDirective;
    @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();
    active = false;
    saving = false;
    formModel: SplitFundSettingDto;
    splitFunds: SplitFundSettingDto[];

    constructor(injector: Injector) {
        super(injector);
    }

    onShown(): void {}

    show(splitFunds: SplitFundSettingDto[]): void {
        this.active = true;
        this.splitFunds = splitFunds;
        this.formModel = new SplitFundSettingDto();
        this.modal.show();
    }

    save(): void {
        let splitFund = this.splitFunds.find(
            p => p.transIn == this.formModel.transIn
        );
        if (splitFund != null) {
            this.message.warn(this.l('TransInExistPleaseReplace'));
        } else {
            this.notify.info(this.l('CreateSuccessfully'));
            this.close();
            this.modalSave.emit(this.formModel);
        }
    }

    close(): void {
        this.modal.hide();
        this.active = false;
        this.modalSave.emit(null);
    }
}
