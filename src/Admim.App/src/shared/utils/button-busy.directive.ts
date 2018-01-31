import { Directive, ElementRef, Input, OnInit } from '@angular/core';

@Directive({
    selector: '[buttonBusy]'
})
export class ButtonBusyDirective implements OnInit {

    @Input() set buttonBusy(isBusy: boolean){
        this.refreshState(isBusy);
    }

    @Input() busyText: string;

    private _$button: JQuery;
    private _$buttonInnerSpan: JQuery;
    private _$buttonIcon: JQuery;

    constructor(
        private _element: ElementRef
        ) {
    }

    ngOnInit(): void {
        this._$button = $(this._element.nativeElement);
        this._$buttonInnerSpan = this._$button.find('span');
        this._$buttonIcon = this._$button.find('i');
    }

    refreshState(isBusy: boolean): void {
        if (!this._$button) {
            return;
        }

        if (isBusy) {
            // disable button
            this._$button.attr('disabled', 'disabled');

            //change icon
            if (this._$buttonIcon.length) {
                this._$buttonIcon.data('_originalClasses', this._$buttonIcon.attr('class'));
                this._$buttonIcon.removeClass();
                this._$buttonIcon.addClass('fa fa-spin fa-spinner');
            }

            // change text
            if (this.busyText && this._$buttonInnerSpan.length) {
                this._$buttonInnerSpan.data('_originalText', this._$buttonInnerSpan.html());
                this._$buttonInnerSpan.html(this.busyText);
            }

            this._$button.data('_disabledBefore', true);
        } else {
            if (!this._$button.data('_disabledBefore')) {
                return;
            }

            // enable button
            this._$button.removeAttr('disabled');

            // restore icon
            if (this._$buttonIcon.length && this._$buttonIcon.data('_originalClasses')) {
                this._$buttonIcon.removeClass();
                this._$buttonIcon.addClass(this._$buttonIcon.data('_originalClasses'));
            }

            // restore text
            if (this._$buttonInnerSpan.length && this._$buttonInnerSpan.data('_originalText')) {
                this._$buttonInnerSpan.html(this._$buttonInnerSpan.data('_originalText'));
            }
        }
    }
}
