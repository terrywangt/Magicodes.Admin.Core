using Abp.Notifications;
using Magicodes.Admin.Dto;

namespace Magicodes.Admin.Notifications.Dto
{
    public class GetUserNotificationsInput : PagedInputDto
    {
        public UserNotificationState? State { get; set; }
    }
}