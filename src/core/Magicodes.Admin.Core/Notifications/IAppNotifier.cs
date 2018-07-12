using System.Threading.Tasks;
using Abp;
using Abp.Notifications;
using Magicodes.Admin.Authorization.Users;
using Magicodes.Admin.MultiTenancy;

namespace Magicodes.Admin.Notifications
{
    public interface IAppNotifier
    {
        Task WelcomeToTheApplicationAsync(User user);

        Task NewUserRegisteredAsync(User user);

        Task NewTenantRegisteredAsync(Tenant tenant);

        Task SendMessageAsync(UserIdentifier user, string message, NotificationSeverity severity = NotificationSeverity.Info);
    }
}
