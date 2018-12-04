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
    selector: 'editSplitFundInfoModal',
    templateUrl: './edit-splitFundInfo-modal.component.html'
})
export class EditSplitFundInfoModalComponent extends AppComponentBase {
    @ViewChild('editModal') modal: ModalDirective;
    @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();
    active = false;
    saving = false;
    formModel: SplitFundSettingDto;
    splitFunds: SplitFundSettingDto[];
    transIn: string;

    constructor(injector: Injector) {
        super(injector);
    }

    onShown(): void {}

    show(splitFunds: SplitFundSettingDto[], dto: SplitFundSettingDto): void {
        this.splitFunds = splitFunds;
        this.transIn = dto.transIn;
        this.active = true;
        this.formModel = dto;
        this.modal.show();
    }

    save(): void {
        let splitFund: any;
        if (this.formModel.transIn != this.transIn) {
            splitFund = this.splitFunds.find(
                p => p.transIn == this.formModel.transIn
            );
        }
        if (splitFund != null) {
            this.message.warn(this.l('TransInExistPleaseReplace'));
        } else {
            this.notify.info(this.l('EditSuccessfully'));
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
