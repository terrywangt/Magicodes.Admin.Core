using System.Threading.Tasks;
using Abp.Net.Mail;
using Magicodes.Admin.Configuration.Host.Dto;

namespace Magicodes.Admin.Configuration
{
    public abstract class SettingsAppServiceBase : AdminAppServiceBase
    {
        private readonly IEmailSender _emailSender;

        protected SettingsAppServiceBase(
            IEmailSender emailSender)
        {
            _emailSender = emailSender;
        }

        #region Send Test Email

        public async Task SendTestEmail(SendTestEmailInput input)
        {
            await _emailSender.SendAsync(
                input.EmailAddress,
                L("TestEmail_Subject"),
                L("TestEmail_Body")
            );
        }

        #endregion
    }
}
