import { Component, ViewChild, Injector, Output, EventEmitter, ElementRef } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap';
import { LanguageServiceProxy, ApplicationLanguageEditDto, CreateOrUpdateLanguageInput, ComboboxItemDto } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';

import * as _ from 'lodash';

@Component({
    selector: 'createOrEditLanguageModal',
    templateUrl: './create-or-edit-language-modal.component.html'
})
export class CreateOrEditLanguageModalComponent extends AppComponentBase {

    @ViewChild('createOrEditModal') modal: ModalDirective;
    @ViewChild('languageCombobox') languageCombobox: ElementRef;
    @ViewChild('iconCombobox') iconCombobox: ElementRef;

    @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

    active = false;
    saving = false;

    language: ApplicationLanguageEditDto = new ApplicationLanguageEditDto();

    languageNames: ComboboxItemDto[] = [];
    flags: ComboboxItemDto[] = [];

    constructor(
        injector: Injector,
        private _languageService: LanguageServiceProxy
    ) {
        super(injector);
    }

    show(languageId?: number): void {
        this.active = true;

        this._languageService.getLanguageForEdit(languageId).subscribe(result => {
            this.language = result.language;
            this.languageNames = result.languageNames;
            this.flags = result.flags;

            if (!languageId) {
                this.language.isEnabled = true;
            }

            this.modal.show();
            setTimeout(() => {
                    $(this.languageCombobox.nativeElement).selectpicker('refresh');
                    $(this.iconCombobox.nativeElement).selectpicker('refresh');
            }, 0);
        });
    }

    save(): void {
        let input = new CreateOrUpdateLanguageInput();
        input.language = this.language;

        this.saving = true;
        this._languageService.createOrUpdateLanguage(input)
            .finally(() => this.saving = false)
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
