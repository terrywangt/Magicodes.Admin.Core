import { IAjaxResponse } from '@abp/abpHttpInterceptor';
import { TokenService } from '@abp/auth/token.service';
import { Component, Injector, ViewChild } from '@angular/core';
import { AppConsts } from '@shared/AppConsts';
import { AppComponentBase } from '@shared/common/app-component-base';
import { ProfileServiceProxy, UpdateProfilePictureInput } from '@shared/service-proxies/service-proxies';
import { FileUploader, FileUploaderOptions } from 'ng2-file-upload';
import { ModalDirective } from 'ngx-bootstrap';
import { finalize } from 'rxjs/operators';

@Component({
    selector: 'changeProfilePictureModal',
    templateUrl: './change-profile-picture-modal.component.html'
})
export class ChangeProfilePictureModalComponent extends AppComponentBase {

    @ViewChild('changeProfilePictureModal') modal: ModalDirective;

    public active = false;
    public uploader: FileUploader;
    public temporaryPictureUrl: string;
    public saving = false;

    private maxProfilPictureBytesUserFriendlyValue = 5;
    private temporaryPictureFileName: string;
    private _uploaderOptions: FileUploaderOptions = {};
    private _$profilePictureResize: JQuery;
    private _$jcropApi: any;

    constructor(
        injector: Injector,
        private _profileService: ProfileServiceProxy,
        private _tokenService: TokenService
    ) {
        super(injector);
    }

    initializeModal(): void {
        this.active = true;
        this.temporaryPictureUrl = '';
        this.temporaryPictureFileName = '';
        this._$profilePictureResize = null;
        this._$jcropApi = null;
        this.initFileUploader();
    }

    initFileUploader(): void {
        const self = this;
        self.uploader = new FileUploader({ url: AppConsts.remoteServiceBaseUrl + '/Profile/UploadProfilePicture' });
        self._uploaderOptions.autoUpload = true;
        self._uploaderOptions.authToken = 'Bearer ' + self._tokenService.getToken();
        self._uploaderOptions.removeAfterUpload = true;
        self.uploader.onAfterAddingFile = (file) => {
            file.withCredentials = false;
        };

        self.uploader.onSuccessItem = (item, response, status) => {
            const resp = <IAjaxResponse>JSON.parse(response);
            if (resp.success) {
                self.temporaryPictureFileName = resp.result.fileName;
                self.temporaryPictureUrl = AppConsts.remoteServiceBaseUrl + '/Temp/Downloads/' + resp.result.fileName + '?v=' + new Date().valueOf();

                const newCanvasHeight = resp.result.height * self._$profilePictureResize.width() / resp.result.width;
                self._$profilePictureResize.height(newCanvasHeight + 'px');

                if (self._$jcropApi) {
                    self._$jcropApi.destroy();
                }

                self._$profilePictureResize.attr('src', self.temporaryPictureUrl);
                self._$profilePictureResize.attr('originalWidth', resp.result.width);
                self._$profilePictureResize.attr('originalHeight', resp.result.height);

                self._$profilePictureResize.Jcrop({
                    setSelect: [0, 0, 100, 100],
                    aspectRatio: 1,
                    boxWidth: 400,
                    boxHeight: 400
                }, function () {
                    self._$jcropApi = this;
                });

            } else {
                this.message.error(resp.error.message);
            }
        };

        self.uploader.setOptions(self._uploaderOptions);
    }

    onModalShown() {
        this._$profilePictureResize = $('#ProfilePictureResize');
    }

    show(): void {
        this.initializeModal();
        this.modal.show();
    }

    close(): void {
        this.active = false;
        this.modal.hide();
    }

    save(): void {
        const self = this;
        if (!self.temporaryPictureFileName) {
            return;
        }

        let resizeParams = { x: 0, y: 0, w: 0, h: 0 };
        if (self._$jcropApi) {
            resizeParams = self._$jcropApi.getSelection();
        }
        
        const containerWidth = Math.ceil(self._$jcropApi.getContainerSize()[0]);
        const containerHeight = Math.ceil(self._$jcropApi.getContainerSize()[1]);

        let originalWidth = containerWidth;
        let originalHeight = containerHeight;

        if (self._$profilePictureResize) {
            originalWidth = parseInt(self._$profilePictureResize.attr('originalWidth'));
            originalHeight = parseInt(self._$profilePictureResize.attr('originalHeight'));
        }

        const widthRatio = originalWidth / containerWidth;
        const heightRatio = originalHeight / containerHeight;

        const input = new UpdateProfilePictureInput();
        input.fileName = self.temporaryPictureFileName;
        input.x = Math.floor(resizeParams.x * widthRatio);
        input.y = Math.floor(resizeParams.y * heightRatio);
        input.width = Math.floor(resizeParams.w * widthRatio);
        input.height = Math.floor(resizeParams.h * heightRatio);

        this.saving = true;
        self._profileService.updateProfilePicture(input)
            .pipe(finalize(() => { this.saving = false; }))
            .subscribe(() => {
                const self = this;
                self._$jcropApi.destroy();
                self._$jcropApi = null;
                abp.event.trigger('profilePictureChanged');
                self.close();
            });
    }
}
