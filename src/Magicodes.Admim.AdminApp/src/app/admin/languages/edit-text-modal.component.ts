import { Component, ElementRef, EventEmitter, Injector, Output, ViewChild } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import { LanguageServiceProxy, UpdateLanguageTextInput } from '@shared/service-proxies/service-proxies';
import * as _ from 'lodash';
import { ModalDirective } from 'ngx-bootstrap';
import { finalize } from 'rxjs/operators';

@Component({
    selector: 'editTextModal',
    templateUrl: './edit-text-modal.component.html'
})
export class EditTextModalComponent extends AppComponentBase {

    @ViewChild('targetValueInput') targetValueInput: ElementRef;
    @ViewChild('modal') modal: ModalDirective;

    @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

    model: UpdateLanguageTextInput = new UpdateLanguageTextInput();

    baseText: string;
    baseLanguage: abp.localization.ILanguageInfo;
    targetLanguage: abp.localization.ILanguageInfo;

    active = false;
    saving = false;

    constructor(
        injector: Injector,
        private _languageService: LanguageServiceProxy
    ) {
        super(injector);
    }

    show(baseLanguageName: string, targetLanguageName: string, sourceName: string, key: string, baseText: string, targetText: string): void {
        this.model.sourceName = sourceName;
        this.model.key = key;
        this.model.languageName = targetLanguageName;
        this.model.value = targetText;

        this.baseText = baseText;
        this.baseLanguage = _.find(abp.localization.languages, l => l.name === baseLanguageName);
        this.targetLanguage = _.find(abp.localization.languages, l => l.name === targetLanguageName);

        this.active = true;

        this.modal.show();
    }

    onShown(): void {
        $(this.targetValueInput.nativeElement).focus();
    }

    save(): void {
        this.saving = true;
        this._languageService.updateLanguageText(this.model)
            .pipe(finalize(() => this.saving = false))
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

    private findLanguage(name: string): abp.localization.ILanguageInfo {
        return _.find(abp.localization.languages, l => l.name === name);
    }
}
