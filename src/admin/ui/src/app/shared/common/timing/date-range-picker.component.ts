import { AfterViewInit, Component, ElementRef, EventEmitter, Injector, Input, Output, ViewChild } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import * as moment from 'moment';

@Component({
    selector: 'date-range-picker',
    template:
    `<input #DateRangePicker type="text" class="form-control" [disabled]="isDisabled"/>`
})
export class DateRangePickerComponent extends AppComponentBase implements AfterViewInit {

    @ViewChild('DateRangePicker') dateRangePickerElement: ElementRef;

    _startDate: moment.Moment = moment().startOf('day');
    _endDate: moment.Moment = moment().startOf('day');

    @Input() isDisabled = false;
    @Input() allowFutureDate = false;
    @Input() isSingleDatePicker = false;

    @Input() dateRangePickerOptions: any = undefined;

    @Output() startDateChange = new EventEmitter();
    @Output() endDateChange = new EventEmitter();

    @Input()
    get startDate() {
        return this._startDate;
    }

    set startDate(val) {
        this._startDate = val;
        this.startDateChange.emit(this._startDate);
    }

    @Input()
    get endDate() {
        return this._endDate;
    }

    set endDate(val) {
        this._endDate = val;
        this.endDateChange.emit(this._endDate);
    }

    constructor(
        injector: Injector,
        private _element: ElementRef
    ) {
        super(injector);
    }

    ngAfterViewInit(): void {
        const $element = $(this.dateRangePickerElement.nativeElement);

        const _selectedDateRange = {
            startDate: this._startDate,
            endDate: this._endDate
        };

        if (!this.dateRangePickerOptions) {
            this.dateRangePickerOptions = {
                singleDatePicker: this.isSingleDatePicker
            };
        }

        $element.daterangepicker(
            $.extend(true, this.createDateRangePickerOptions(), this.dateRangePickerOptions, _selectedDateRange), (start, end, label) => {
                this.startDate = start;
                this.endDate = end;
            });
    }

    createDateRangePickerOptions(): any {
        const self = this;
        const options: any = {
            locale: {
                format: 'L',
                applyLabel: self.l('Apply'),
                cancelLabel: self.l('Cancel'),
                customRangeLabel: self.l('CustomRange')
            },
            ranges: {}
        };

        if (!this.isSingleDatePicker) {
            if (!this.allowFutureDate) {
                options.max = moment();
                options.maxDate = moment();
            }

            options.ranges[self.l('Today')] = [moment().startOf('day'), moment().endOf('day')];
            options.ranges[self.l('Yesterday')] = [moment().subtract(1, 'days').startOf('day'), moment().subtract(1, 'days').endOf('day')];
            options.ranges[self.l('Last7Days')] = [moment().subtract(6, 'days').startOf('day'), moment().endOf('day')];
            options.ranges[self.l('Last30Days')] = [moment().subtract(29, 'days').startOf('day'), moment().endOf('day')];
            options.ranges[self.l('ThisMonth')] = [moment().startOf('month'), moment().endOf('month')];
            options.ranges[self.l('LastMonth')] = [moment().subtract(1, 'month').startOf('month'), moment().subtract(1, 'month').endOf('month')];
        }

        return options;
    }
}
