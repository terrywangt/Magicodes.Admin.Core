import { AfterViewInit, Component, ElementRef, Injector, ViewChild } from '@angular/core';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { AppComponentBase } from '@shared/common/app-component-base';
import { DemoUiComponentsServiceProxy } from '@shared/service-proxies/service-proxies';
import * as moment from 'moment';

@Component({
    selector: 'demo-ui-date-time',
    templateUrl: './demo-ui-date-time.component.html',
    animations: [appModuleAnimation()]
})

export class DemoUiDateTimeComponent extends AppComponentBase implements AfterViewInit {

    @ViewChild('SampleDatePicker') sampleDatePicker: ElementRef;
    @ViewChild('SampleDateTimePicker') sampleDateTimePicker: ElementRef;

    dateRangePickerStartDate: moment.Moment;
    dateRangePickerEndDate: moment.Moment;

    constructor(
        injector: Injector,
        private demoUiComponentsService: DemoUiComponentsServiceProxy
    ) {
        super(injector);
        this.dateRangePickerStartDate = moment().add(-7, 'days').endOf('day');
        this.dateRangePickerEndDate = moment().startOf('day');
    }

    ngAfterViewInit(): void {

        // default date picker
        $(this.sampleDatePicker.nativeElement).datetimepicker({
            locale: abp.localization.currentLanguage.name,
            format: 'L'
        });

        // default date time picker
        $(this.sampleDateTimePicker.nativeElement).datetimepicker({
            locale: abp.localization.currentLanguage.name,
            format: 'L LT'
        });
    }

    // default date picker - post
    submitDate(): void {
        let dateInput = moment($(this.sampleDatePicker.nativeElement).data('DateTimePicker').date().format('YYYY-MM-DDTHH:mm:ssZ'));

        this.demoUiComponentsService.sendAndGetDate(dateInput)
            .subscribe(data => {
                this.message.info(data.dateString, this.l('PostedValue'));
                this.notify.info(this.l('SavedSuccessfully'));
            });
    }

    // default date time picker - post
    submitDateTime(): void {
        let dateInput = moment($(this.sampleDateTimePicker.nativeElement).data('DateTimePicker').date().format('YYYY-MM-DDTHH:mm:ssZ'));

        this.demoUiComponentsService.sendAndGetDateTime(dateInput)
            .subscribe(data => {
                this.message.info(data.dateString, this.l('PostedValue'));
                this.notify.info(this.l('SavedSuccessfully'));
            });
    }

    // default date range picker - post
    submitDateRange(): void {
        this.demoUiComponentsService.sendAndGetDateRange(this.dateRangePickerStartDate, this.dateRangePickerEndDate)
            .subscribe(data => {
                this.message.info(data.dateString, this.l('PostedValue'));
                this.notify.info(this.l('SavedSuccessfully'));
            });
    }
}
