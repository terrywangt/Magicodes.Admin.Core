import { AfterViewInit, Component, EventEmitter, Injector, OnInit, Output, ViewChild, ViewEncapsulation } from '@angular/core';
import { CommonLookupModalComponent } from '@app/shared/common/lookup/common-lookup-modal.component';
import { AppConsts } from '@shared/AppConsts';
import { AppChatMessageReadState, AppChatSide, AppFriendshipState } from '@shared/AppEnums';
import { AppComponentBase } from '@shared/common/app-component-base';
import { AppSessionService } from '@shared/common/session/app-session.service';
import { DomHelper } from '@shared/helpers/DomHelper';
import { BlockUserInput, ChatMessageDtoSide, ChatServiceProxy, CommonLookupServiceProxy, CreateFriendshipRequestByUserNameInput, CreateFriendshipRequestInput, FindUsersInput, FriendDto, FriendDtoState, FriendshipServiceProxy, MarkAllUnreadMessagesOfUserAsReadInput, NameValueDto, ProfileServiceProxy, UnblockUserInput, UserLoginInfoDto } from '@shared/service-proxies/service-proxies';
import { LocalStorageService } from '@shared/utils/local-storage.service';
import { QuickSideBarChat } from 'app/shared/layout/chat/QuickSideBarChat';
import * as _ from 'lodash';
import * as moment from 'moment';
import { ChatFriendDto } from './ChatFriendDto';
import { ChatSignalrService } from './chat-signalr.service';

@Component({
    templateUrl: './chat-bar.component.html',
    selector: 'chat-bar',
    styleUrls: ['./chat-bar.component.less'],
    encapsulation: ViewEncapsulation.None
})
export class ChatBarComponent extends AppComponentBase implements OnInit, AfterViewInit {

    @Output() onProgress: EventEmitter<any> = new EventEmitter();

    public progress = 0;
    uploadUrl: string;
    isFileSelected = false;


    @ViewChild('userLookupModal') userLookupModal: CommonLookupModalComponent;
    $_chatMessageInput: JQuery;

    friendDtoState: typeof AppFriendshipState = AppFriendshipState;

    friends: ChatFriendDto[];
    currentUser: UserLoginInfoDto = this._appSessionService.user;
    profilePicture = AppConsts.appBaseUrl + '/assets/common/images/default-profile-picture.png';
    chatMessage = '';

    tenantToTenantChatAllowed = false;
    tenantToHostChatAllowed = false;
    interTenantChatAllowed = false;
    sendingMessage = false;
    loadingPreviousUserMessages = false;
    userNameFilter = '';
    serverClientTimeDifference = 0;
    isMultiTenancyEnabled: boolean = this.multiTenancy.isEnabled;
    appChatSide: typeof AppChatSide = AppChatSide;
    appChatMessageReadState: typeof AppChatMessageReadState = AppChatMessageReadState;

    _isOpen: boolean;
    set isOpen(newValue: boolean) {
        if (newValue === this._isOpen) {
            return;
        }

        this._localStorageService.setItem('app.chat.isOpen', newValue);
        this._isOpen = newValue;

        if (newValue) {
            this.markAllUnreadMessagesOfUserAsRead(this.selectedUser);
        }

        this.adjustNotifyPosition();
    }
    get isOpen(): boolean {
        return this._isOpen;
    }

    _pinned = false;
    set pinned(newValue: boolean) {
        if (newValue === this._pinned) {
            return;
        }

        this._pinned = newValue;
        this._localStorageService.setItem('app.chat.pinned', newValue);
    }
    get pinned(): boolean {
        return this._pinned;
    }

    _selectedUser: ChatFriendDto = new ChatFriendDto();
    set selectedUser(newValue: ChatFriendDto) {
        if (newValue === this._selectedUser) {
            return;
        }

        this._selectedUser = newValue;

        //NOTE: this is a fix for localForage is not able to store user with messages array filled
        if (newValue.messages) {
            newValue.messages = [];
            newValue.messagesLoaded = false;
        }
        this._localStorageService.setItem('app.chat.selectedUser', newValue);
    }
    get selectedUser(): ChatFriendDto {
        return this._selectedUser;
    }


    constructor(injector: Injector,
        private _appSessionService: AppSessionService,
        private _friendshipService: FriendshipServiceProxy,
        private _chatService: ChatServiceProxy,
        private _commonLookupService: CommonLookupServiceProxy,
        private _localStorageService: LocalStorageService,
        private _chatSignalrService: ChatSignalrService,
        private _profileService: ProfileServiceProxy,
        private _quickSideBarChat: QuickSideBarChat) {
        super(injector);
        this.uploadUrl = AppConsts.remoteServiceBaseUrl + '/Chat/UploadFile';
    }

    shareCurrentLink() {
        this.chatMessage = '[link]{"message":"' + window.location.href + '"}';
        this.sendMessage();
        $('#chatDropdownToggle').dropdown('toggle');
    }

    onFileSelect() {
        $('#chatDropdownToggle').dropdown('toggle');
    }

    onUploadImage(event): void {

        const jsonResult = JSON.parse(event.xhr.response);
        this.chatMessage = '[image]{"id":"' + jsonResult.result.id + '", "name":"' + jsonResult.result.name + '", "contentType":"' + jsonResult.result.contentType + '"}';
        this.sendMessage();

        this.isFileSelected = false;
        this.progress = 0;
    }

    onUploadFile(event): void {
        const jsonResult = JSON.parse(event.xhr.response);
        this.chatMessage = '[file]{"id":"' + jsonResult.result.id + '", "name":"' + jsonResult.result.name + '", "contentType":"' + jsonResult.result.contentType + '"}';
        this.sendMessage();

        this.isFileSelected = false;
        this.progress = 0;
    }

    onBeforeSend(event): void {
        this.isFileSelected = true;
        event.xhr.setRequestHeader('Authorization', 'Bearer ' + abp.auth.getToken());

        event.xhr.upload.addEventListener('progress', (e: ProgressEvent) => {
            if (e.lengthComputable) {
                this.progress = Math.round((e.loaded * 100) / e.total);
            }

            this.onProgress.emit({ originalEvent: e, progress: this.progress });
        }, false);
    }

    ngOnInit(): void {
        this.init();
    }

    getShownUserName(tenanycName: string, userName: string): string {
        if (!this.isMultiTenancyEnabled) {
            return userName;
        }

        return (tenanycName ? tenanycName : '.') + '\\' + userName;
    }

    getProfilePicture(): void {
        this._profileService.getProfilePicture().subscribe(result => {
            if (result && result.profilePicture) {
                this.profilePicture = 'data:image/jpeg;base64,' + result.profilePicture;
            }
        });
    }

    block(user: FriendDto): void {
        const blockUserInput = new BlockUserInput();
        blockUserInput.tenantId = user.friendTenantId;
        blockUserInput.userId = user.friendUserId;

        this._friendshipService.blockUser(blockUserInput).subscribe(() => {
            this.notify.info(this.l('UserBlocked'));
        });
    }

    unblock(user: FriendDto): void {
        const unblockUserInput = new UnblockUserInput();
        unblockUserInput.tenantId = user.friendTenantId;
        unblockUserInput.userId = user.friendUserId;

        this._friendshipService.unblockUser(unblockUserInput).subscribe(() => {
            this.notify.info(this.l('UserUnblocked'));
        });
    }

    markAllUnreadMessagesOfUserAsRead(user: ChatFriendDto): void {
        if (!user || !this.isOpen) {
            return;
        }

        const unreadMessages = _.filter(user.messages, m => m.readState === AppChatMessageReadState.Unread);
        const unreadMessageIds = _.map(unreadMessages, 'id');

        if (!unreadMessageIds.length) {
            return;
        }

        const input = new MarkAllUnreadMessagesOfUserAsReadInput();
        input.tenantId = user.friendTenantId;
        input.userId = user.friendUserId;

        this._chatService.markAllUnreadMessagesOfUserAsRead(input).subscribe(() => {
            _.forEach(user.messages, message => {
                if (unreadMessageIds.indexOf(message.id) >= 0) {
                    message.readState = AppChatMessageReadState.Read;
                }
            });
        });
    }

    loadMessages(user: ChatFriendDto, callback: any): void {
        this.loadingPreviousUserMessages = true;

        let minMessageId;
        if (user.messages && user.messages.length) {
            minMessageId = _.min(_.map(user.messages, m => m.id));
        }

        this._chatService.getUserChatMessages(user.friendTenantId ? user.friendTenantId : undefined, user.friendUserId, minMessageId)
            .subscribe(result => {
                if (!user.messages) {
                    user.messages = [];
                }

                user.messages = result.items.concat(user.messages);

                this.markAllUnreadMessagesOfUserAsRead(user);

                if (!result.items.length) {
                    user.allPreviousMessagesLoaded = true;
                }

                this.loadingPreviousUserMessages = false;
                if (callback) {
                    callback();
                }
            });
    }

    openSearchModal(userName: string, tenantId?: number): void {
        this.userLookupModal.filterText = userName;
        this.userLookupModal.show();
    }

    addFriendSelected(item: NameValueDto): void {
        const userId = item.value;
        const input = new CreateFriendshipRequestInput();
        input.userId = parseInt(userId);
        input.tenantId = this._appSessionService.tenant ? this._appSessionService.tenant.id : null;

        this._friendshipService.createFriendshipRequest(input).subscribe(() => {
            this.userNameFilter = '';
        });
    }

    search(): void {
        const input = new CreateFriendshipRequestByUserNameInput();

        if (this.userNameFilter.indexOf('\\') === -1) {
            input.userName = this.userNameFilter;
        } else {
            const tenancyAndUserNames = this.userNameFilter.split('\\');
            input.tenancyName = tenancyAndUserNames[0];
            input.userName = tenancyAndUserNames[1];
        }

        if (!input.tenancyName || !this.interTenantChatAllowed) {
            const tenantId = this._appSessionService.tenant ? this._appSessionService.tenant.id : null;
            this.openSearchModal(input.userName, tenantId);
        } else {
            this._friendshipService.createFriendshipRequestByUserName(input).subscribe(() => {
                this.userNameFilter = '';
            });
        }
    }

    getFriendOrNull(userId: number, tenantId?: number): ChatFriendDto {
        const friends = _.filter(this.friends, friend => friend.friendUserId === userId && friend.friendTenantId === tenantId);
        if (friends.length) {
            return friends[0];
        }

        return null;
    }

    getFilteredFriends(state: FriendDtoState, userNameFilter: string): FriendDto[] {
        const foundFriends = _.filter(this.friends, friend => friend.state === state &&
            this.getShownUserName(friend.friendTenancyName, friend.friendUserName)
                .toLocaleLowerCase()
                .indexOf(userNameFilter.toLocaleLowerCase()) >= 0);

        return foundFriends;
    }

    getFilteredFriendsCount(state: FriendDtoState): number {
        return _.filter(this.friends, friend => friend.state === state).length;
    }

    getUserNameByChatSide(chatSide: ChatMessageDtoSide): string {
        return chatSide === AppChatSide.Sender ?
            this.currentUser.userName :
            this.selectedUser.friendUserName;
    }

    getFixedMessageTime(messageTime: moment.Moment): string {
        return moment(messageTime).add(-1 * this.serverClientTimeDifference, 'seconds').format('YYYY-MM-DDTHH:mm:ssZ');
    }

    changeNotifyPosition(positionClass: string): void {
        if (!toastr) {
            return;
        }

        toastr.clear();
        toastr.options.positionClass = positionClass;
    }

    getFriendsAndSettings(callback: any): void {
        this._chatService.getUserChatFriendsWithSettings().subscribe(result => {
            this.friends = (result.friends as ChatFriendDto[]);
            this.serverClientTimeDifference = moment(abp.clock.now()).diff(result.serverTime, 'seconds');

            this.triggerUnreadMessageCountChangeEvent();
            callback();
        });
    }

    scrollToBottom(): void {
        setTimeout(() => {
            this.scrollToBottomInternal();
        }, 100);
    }

    scrollToBottomInternal(): void {
        DomHelper.waitUntilElementIsVisible('.m-messenger-conversation .m-messenger__messages', () => {
            setTimeout(() => {
                const $scrollArea = $('.m-messenger-conversation .m-messenger__messages');
                const scrollToVal = $scrollArea.prop('scrollHeight') + 'px';
                $scrollArea.slimScroll({ scrollTo: scrollToVal }); 
            });
        });
    }

    loadLastState(): void {
        const self = this;
        self._localStorageService.getItem('app.chat.isOpen', (err, isOpen) => {
            self.isOpen = isOpen;

            self._localStorageService.getItem('app.chat.pinned', (err, pinned) => {
                self.pinned = pinned;
            });

            if (isOpen) {
                self._quickSideBarChat.show();
                self._localStorageService.getItem('app.chat.selectedUser', (err, selectedUser) => {
                    if (selectedUser && selectedUser.friendUserId) {
                        self.showMessagesPanel();
                        self.selectFriend(selectedUser);
                    } else {
                        self.showFriendsPanel();
                    }
                });
            }
        });
    }

    selectFriend(friend: ChatFriendDto): void {
        const chatUser = this.getFriendOrNull(friend.friendUserId, friend.friendTenantId);
        this.selectedUser = chatUser;

        if (!chatUser) {
            return;
        }

        this.chatMessage = '';

        this.showMessagesPanel();

        if (!chatUser.messagesLoaded) {
            this.loadMessages(chatUser, () => {
                chatUser.messagesLoaded = true;
                this.scrollToBottom();
                this.$_chatMessageInput.focus();
            });
        } else {
            this.markAllUnreadMessagesOfUserAsRead(this.selectedUser);
            this.scrollToBottom();
            this.$_chatMessageInput.focus();
        }
    }

    showMessagesPanel(): void {
        $('.m-messenger-friends').hide();
        $('.m-messenger-conversation').show(() => {
            this.initConversationScrollbar();
        });
        $('#m_quick_sidebar_back').removeClass('d-none');
    }

    showFriendsPanel(): void {
        $('.m-messenger-friends').show();
        $('.m-messenger-conversation').hide();
        $('#m_quick_sidebar_back').addClass('d-none');
    }

    initConversationScrollbar(): void {
        let $messengerMessages = $('.m-messenger-conversation .m-messenger__messages');
        let height = $('#m_quick_sidebar').outerHeight(true) - $('.selected-chat-user').outerHeight(true) - $('#ChatMessage').height() - 150;

        $messengerMessages.slimScroll({ destroy: true });
        $messengerMessages.slimScroll({
            height: height
        });
    }

    sendMessage(): void {
        if (!this.chatMessage) {
            return;
        }

        this.sendingMessage = true;
        const tenancyName = this._appSessionService.tenant ? this._appSessionService.tenant.tenancyName : null;
        this._chatSignalrService.sendMessage({
            tenantId: this.selectedUser.friendTenantId,
            userId: this.selectedUser.friendUserId,
            message: this.chatMessage,
            tenancyName: tenancyName,
            userName: this._appSessionService.user.userName,
            profilePictureId: this._appSessionService.user.profilePictureId
        }, () => {
            this.chatMessage = '';
            this.sendingMessage = false;
        });
    }

    reversePinned(): void {
        this.pinned = !this.pinned;
    }

    bindUiEvents(): void {
        const self = this;
        self._quickSideBarChat.init((e, pos) => {
            if (pos === 0 && !this.selectedUser.allPreviousMessagesLoaded && !this.loadingPreviousUserMessages) {
                self.loadMessages(self.selectedUser, null);
            }
        });

        const $backToList = $('#m_quick_sidebar_back');
        $backToList.on('click', () => {
            self.selectedUser = new ChatFriendDto();
            this.showFriendsPanel();
        });

        const $sidebarTogglers = $('#m_quick_sidebar_toggle');
        $sidebarTogglers.on('click', () => {
            this.isOpen = $('body').hasClass('m-quick-sidebar--on');
        });

        $('div.m-quick-sidebar').on('mouseleave', () => {
            if (this.pinned) {
                return;
            }

            self._quickSideBarChat.hide();
            this.isOpen = false;
            this.adjustNotifyPosition();
        });

        $(window as any).on('resize', () => {
            this.initConversationScrollbar();
        });
    }

    ngAfterViewInit(): void {
        this.$_chatMessageInput = $('#ChatMessage');
    }

    adjustNotifyPosition(): void {
        if (this.isOpen) {
            this.changeNotifyPosition('toast-chat-open');
        } else {
            this.changeNotifyPosition('toast-bottom-right');
        }
    }

    triggerUnreadMessageCountChangeEvent(): void {
        let totalUnreadMessageCount = 0;

        if (this.friends) {
            totalUnreadMessageCount = _.reduce(this.friends, (memo, friend) => memo + friend.unreadMessageCount, 0);
        }

        abp.event.trigger('app.chat.unreadMessageCountChanged', totalUnreadMessageCount);
    }

    registerEvents(): void {
        const self = this;

        abp.event.on('app.chat.messageReceived', message => {
            const user = this.getFriendOrNull(message.targetUserId, message.targetTenantId);
            if (!user) {
                return;
            }

            user.messages = user.messages || [];
            user.messages.push(message);

            if (message.side === AppChatSide.Receiver) {
                user.unreadMessageCount += 1;
                message.readState = AppChatMessageReadState.Unread;
                this.triggerUnreadMessageCountChangeEvent();

                if (this.isOpen && this.selectedUser !== null && user.friendTenantId === this.selectedUser.friendTenantId && user.friendUserId === this.selectedUser.friendUserId) {
                    this.markAllUnreadMessagesOfUserAsRead(user);
                } else {
                    this.notify.info(
                        abp.utils.formatString('{0}: {1}', user.friendUserName, abp.utils.truncateString(message.message, 100)),
                        null,
                        {
                            onclick() {
                                if (!$('body').hasClass('m-quick-sidebar--on')) {
                                    self._quickSideBarChat.show();
                                    self.isOpen = true;
                                }

                                self.showMessagesPanel();

                                self.selectFriend(user);
                                self.pinned = true;
                            }
                        });
                }
            }

            self.scrollToBottom();
        });

        abp.event.on('app.chat.friendshipRequestReceived', (data, isOwnRequest) => {
            if (!isOwnRequest) {
                abp.notify.info(this.l('UserSendYouAFriendshipRequest', data.friendUserName));
            }

            if (!_.filter(this.friends, f => f.friendUserId === data.friendUserId && f.friendTenantId === data.friendTenantId).length) {
                this.friends.push(data);
            }
        });

        abp.event.on('app.chat.userConnectionStateChanged', data => {
            const user = this.getFriendOrNull(data.friend.userId, data.friend.tenantId);
            if (!user) {
                return;
            }

            user.isOnline = data.isConnected;
        });

        abp.event.on('app.chat.userStateChanged', data => {
            const user = this.getFriendOrNull(data.friend.userId, data.friend.tenantId);
            if (!user) {
                return;
            }

            user.state = data.state;
        });

        abp.event.on('app.chat.allUnreadMessagesOfUserRead', data => {
            const user = this.getFriendOrNull(data.friend.userId, data.friend.tenantId);
            if (!user) {
                return;
            }

            user.unreadMessageCount = 0;
            this.triggerUnreadMessageCountChangeEvent();
        });

        abp.event.on('app.chat.readStateChange', data => {
            const user = this.getFriendOrNull(data.friend.userId, data.friend.tenantId);
            if (!user) {
                return;
            }

            $.each(user.messages,
                (index, message) => {
                    message.receiverReadState = AppChatMessageReadState.Read;
                });
        });

        abp.event.on('app.chat.connected', () => {
            const self = this;
            self.getFriendsAndSettings(() => {
                DomHelper.waitUntilElementIsReady('#m_quick_sidebar, #m_quick_sidebar_toggle', () => {
                    self.bindUiEvents();

                    DomHelper.waitUntilElementIsReady('.m-quick-sidebar', () => {
                        self.loadLastState();
                    });
                });
            });
        });
    }

    init(): void {
        this.registerEvents();
        this.userLookupModal.configure({
            title: this.l('SelectAUser'),
            dataSource: (skipCount: number, maxResultCount: number, filter: string, tenantId?: number) => {
                const input = new FindUsersInput();
                input.filter = filter;
                input.maxResultCount = maxResultCount;
                input.skipCount = skipCount;
                input.tenantId = tenantId;
                return this._commonLookupService.findUsers(input);
            }
        });

        this.getProfilePicture();

        this.tenantToTenantChatAllowed = this.feature.isEnabled('App.ChatFeature.TenantToTenant');
        this.tenantToHostChatAllowed = this.feature.isEnabled('App.ChatFeature.TenantToHost');
        this.interTenantChatAllowed = this.feature.isEnabled('App.ChatFeature.TenantToTenant') || this.feature.isEnabled('App.ChatFeature.TenantToHost') || !this._appSessionService.tenant;
    }
}
