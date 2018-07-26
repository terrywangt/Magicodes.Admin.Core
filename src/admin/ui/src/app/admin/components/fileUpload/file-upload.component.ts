import { Component, Injector, Input } from '@angular/core';
import { AppConsts } from '@shared/AppConsts';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { AppComponentBase } from '@shared/common/app-component-base';
import { CommonServiceProxy, AddObjectAttachmentInfosInput } from '@shared/service-proxies/service-proxies';
import { finalize } from 'rxjs/operators';

@Component({
    selector: 'file-upload',
    templateUrl: './file-upload.component.html',
    animations: [appModuleAnimation()]
})

export class FileUploadComponent extends AppComponentBase {
    @Input() input: AddObjectAttachmentInfosInput | null;
    uploadUrl: string;
    uploadedFiles: any[] = [];

    constructor(
        injector: Injector,
        private _commonService: CommonServiceProxy
    ) {
        super(injector);
        this.uploadUrl = AppConsts.remoteServiceBaseUrl + '/Attachment/UploadFiles';
    }

    // upload completed event
    onUpload(event): void {
        for (const file of event.files) {
            this.uploadedFiles.push(file);
        }
        if (this.input) {

            var data = JSON.parse(event.xhr.responseText);
            this.input.attachmentInfoIds = [];
            data["result"].forEach(element => {
                this.input.attachmentInfoIds.push(element["id"]);
            });

            console.log(this.input);

            this._commonService.addObjectAttachmentInfos(this.input)
                .pipe()
                .subscribe(() => {

                });
        }
    }

    onBeforeSend(event): void {
        event.xhr.setRequestHeader('Authorization', 'Bearer ' + abp.auth.getToken());
    }
}
