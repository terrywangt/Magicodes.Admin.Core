import { AfterViewInit, Component, ElementRef, Input } from '@angular/core';
import { AppLocalizationService } from '@app/shared/common/localization/app-localization.service';

@Component({
    selector: '<validation-messages>',
    template: `<div class="has-danger" *ngIf="formCtrl.invalid && formCtrl.dirty">
                    <div *ngFor="let errorDef of errorDefs">
                        <div *ngIf="getErrorDefinitionIsInValid(errorDef)" class="form-control-feedback">
                            {{getErrorDefinitionMessage(errorDef)}}
                        </div>
                    </div>
               </div>`
})
export class ValidationMessagesComponent implements AfterViewInit {

    @Input() formCtrl;
    @Input() errorDefs: any[] = [];

    private _elementRef: ElementRef;
    private _appLocalizationService: AppLocalizationService;

    constructor(
        private elementRef: ElementRef,
        private appLocalizationService: AppLocalizationService
    ) {
        this._elementRef = elementRef;
        this._appLocalizationService = appLocalizationService;
    }

    ngAfterViewInit(): void {
        setTimeout(() => {
            let targetElements = $(this._elementRef.nativeElement).parent().find('[name=\'' + this.formCtrl.name + '\']');
            if (!targetElements || targetElements.length > 1) {
                return;
            }

            let targetElement = $(targetElements[0] as any);

            if (targetElement.attr('required')) {
                this.errorDefs.push({ required: this._appLocalizationService.l('ThisFieldIsRequired') });
            }

            if (targetElement.attr('minlength')) {
                this.errorDefs.push({ minlength: this._appLocalizationService.l('PleaseEnterAtLeastNCharacter', targetElement.attr('minlength')) });
            }

            if (targetElement.attr('maxlength')) {
                this.errorDefs.push({ maxlength: this._appLocalizationService.l('PleaseEnterNoMoreThanNCharacter', targetElement.attr('maxlength')) });
            }
        });
    }

    getErrorDefinitionIsInValid(errorDef: any): boolean {
        return !!this.formCtrl.errors[Object.keys(errorDef)[0]];
    }

    getErrorDefinitionMessage(errorDef: any): any {
        return errorDef[Object.keys(errorDef)[0]];
    }

    addValidationDefinitionIfNotExists(validationKey: string, validationMessage: string): void {
        if (this.errorDefs[validationKey]) {
            return;
        }

        this.errorDefs.push({ validationKey: validationMessage });
    }
}
