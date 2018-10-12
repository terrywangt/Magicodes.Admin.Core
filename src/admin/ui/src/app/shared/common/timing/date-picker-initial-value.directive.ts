import { AfterViewInit, Directive, ElementRef, EventEmitter, Injector, Input, Output } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';

@Directive({
    selector: '[datePickerInitialValue]'
})
export class DatePickerInitialValueSetterDirective extends AppComponentBase implements AfterViewInit {

    hostElement: ElementRef;
    @Input() ngModel;

    constructor(
        injector: Injector,
        private _element: ElementRef
    ) {
        super(injector);
        this.hostElement = _element;
    }

    ngAfterViewInit(): void {
        if (this.ngModel) {
            setTimeout(() => {
                (this.hostElement.nativeElement as any).value = this.ngModel.format('L');
            });
        }
    }
}
