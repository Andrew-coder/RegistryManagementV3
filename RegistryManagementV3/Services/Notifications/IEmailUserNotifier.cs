using System.Threading.Tasks;

namespace RegistryManagementV3.Services.Notifications
{
    public interface IEmailUserNotifier
    {
        Task NotifyAsync(EmailNotificationDto smsNotification);
    }
}