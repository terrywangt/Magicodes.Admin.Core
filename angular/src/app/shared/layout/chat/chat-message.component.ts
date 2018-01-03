import { Component, Input, OnInit } from '@angular/core';
import { ChatServiceProxy, ChatMessageDto } from '@shared/service-proxies/service-proxies';
import {AppConsts} from 'shared/AppConsts';

@Component({
    selector: 'chat-message',
    templateUrl: './chat-message.component.html'
})
export class ChatMessageComponent implements OnInit {

    @Input()
    message: ChatMessageDto;

    chatMessage: string;
    chatMessageType: string;
    fileName: string;
    fileContentType: string;

    constructor(private _chatService: ChatServiceProxy) {
    }

    ngOnInit(): void {
        this.setChatMessageType();
        $('#chatImageUpload').find('.fa-plus').addClass('fa-camera').removeClass('fa-plus');
        $('#chatFileUpload').find('.fa-plus').addClass('fa-file').removeClass('fa-plus');
    }

    private setChatMessageType(): void {
        if (this.message.message.startsWith('[image]')) {
            this.chatMessageType = 'image';

            let image = JSON.parse(this.message.message.substring('[image]'.length));
            this.chatMessage = AppConsts.remoteServiceBaseUrl +
                '/Chat/GetUploadedObject?fileId=' +
                image.id +
                '&contentType=' +
                image.contentType;

        } else if (this.message.message.startsWith('[file]')) {
            this.chatMessageType = 'file';

            let file = JSON.parse(this.message.message.substring('[file]'.length));
            this.chatMessage = AppConsts.remoteServiceBaseUrl +
                '/Chat/GetUploadedObject?fileId=' +
                file.id +
                '&contentType=' +
                file.contentType;
            this.fileName = file.name;

        } else if (this.message.message.startsWith('[link]')) {
            this.chatMessageType = 'link';

            let linkMessage = JSON.parse(this.message.message.substring('[link]'.length));

            this.chatMessage = linkMessage.message == null ? '' : linkMessage.message;
        } else {
            this.chatMessageType = 'text';

            this.chatMessage = this.message.message;
        }
    }

}
