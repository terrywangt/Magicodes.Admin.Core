import { AfterViewInit, Directive, ElementRef, EventEmitter, Injector, Input, Output } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import * as moment from 'moment';

@Directive({
    selector: '[dateRangePickerInitialValue]'
})
export class DateRangePickerInitialValueSetterDirective extends AppComponentBase implements AfterViewInit {

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
        if (this.ngModel && this.ngModel[0] && this.ngModel[1]) {
            setTimeout(() => {
                (this.hostElement.nativeElement as any).value = moment(this.ngModel[0]).format('L') + ' - ' + moment(this.ngModel[1]).format('L');
            });
        }
    }
}
