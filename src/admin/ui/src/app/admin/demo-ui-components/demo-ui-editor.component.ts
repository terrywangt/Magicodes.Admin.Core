import { Component, Injector } from '@angular/core';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { AppComponentBase } from '@shared/common/app-component-base';
import { DemoUiComponentsServiceProxy } from '@shared/service-proxies/service-proxies';

@Component({
    selector: 'demo-ui-editor',
    templateUrl: './demo-ui-editor.component.html',
    animations: [appModuleAnimation()]
})

export class DemoUiEditorComponent extends AppComponentBase {

    htmlEditorInput: string;

    constructor(
        injector: Injector,
        private demoUiComponentsService: DemoUiComponentsServiceProxy
    ) {
        super(injector);
    }

    // input mask - post
    submitValue(): void {

        this.demoUiComponentsService.sendAndGetValue(this.htmlEditorInput)
            .subscribe(data => {
                (abp as any).libs.sweetAlert.config.info.html = true;
                this.message.info(data.output, this.l('PostedValue'));
                this.notify.info(this.l('SavedSuccessfully'));
            });
    }
}
