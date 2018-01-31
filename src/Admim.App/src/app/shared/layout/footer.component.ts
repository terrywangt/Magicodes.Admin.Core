import { Component, Injector, OnInit } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';

@Component({
    templateUrl: './footer.component.html',
    selector: 'footer-bar'
})
export class FooterComponent extends AppComponentBase implements OnInit {

    releaseDate: string;

    constructor(
        injector: Injector
    ) {
        super(injector);
    }

    ngOnInit(): void {
        this.releaseDate = this.appSession.application.releaseDate.format('YYYYMMDD');
    }
}
