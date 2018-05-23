import { ChatMessageDto, FriendDto } from '@shared/service-proxies/service-proxies';

export class ChatFriendDto extends FriendDto {
    messages: ChatMessageDto[];
    allPreviousMessagesLoaded = false;
    messagesLoaded = false;
}
