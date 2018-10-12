import { Component, ElementRef, EventEmitter, Injector, Input, OnInit, Output, ViewChild } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import { GetEnumValuesListDto, CommonServiceProxy } from '@shared/service-proxies/service-proxies';

@Component({
    selector: 'enum-combo',
    template:
        `<select #EnumCombobox
        class="form-control"
        [(ngModel)]="selectedValue"
        (ngModelChange)="selectedValueChange.emit($event)"
        [attr.data-live-search]="true"
        jq-plugin="selectpicker">
            <option value="">{{emptyText}}</option>
            <option *ngFor="let item of values" [value]="item.value">{{item.displayName}}</option>
    </select>`
})
export class EnumComboComponent extends AppComponentBase implements OnInit {
    @ViewChild('EnumCombobox') EnumComboboxElement: ElementRef;
    //选择的值
    @Input() selectedValue: number = null;
    @Output() selectedValueChange: EventEmitter<string> = new EventEmitter<string>();
    //空文本提示
    @Input() emptyText = this.l('EmptyTextTip');
    //枚举类型全名
    @Input() fullName = '';

    values: GetEnumValuesListDto[] = [];

    constructor(
        private _commonService: CommonServiceProxy,
        injector: Injector) {
        super(injector);
    }

    ngOnInit(): void {
        const self = this;
        this._commonService.getEnumValuesList(this.fullName).subscribe(result => {
            this.values = result;
            setTimeout(() => {
                //$(self.EnumComboboxElement.nativeElement).selectpicker('refresh');
            }, 0);
        });
    }
}
