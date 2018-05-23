import { Component, ElementRef, EventEmitter, Injector, Input, OnInit, Output, ViewChild } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import { DefaultTimezoneScope, NameValueDto, TimingServiceProxy } from '@shared/service-proxies/service-proxies';

@Component({
    selector: 'timezone-combo',
    template:
    `<select #TimeZoneCombobox
        class="form-control"
        [(ngModel)]="selectedTimeZone"
        (ngModelChange)="selectedTimeZoneChange.emit($event)">
            <option *ngFor="let timeZone of timeZones" [value]="timeZone.value">{{timeZone.name}}</option>
    </select>`
})
export class TimeZoneComboComponent extends AppComponentBase implements OnInit {

    @ViewChild('TimeZoneCombobox') timeZoneComboboxElement: ElementRef;
    @Output() selectedTimeZoneChange: EventEmitter<string> = new EventEmitter<string>();

    timeZones: NameValueDto[] = [];

    @Input() selectedTimeZone: string = undefined;
    @Input() defaultTimezoneScope: DefaultTimezoneScope;

    constructor(
        private _timingService: TimingServiceProxy,
        injector: Injector) {
        super(injector);
    }

    ngOnInit(): void {
        let self = this;
        self._timingService.getTimezones(self.defaultTimezoneScope).subscribe(result => {
            self.timeZones = result.items;
        });
    }
}
