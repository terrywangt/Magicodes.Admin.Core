import { Component, Injector, ViewEncapsulation, ViewChild, ElementRef } from '@angular/core';
import { Router } from '@angular/router';
import { LanguageServiceProxy, ApplicationLanguageListDto, SetDefaultLanguageInput } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import { CreateOrEditLanguageModalComponent } from './create-or-edit-language-modal.component';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { DataTable } from 'primeng/components/datatable/datatable';
import { Paginator } from 'primeng/components/paginator/paginator';

@Component({
    templateUrl: './languages.component.html',
    encapsulation: ViewEncapsulation.None,
    animations: [appModuleAnimation()]
})
export class LanguagesComponent extends AppComponentBase {

    @ViewChild('languagesTable') languagesTable: ElementRef;
    @ViewChild('createOrEditLanguageModal') createOrEditLanguageModal: CreateOrEditLanguageModalComponent;
    @ViewChild('dataTable') dataTable: DataTable;
    @ViewChild('paginator') paginator: Paginator;

    defaultLanguageName: string;
    private _$languagesTable: JQuery;

    constructor(
        injector: Injector,
        private _languageService: LanguageServiceProxy,
        private _router: Router
    ) {
        super(injector);
    }

    getLanguages(): void {
        this.primengDatatableHelper.showLoadingIndicator();

        this._languageService.getLanguages().subscribe(result => {
            this.defaultLanguageName = result.defaultLanguageName;
            this.primengDatatableHelper.records = result.items;
            this.primengDatatableHelper.totalRecordsCount = result.items.length;
            this.primengDatatableHelper.hideLoadingIndicator();
        });
    }

    changeTexts(language: ApplicationLanguageListDto): void {
        this._router.navigate(['app/admin/languages', language.name, 'texts']);
    }

    setAsDefaultLanguage(language: ApplicationLanguageListDto): void {
        const input = new SetDefaultLanguageInput();
        input.name = language.name;
        this._languageService.setDefaultLanguage(input).subscribe(() => {
            this.getLanguages();
            this.notify.success(this.l('SuccessfullySaved'));
        });
    }

    deleteLanguage(language: ApplicationLanguageListDto): void {
        this.message.confirm(
            this.l('LanguageDeleteWarningMessage', language.displayName),
            isConfirmed => {
                if (isConfirmed) {
                    this._languageService.deleteLanguage(language.id).subscribe(() => {
                        this.getLanguages();
                        this.notify.success(this.l('SuccessfullyDeleted'));
                    });
                }
            }
        );
    }
}
