using System.Threading.Tasks;

namespace RegistryManagementV3.Services.Notifications
{
    public interface IUserNotifier
    {
        Task NotifyAsync(UserNotificationDto userNotification);
    }
}