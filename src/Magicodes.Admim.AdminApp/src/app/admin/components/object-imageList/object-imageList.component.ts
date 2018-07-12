import { Component, ViewChild, Injector, Output, EventEmitter, ChangeDetectorRef, Input } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap';
import { CommonServiceProxy, AddObjectAttachmentInfosInput } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import { finalize } from 'rxjs/operators';

@Component({
    selector: 'object-imageList',
    templateUrl: './object-imageList.component.html',
    styleUrls: ['./object-imageList.component.less']
})
export class ObjectImageListComponent extends AppComponentBase {
    @Input() objectTypeName: string;
    @Input() objectId: number;
    @ViewChild('op') op: ModalDirective;
    images: any[] = [];
    overlayModel: any;
    active = false;
    saving = false;
    constructor(
        injector: Injector,
        private _commonService: CommonServiceProxy
    ) {
        super(injector);
    }
    ngOnInit() {
        this.load();
    }

    load(): void {
        this.images = [];
        this._commonService.getObjectImages(this.objectTypeName, this.objectId)
            .pipe()
            .subscribe((result) => {
                result.forEach(element => {
                    this.images.push({ source: element.url, thumbnail: element.url, title: element.name });
                });
            });
    }

    show(img: any): void {
        this.overlayModel = img;
        this.op.show();
    }
}