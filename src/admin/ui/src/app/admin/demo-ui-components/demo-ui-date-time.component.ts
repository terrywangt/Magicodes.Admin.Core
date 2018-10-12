import { Component, ElementRef, Injector, ViewChild } from '@angular/core';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { AppComponentBase } from '@shared/common/app-component-base';
import { DemoUiComponentsServiceProxy } from '@shared/service-proxies/service-proxies';
import * as moment from 'moment';

@Component({
    selector: 'demo-ui-date-time',
    templateUrl: './demo-ui-date-time.component.html',
    animations: [appModuleAnimation()]
})

export class DemoUiDateTimeComponent extends AppComponentBase {

    @ViewChild('SampleDatePicker') sampleDatePicker: ElementRef;
    @ViewChild('SampleDateTimePicker') sampleDateTimePicker: ElementRef;

    sampleDate: moment.Moment;
    sampleDateRange: moment.Moment[] = [moment().add(-7, 'days').endOf('day'), moment().startOf('day')];

    constructor(
        injector: Injector,
        private demoUiComponentsService: DemoUiComponentsServiceProxy
    ) {
        super(injector);
    }

    // default date picker - post
    submitDate(): void {
        this.demoUiComponentsService.sendAndGetDate(this.sampleDate)
            .subscribe(data => {
                this.message.info(data.dateString, this.l('PostedValue'));
            });
    }

    // default date range picker - post
    submitDateRange(): void {
        this.demoUiComponentsService.sendAndGetDateRange(this.sampleDateRange[0], this.sampleDateRange[1])
            .subscribe(data => {
                this.message.info(data.dateString, this.l('PostedValue'));
            });
    }
}
