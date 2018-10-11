import { Component, ElementRef, Input } from '@angular/core';
import { AppLocalizationService } from '@app/shared/common/localization/app-localization.service';

class ErrorDef {
    error: string;
    localizationKey: string;
    errorProperty: string;
}

@Component({
    selector: '<validation-messages>',
    template: `<div class="has-danger" *ngIf="formCtrl.invalid && (formCtrl.dirty || formCtrl.touched)">
                    <div *ngFor="let errorDef of errorDefs">
                        <div *ngIf="getErrorDefinitionIsInValid(errorDef)" class="form-control-feedback">
                            {{getErrorDefinitionMessage(errorDef)}}
                        </div>
                    </div>
               </div>`
})
export class ValidationMessagesComponent {

    @Input() formCtrl;

    errorDefs: ErrorDef[];

    private _elementRef: ElementRef;
    private _appLocalizationService: AppLocalizationService;

    constructor(
        private elementRef: ElementRef,
        private appLocalizationService: AppLocalizationService
    ) {
        this._elementRef = elementRef;
        this._appLocalizationService = appLocalizationService;
        this.errorDefs = [
            { error: 'required', localizationKey: 'ThisFieldIsRequired' } as ErrorDef,
            { error: 'minlength', localizationKey: 'PleaseEnterAtLeastNCharacter', errorProperty: 'requiredLength' } as ErrorDef,
            { error: 'maxlength', localizationKey: 'PleaseEnterNoMoreThanNCharacter', errorProperty: 'requiredLength' } as ErrorDef,
            { error: 'email', localizationKey: 'InvalidEmailAddress' } as ErrorDef,
            { error: 'pattern', localizationKey: 'InvalidPattern', errorProperty: 'requiredPattern' } as ErrorDef
        ];
    }

    getErrorDefinitionIsInValid(errorDef: ErrorDef): boolean {
        return !!this.formCtrl.errors[errorDef.error];
    }

    getErrorDefinitionMessage(errorDef: ErrorDef): string {
        let errorRequirement = this.formCtrl.errors[errorDef.error][errorDef.errorProperty];
        return !!errorRequirement
            ? this._appLocalizationService.l(errorDef.localizationKey, errorRequirement)
            : this._appLocalizationService.l(errorDef.localizationKey);
    }
}

