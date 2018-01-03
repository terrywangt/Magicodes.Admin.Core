import { Component, Injector } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { DemoUiComponentsServiceProxy } from '@shared/service-proxies/service-proxies';

@Component({
    selector: 'demo-ui-input-mask',
    templateUrl: './demo-ui-input-mask.component.html',
    animations: [appModuleAnimation()]
})

export class DemoUiInputMaskComponent extends AppComponentBase {

    dateValue = '';
    phoneValue = '';
    serialValue = '';
    phoneExtValue = '';

    constructor(
        injector: Injector,
        private demoUiComponentsService: DemoUiComponentsServiceProxy
    ) {
        super(injector);
    }

    submitDateValue(): void {
        this.submitValue(this.dateValue);
    }

    submiPhoneValue(): void {
        this.submitValue(this.phoneValue);
    }

    submitSerialValue(): void {
        this.submitValue(this.serialValue);
    }

    submitPhoneExtValue(): void {
        this.submitValue(this.phoneExtValue);
    }

    // input mask - post
    submitValue(value: any): void {

        this.demoUiComponentsService.sendAndGetValue(value)
            .subscribe(data => {
                this.message.info(data.output, this.l('PostedValue'));
                this.notify.info(this.l('SavedSuccessfully'));
            });
    }
}
