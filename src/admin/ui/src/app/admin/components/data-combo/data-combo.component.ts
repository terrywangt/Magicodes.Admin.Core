import { Component, ElementRef, EventEmitter, Injector, Input, OnInit, Output, ViewChild, SimpleChanges } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';

@Component({
    selector: 'data-combo',
    template:
        `<select #DataCombobox
        class="form-control"
        [(ngModel)]="selectedValue"
        (ngModelChange)="selectedValueChange.emit($event)"
        [attr.data-live-search]="true"
        jq-plugin="selectpicker">
            <option value="">{{emptyText}}</option>
            <option *ngFor="let item of values" [value]="item.value">{{item.displayName}}</option>
    </select>`
})
export class DataComboComponent extends AppComponentBase implements OnInit {
    @ViewChild('DataCombobox') DataComboboxElement: ElementRef;
    //选择的值
    @Input() selectedValue: number = null;
    @Input() values: any[] = [];
    @Output() selectedValueChange: EventEmitter<string> = new EventEmitter<string>();
    //空文本提示
    @Input() emptyText = this.l('EmptyTextTip');

    constructor(injector: Injector) {
        super(injector);
    }

    ngOnInit(): void {
        // setTimeout(() => {
        //     $(this.DataComboboxElement.nativeElement).selectpicker('refresh');
        // }, 0);
    }
    ngOnChanges(changes: SimpleChanges) {
        if (changes.values && changes.values.previousValue) {
            // setTimeout(() => {
            //     $(this.DataComboboxElement.nativeElement).selectpicker('refresh');
            // }, 0);
        }
    }
}
