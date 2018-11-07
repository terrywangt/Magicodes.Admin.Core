import {
    Component,
    ViewChild,
    Injector,
    Output,
    EventEmitter,
    ElementRef,
    ChangeDetectorRef
} from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap';
import {
    ArticleInfoServiceProxy,
    CreateOrUpdateArticleInfoDto,
    ArticleInfoEditDto,
    GetDataComboItemDtoOfInt64
} from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import * as _ from 'lodash';
import { Data } from '@angular/router/src/config';
import * as moment from 'moment';
import { finalize } from 'rxjs/operators';

@Component({
    selector: 'createOrEditArticleInfoModal',
    templateUrl: './create-or-edit-articleInfo-modal.component.html'
})
export class CreateOrEditArticleInfoModalComponent extends AppComponentBase {
    @ViewChild('ReleaseTimeDateUtc')
    releaseTimeDateUtc: ElementRef;
    @ViewChild('createOrEditModal')
    modal: ModalDirective;
    @Output()
    modalSave: EventEmitter<any> = new EventEmitter<any>();
    active = false;
    saving = false;
    formModel: ArticleInfoEditDto = new ArticleInfoEditDto();
    columnInfoComboItemDtoList: GetDataComboItemDtoOfInt64[];
    articleSourceInfoComboItemDtoList: GetDataComboItemDtoOfInt64[];
    releaseTimeDateUtcx: moment.Moment = moment().startOf('day');
    releaseTimeDateIsValid = false;

    constructor(
        injector: Injector,
        private _articleInfoService: ArticleInfoServiceProxy,
        private _changeDetector: ChangeDetectorRef
    ) {
        super(injector);
    }

    onShown(): void {
        if (this.formModel.releaseTime) {
            (this.releaseTimeDateUtc
                .nativeElement as any).value = this.formModel.releaseTime.format(
                'L'
            );
        }
        this._articleInfoService
            .getColumnInfoDataComboItems()
            .subscribe(result => {
                this.columnInfoComboItemDtoList = result;
            });
        this._articleInfoService
            .getArticleSourceInfoDataComboItems()
            .subscribe(result => {
                this.articleSourceInfoComboItemDtoList = result;
            });
    }

    show(id?: number): void {
        this.formModel = new ArticleInfoEditDto();
        this.active = true;
        this._changeDetector.detectChanges();
        this.formModel.id = id;
        this._articleInfoService.getArticleInfoForEdit(id).subscribe(result => {
            this.formModel = result.articleInfo;
            console.log(this.formModel);
            this.modal.show();
        });
    }

    releaseTimeDateChange(e): void {
        this.releaseTimeDateIsValid = e && e.date !== false;
    }

    save(): void {
        const createOrEditInput = new CreateOrUpdateArticleInfoDto();
        createOrEditInput.articleInfo = this.formModel;
        this.saving = true;
        this._articleInfoService
            .createOrUpdateArticleInfo(createOrEditInput)
            .pipe(finalize(() => (this.saving = false)))
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
