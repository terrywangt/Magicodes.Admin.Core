import { Directive, ElementRef, Input, OnInit, AfterViewInit } from '@angular/core';
import { AppLocalizationService } from '@app/shared/common/localization/app-localization.service';
import * as _ from 'lodash';

@Directive({
    selector: '[buttonBusy]'
})
export class ButtonBusyDirective implements OnInit, AfterViewInit {

    @Input() set buttonBusy(isBusy: boolean) {
        this.refreshState(isBusy);
    }

    @Input() busyText: string;

    private _button: any;
    private _originalButtonInnerHtml: any;

    constructor(
        private _element: ElementRef,
        private _appLocalizationService: AppLocalizationService
    ) {
    }

    ngOnInit(): void {
        this._button = this._element.nativeElement;
    }

    ngAfterViewInit(): void {
        this._originalButtonInnerHtml = this._button.innerHTML;
    }

    refreshState(isBusy: boolean): void {
        if (!this._button) {
            return;
        }

        if (isBusy) {
            // disable button
            this._button.setAttribute('disabled', 'disabled');

            this._button.innerHTML = '<i class="fa fa-spin fa-spinner"></i>' +
                '<span>' + (this.busyText ? this.busyText : this._appLocalizationService.l('ProcessingWithThreeDot')) + '</span>';

            this._button.setAttribute('_disabledBefore', true);
        } else {
            if (!this._button.getAttribute('_disabledBefore')) {
                return;
            }

            // enable button
            this._button.removeAttribute('disabled');
            this._button.innerHTML = this._originalButtonInnerHtml;
        }
    }
}
