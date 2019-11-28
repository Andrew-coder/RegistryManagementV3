using System.Collections.Generic;

namespace RegistryManagementV3.Services.Notifications
{
    public class UserNotificationDto
    {
        public IList<string> PhoneNumbers { get; set; }
        public NotificationType NotificationType { get; set; }
        public string Content { get; set; }
        public string Protocol { get; set; }
    }
}
