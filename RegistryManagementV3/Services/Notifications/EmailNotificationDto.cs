using System.Collections.Generic;

namespace RegistryManagementV3.Services.Notifications
{
    public class EmailNotificationDto
    {
        public IList<string> Emails { get; set; }
        public NotificationType NotificationType { get; set; }
        public string Content { get; set; }
    }
}