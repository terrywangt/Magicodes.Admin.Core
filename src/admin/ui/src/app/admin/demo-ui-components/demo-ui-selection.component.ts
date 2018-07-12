import { Component, Injector } from '@angular/core';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { AppComponentBase } from '@shared/common/app-component-base';
import { DemoUiComponentsServiceProxy, NameValueOfString } from '@shared/service-proxies/service-proxies';

@Component({
    selector: 'demo-ui-selection',
    templateUrl: './demo-ui-selection.component.html',
    animations: [appModuleAnimation()]
})

export class DemoUiSelectionComponent extends AppComponentBase {

    filteredCountries: NameValueOfString[];
    country: any;
    countries: NameValueOfString[] = new Array<NameValueOfString>();

    constructor(
        injector: Injector,
        private demoUiComponentsService: DemoUiComponentsServiceProxy
    ) {
        super(injector);
    }

    // get countries
    filterCountries(event): void {
        this.demoUiComponentsService.getCountries(event.query).subscribe(countries => {
            this.filteredCountries = countries;
        });
    }

    // single select - post
    submitSelectedCountry(): void {
        let selectedCountries = new Array<NameValueOfString>();

        selectedCountries.push(this.country);

        this.demoUiComponentsService.sendAndGetSelectedCountries(selectedCountries)
            .subscribe((countries: NameValueOfString[]) => {
                let message = '';

                $.each(countries, (index, item) => {
                    message += `<div><strong>id</strong>: ${item.value} - <strong>name</strong>: ${item.name}</div>`;
                });

                let $div = document.createElement('div');
                $div.innerHTML = message;

                (abp as any).libs.sweetAlert.config.info.content = $div;
                this.message.info('', this.l('PostedValue'));
                this.notify.info(this.l('SavedSuccessfully'));
                (abp as any).libs.sweetAlert.config.info.content = '';
            });
    }

    // multi select - post
    submitSelectedCountries(): void {
        this.demoUiComponentsService.sendAndGetSelectedCountries(this.countries)
            .subscribe((countries: NameValueOfString[]) => {
                let message = '';

                $.each(countries, (index, item) => {
                    message += `<div><strong>id</strong>: ${item.value} - <strong>name</strong>: ${item.name}</div>`;
                });

                let $div = document.createElement('div');
                $div.innerHTML = message;

                (abp as any).libs.sweetAlert.config.info.content = $div;
                this.message.info('', this.l('PostedValue'));
                this.notify.info(this.l('SavedSuccessfully'));
                (abp as any).libs.sweetAlert.config.info.content = '';
            });
    }
}
