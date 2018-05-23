import { AfterViewInit, Component, Input } from '@angular/core';
import { AppConsts } from '@shared/AppConsts';
import { ProfileServiceProxy } from '@shared/service-proxies/service-proxies';

@Component({
    selector: 'friend-profile-picture',
    template:
    `<img [src]="profilePicture" class="{{cssClass}}" alt="...">`
})
export class FriendProfilePictureComponent implements AfterViewInit {

    @Input() profilePictureId: string;
    @Input() userId: number;
    @Input() tenantId: number;
    @Input() cssClass = 'media-object';

    profilePicture = AppConsts.appBaseUrl + '/assets/common/images/default-profile-picture.png';

    constructor(
        private _profileService: ProfileServiceProxy
    ) {
    }

    ngAfterViewInit(): void {
        this.setProfileImage();
    }

    private setProfileImage(): void {
        if (!this.profilePictureId) {
            this.profilePictureId = undefined;
        }

        if (!this.tenantId) {
            this.tenantId = undefined;
        }

        if (!this.profilePictureId) {
            return;
        }

        this._profileService.getFriendProfilePictureById(this.profilePictureId, this.userId, this.tenantId).subscribe((result) => {
            if (result && result.profilePicture) {
                this.profilePicture = 'data:image/jpeg;base64,' + result.profilePicture;
            }
        });
    }
}
