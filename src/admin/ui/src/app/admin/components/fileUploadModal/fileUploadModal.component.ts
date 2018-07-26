import { Component, ViewChild, Injector, Output, EventEmitter, ChangeDetectorRef, Input } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap';
import { CommonServiceProxy, AddObjectAttachmentInfosInput } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import { finalize } from 'rxjs/operators';

@Component({
    selector: 'fileUploadModal',
    templateUrl: './fileUploadModal.component.html',
    styleUrls: ['./fileUploadModal.component.less']
})
export class FileUploadModalComponent extends AppComponentBase {
    @ViewChild('fileUploadModal') modal: ModalDirective;
    @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();
    @Input() objectTypeName: string;
    images: any[];
    active = false;
    saving = false;
    input: AddObjectAttachmentInfosInput = new AddObjectAttachmentInfosInput();
    constructor(
        injector: Injector,
        private _commonService: CommonServiceProxy
    ) {
        super(injector);
    }

    onShown(): void {

    }

    show(id: number): void {
        this.active = true;
        console.log(id);
        this.input.objectId = id;
        this.input.objectType = this.objectTypeName;
        this.input.attachmentInfoIds = [];

        this.load();
        this.modal.show();
    }

    load(): void {
        this.images = [];
        this._commonService.getObjectImages(this.input.objectType, this.input.objectId)
            .pipe()
            .subscribe((result) => {
                result.forEach(element => {
                    this.images.push({ source: element.url, thumbnail: element.url, title: element.name });
                });
            });
    }

    close(): void {
        this.modal.hide();
        this.active = false;
    }
}