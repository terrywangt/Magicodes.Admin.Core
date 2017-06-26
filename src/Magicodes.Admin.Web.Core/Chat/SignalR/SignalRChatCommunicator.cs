#if FEATURE_SIGNALR
using System.Collections.Generic;
using Abp;
using Abp.AutoMapper;
using Abp.Dependency;
using Abp.ObjectMapping;
using Abp.RealTime;
using Castle.Core.Logging;
using Microsoft.AspNet.SignalR;
using Magicodes.Admin.Chat;
using Magicodes.Admin.Chat.Dto;
using Magicodes.Admin.Friendships;
using Magicodes.Admin.Friendships.Dto;

namespace Magicodes.Admin.Web.Chat.SignalR
{
    public class SignalRChatCommunicator : IChatCommunicator, ITransientDependency
    {
        /// <summary>
        /// Reference to the logger.
        /// </summary>
        public ILogger Logger { get; set; }

        private readonly IObjectMapper _objectMapper;

        private static IHubContext ChatHub => GlobalHost.ConnectionManager.GetHubContext<ChatHub>();

        public SignalRChatCommunicator(IObjectMapper objectMapper)
        {
            _objectMapper = objectMapper;
            Logger = NullLogger.Instance;
        }

        public void SendMessageToClient(IReadOnlyList<IOnlineClient> clients, ChatMessage message)
        {
            foreach (var client in clients)
            {
                var signalRClient = GetSignalRClientOrNull(client);
                if (signalRClient == null)
                {
                    return;
                }

                signalRClient.getChatMessage(_objectMapper.Map<ChatMessageDto>(message));
            }
        }

        public void SendFriendshipRequestToClient(IReadOnlyList<IOnlineClient> clients, Friendship friendship, bool isOwnRequest, bool isFriendOnline)
        {
            foreach (var client in clients)
            {
                var signalRClient = GetSignalRClientOrNull(client);
                if (signalRClient == null)
                {
                    return;
                }

                var friendshipRequest = _objectMapper.Map<FriendDto>(friendship);
                friendshipRequest.IsOnline = isFriendOnline;

                signalRClient.getFriendshipRequest(friendshipRequest, isOwnRequest);
            }
        }

        public void SendUserConnectionChangeToClients(IReadOnlyList<IOnlineClient> clients, UserIdentifier user, bool isConnected)
        {
            foreach (var client in clients)
            {
                var signalRClient = GetSignalRClientOrNull(client);
                if (signalRClient == null)
                {
                    continue;
                }

                signalRClient.getUserConnectNotification(user, isConnected);
            }
        }

        public void SendUserStateChangeToClients(IReadOnlyList<IOnlineClient> clients, UserIdentifier user, FriendshipState newState)
        {
            foreach (var client in clients)
            {
                var signalRClient = GetSignalRClientOrNull(client);
                if (signalRClient == null)
                {
                    continue;
                }

                signalRClient.getUserStateChange(user, newState);
            }
        }

        public void SendAllUnreadMessagesOfUserReadToClients(IReadOnlyList<IOnlineClient> clients, UserIdentifier user)
        {
            foreach (var client in clients)
            {
                var signalRClient = GetSignalRClientOrNull(client);
                if (signalRClient == null)
                {
                    continue;
                }

                signalRClient.getallUnreadMessagesOfUserRead(user);
            }
        }

        private dynamic GetSignalRClientOrNull(IOnlineClient client)
        {
            var signalRClient = ChatHub.Clients.Client(client.ConnectionId);
            if (signalRClient == null)
            {
                Logger.Debug("Can not get chat user " + client.UserId + " from SignalR hub!");
                return null;
            }

            return signalRClient;
        }
    }
}
#endif