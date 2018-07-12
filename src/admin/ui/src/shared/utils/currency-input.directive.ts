import { AfterViewInit, Directive, ElementRef, EventEmitter, Output, Renderer2 } from '@angular/core';

@Directive({ selector: '[currencyInput]' })
export class CurrencyInputDirective implements AfterViewInit {

    @Output() ngModelChange: EventEmitter<number> = new EventEmitter(false);

    private _isInitialized = false;

    constructor(private _element: ElementRef, private renderer: Renderer2) {

    }

    ngAfterViewInit() {
        if (this._isInitialized) {
            return;
        }

        $(this._element.nativeElement).inputmask('numeric', {
            radixPoint: '.',
            groupSeparator: ',',
            digits: 2,
            autoGroup: true,
            prefix: '$ ',
            rightAlign: false,
            showMaskOnHover: false,
            showMaskOnFocus: false,
            placeholder: '_',
            removeMaskOnSubmit: true,
            autoUnmask: true,
            oncomplete: () => {
                this.ngModelChange.emit(this._element.nativeElement.value);
            }
        });

        this._isInitialized = true;
    }
}
