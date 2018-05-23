import { Component, Injector } from '@angular/core';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { AppComponentBase } from '@shared/common/app-component-base';

@Component({
    templateUrl: './demo-ui-components.component.html',
    animations: [appModuleAnimation()]
})
export class DemoUiComponentsComponent extends AppComponentBase {
    constructor(
        injector: Injector
    ) {
        super(injector);
    }
}
