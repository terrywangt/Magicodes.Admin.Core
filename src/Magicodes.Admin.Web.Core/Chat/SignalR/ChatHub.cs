#if FEATURE_SIGNALR
using System;
using System.Threading.Tasks;
using Abp;
using Abp.Dependency;
using Abp.Localization;
using Abp.Runtime.Session;
using Abp.UI;
using Castle.Core.Logging;
using Castle.Windsor;
using Microsoft.AspNet.SignalR;
using Magicodes.Admin.Chat;

namespace Magicodes.Admin.Web.Chat.SignalR
{
    public class ChatHub : Hub, ITransientDependency
    {
        /// <summary>
        /// Reference to the logger.
        /// </summary>
        public ILogger Logger { get; set; }

        /// <summary>
        /// Reference to the session.
        /// </summary>
        public IAbpSession AbpSession { get; set; }

        private readonly IChatMessageManager _chatMessageManager;
        private readonly ILocalizationManager _localizationManager;
        private readonly IWindsorContainer _windsorContainer;
        private bool _isCallByRelease;

        /// <summary>
        /// Initializes a new instance of the <see cref="ChatHub"/> class.
        /// </summary>
        public ChatHub(
            IChatMessageManager chatMessageManager,
            ILocalizationManager localizationManager, 
            IWindsorContainer windsorContainer)
        {
            _chatMessageManager = chatMessageManager;
            _localizationManager = localizationManager;
            _windsorContainer = windsorContainer;

            Logger = NullLogger.Instance;
            AbpSession = NullAbpSession.Instance;
        }

        public async Task<string> SendMessage(SendChatMessageInput input)
        {
            var sender = AbpSession.ToUserIdentifier();
            var receiver = new UserIdentifier(input.TenantId, input.UserId);

            try
            {
                await _chatMessageManager.SendMessageAsync(sender, receiver, input.Message, input.TenancyName, input.UserName, input.ProfilePictureId);
                return string.Empty;
            }
            catch (UserFriendlyException ex)
            {
                Logger.Warn("Could not send chat message to user: " + receiver);
                Logger.Warn(ex.ToString(), ex);
                return ex.Message;
            }
            catch (Exception ex)
            {
                Logger.Warn("Could not send chat message to user: " + receiver);
                Logger.Warn(ex.ToString(), ex);
                return _localizationManager.GetSource("AbpWeb").GetString("InternalServerError");
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (_isCallByRelease)
            {
                return;
            }
            base.Dispose(disposing);
            if (disposing)
            {
                _isCallByRelease = true;
                _windsorContainer.Release(this);
            }
        }
    }
}
#endif