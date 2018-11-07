import { Component , ViewChild, Injector, Output, EventEmitter, ElementRef, ChangeDetectorRef } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap';
import { ArticleInfoArticleTagInfoServiceProxy, CreateOrUpdateArticleInfoArticleTagInfoDto, ArticleTagInfoEditDto, GetDataComboItemDtoOfInt64 } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import * as _ from 'lodash';
import { Data } from '@angular/router/src/config';
import * as moment from 'moment';
import { finalize } from 'rxjs/operators';

@Component({
    selector: 'createOrEditArticleInfoArticleTagInfoModal',
    templateUrl: './create-or-edit-articleTagInfo-modal.component.html'
})
export class ArticleInfoArticleTagInfoCreateOrEditModalComponent extends AppComponentBase {
    @ViewChild('createOrEditModal') modal: ModalDirective;
    @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();
    active = false;
    saving = false;
    formModel: ArticleTagInfoEditDto = new ArticleTagInfoEditDto();
	articleInfoComboItemDtoList: GetDataComboItemDtoOfInt64[];

    constructor(
        injector: Injector,
        private _articleTagInfoService: ArticleInfoArticleTagInfoServiceProxy,
        private _changeDetector: ChangeDetectorRef,
    ) {
        super(injector);
    }

    onShown(): void {
		this._articleTagInfoService.getArticleInfoDataComboItems()
			.subscribe((result) => {
				this.articleInfoComboItemDtoList = result;
			});
    }

    show(id?: number): void {
        this.formModel = new ArticleTagInfoEditDto();
        this.active = true;
        this._changeDetector.detectChanges();
        if (id) {
            this.formModel.id = id;
            this._articleTagInfoService.getArticleTagInfoForEdit(id).subscribe(result => {
                this.formModel = result.articleTagInfo;
                this.modal.show();
            });
        }else
		    this.modal.show();
    }

    save(): void {
        const createOrEditInput = new CreateOrUpdateArticleInfoArticleTagInfoDto();
		createOrEditInput.articleTagInfo = this.formModel;
		this.saving = true;
		this._articleTagInfoService.createOrUpdateArticleTagInfo(createOrEditInput)
			.pipe(finalize(() => this.saving = false))
			.subscribe(() => {
				this.notify.info(this.l('SavedSuccessfully'));
				this.close();
				this.modalSave.emit(true);
			});
    }

    close(): void {
        this.modal.hide();
        this.active = false;
		this.modalSave.emit(true);
    }
}