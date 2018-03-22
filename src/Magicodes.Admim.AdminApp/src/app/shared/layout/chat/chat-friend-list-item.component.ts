import { Injector } from '@angular/core';
import { Component, Input, Output, EventEmitter } from '@angular/core';
import { ChatFriendDto } from './ChatFriendDto';
import { AppConsts } from '@shared/AppConsts';
import { AbpMultiTenancyService } from '@abp/multi-tenancy/abp-multi-tenancy.service';

@Component({
    templateUrl: './chat-friend-list-item.component.html',
    selector: 'chat-friend-list-item'
})
export class ChatFriendListItemComponent {

    remoteServiceUrl: string = AppConsts.remoteServiceBaseUrl;
    appPath: string = AppConsts.appBaseUrl;

    @Input() friend: ChatFriendDto;
    @Output() selectChatFriend: EventEmitter<string> = new EventEmitter<string>();

    multiTenancy: AbpMultiTenancyService;

    constructor(injector: Injector) {
        this.multiTenancy = injector.get(AbpMultiTenancyService);
    }

    getShownUserName(tenanycName: string, userName: string): string {
        if (!this.multiTenancy.isEnabled) {
            return userName;
        }
        return (tenanycName ? tenanycName : '.') + '\\' + userName;
    }

    getRemoteImageUrl(profilePictureId: string, userId: number, tenantId?: number): string {
        return this.remoteServiceUrl + '/Profile/GetFriendProfilePictureById?id=' + profilePictureId + '&userId=' + userId + '&tenantId=' + tenantId;
    }
}
