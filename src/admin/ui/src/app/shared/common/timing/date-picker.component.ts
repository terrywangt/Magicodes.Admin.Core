import { AfterViewInit, Directive, ElementRef, EventEmitter, Injector, Input, Output } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import * as moment from 'moment';

@Directive({
    selector: '[datePicker]'
})
export class DatePickerDirective extends AppComponentBase implements AfterViewInit {

    hostElement: ElementRef;

    _selectedDate: moment.Moment = moment().startOf('day');
    @Output() selectedDateChange = new EventEmitter();


    @Input()
    get selectedDate() {
        return this._selectedDate;
    }

    set selectedDate(val) {
        this._selectedDate = val;
        this.selectedDateChange.emit(this._selectedDate);
        this.setElementText(val);
    }

    constructor(
        injector: Injector,
        private _element: ElementRef
    ) {
        super(injector);
        this.hostElement = _element;
    }

    ngAfterViewInit(): void {
        const $element = $(this.hostElement.nativeElement);
        $element.datetimepicker({
            locale: abp.localization.currentLanguage.name,
            format: 'L'
        }).on('dp.change', e => {
            if (e.date) {
                this.selectedDate = moment(e.date);
            } else {
                this.selectedDate = null;
            }
        });
    }

    setElementText(val: any) {
        const $element = $(this.hostElement.nativeElement);
        if (val) {
            $element.val(moment(val).format('L'));
        } else {
            $element.val('');
        }
    }
}
