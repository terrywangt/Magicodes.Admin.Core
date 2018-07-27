import { Component , ViewChild, Injector, Output, EventEmitter, ElementRef, ChangeDetectorRef } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap';
import { ArticleSourceInfoServiceProxy, CreateOrUpdateArticleSourceInfoDto, ArticleSourceInfoEditDto} from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import * as _ from 'lodash';
import { Data } from '@angular/router/src/config';
import * as moment from 'moment';
import { finalize } from 'rxjs/operators';

@Component({
    selector: 'createOrEditArticleSourceInfoModal',
    templateUrl: './create-or-edit-articleSourceInfo-modal.component.html'
})
export class CreateOrEditArticleSourceInfoModalComponent extends AppComponentBase {   
    @ViewChild('createOrEditModal') modal: ModalDirective;
    @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();
    active = false;
    saving = false;

    formModel: ArticleSourceInfoEditDto = new ArticleSourceInfoEditDto();

    constructor(
        injector: Injector,
        private _articleSourceInfoService: ArticleSourceInfoServiceProxy,
        private _changeDetector: ChangeDetectorRef,
    ) {
        super(injector);
    }

    onShown(): void {
    }

    show(id?: number): void {
        this.formModel = new ArticleSourceInfoEditDto();
        this.active = true;
        this._changeDetector.detectChanges();
        this.formModel.id = id;
        this._articleSourceInfoService.getArticleSourceInfoForEdit(id).subscribe(result => {
            this.formModel = result.articleSourceInfo;     
            this.modal.show();               
        });
    }

    save(): void {
        const createOrEditInput = new CreateOrUpdateArticleSourceInfoDto();
		createOrEditInput.articleSourceInfo = this.formModel;
		this.saving = true;
		this._articleSourceInfoService.createOrUpdateArticleSourceInfo(createOrEditInput)
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