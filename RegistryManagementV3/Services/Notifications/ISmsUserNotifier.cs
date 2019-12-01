using System.Threading.Tasks;

namespace RegistryManagementV3.Services.Notifications
{
    public interface ISmsUserNotifier
    {
        Task NotifyAsync(SmsNotificationDto smsNotification);
    }
}