import { Injectable, Injector } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import { HubConnection } from '@aspnet/signalr';

@Injectable()
export class ChatSignalrService extends AppComponentBase {

    constructor(
        injector: Injector
    ) {
        super(injector);
    }

    chatHub: HubConnection;
    isChatConnected: boolean = false;

    configureConnection(connection): void {
        // Set the common hub
        this.chatHub = connection;

        // Reconnect if hub disconnects
        connection.onclose(e => {
            this.isChatConnected = false;
            if (e) {
                abp.log.debug('Chat connection closed with error: ' + e);
            }
            else {
                abp.log.debug('Caht disconnected');
            }

            if (!abp.signalr.autoConnect) {
                return;
            }

            setTimeout(() => {
                connection.start().then(result => {
                    this.isChatConnected = true;
                });
            }, 5000);
        });

        // Register to get notifications
        this.registerChatEvents(connection);
    }

    registerChatEvents(connection): void {
        connection.on('getChatMessage', message => {
            abp.event.trigger('app.chat.messageReceived', message);
        });

        connection.on('getAllFriends', friends => {
            abp.event.trigger('abp.chat.friendListChanged', friends);
        });

        connection.on('getFriendshipRequest', (friendData, isOwnRequest) => {
            abp.event.trigger('app.chat.friendshipRequestReceived', friendData, isOwnRequest);
        });

        connection.on('getUserConnectNotification', (friend, isConnected) => {
            abp.event.trigger('app.chat.userConnectionStateChanged',
                {
                    friend: friend,
                    isConnected: isConnected
                });
        });

        connection.on('getUserStateChange', (friend, state) => {
            abp.event.trigger('app.chat.userStateChanged',
                {
                    friend: friend,
                    state: state
                });
        });

        connection.on('getallUnreadMessagesOfUserRead', friend => {
            abp.event.trigger('app.chat.allUnreadMessagesOfUserRead',
                {
                    friend: friend
                });
        });

        connection.on('getReadStateChange', friend => {
            abp.event.trigger('app.chat.readStateChange',
                {
                    friend: friend
                });
        });
    }

    sendMessage(messageData, callback): void {
        if (!this.isChatConnected) {
            if (callback) {
                callback();
            }

            abp.notify.warn(this.l('ChatIsNotConnectedWarning'));
            return;
        }

        this.chatHub.invoke('sendMessage', messageData).then(result => {
            if (result) {
                abp.notify.warn(result);
            }

            if (callback) {
                callback();
            }
        }).catch(error => {
            abp.log.error(error);

            if (callback) {
                callback();
            }
        });
    }

    init(): void {
        abp.signalr.startConnection('/signalr-chat', connection => {
            abp.event.trigger('app.chat.connected');
            this.isChatConnected = true;
            this.configureConnection(connection);
        });
    }
}
