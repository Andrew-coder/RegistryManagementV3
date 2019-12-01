using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;

namespace RegistryManagementV3.Services.Notifications
{
    public class AwsEmailUserNotifier : IEmailUserNotifier
    {
        
        private const string AwsNotificationProtocolName = "Email";
        private readonly IAmazonSimpleNotificationService _snsClient;
        
        private readonly IDictionary<NotificationType, string> _topicNames = new Dictionary<NotificationType, string>
        {
            { NotificationType.RmRestorePassword, "rm_restore_password" }
        };

        public AwsEmailUserNotifier(IAmazonSimpleNotificationService snsClient)
        {
            _snsClient = snsClient;
        }

        public async Task NotifyAsync(EmailNotificationDto emailNotification)
        {
            await CreateSnsTopic(_topicNames[emailNotification.NotificationType])
                .ContinueWith(topicResponse => PublishNotificationToCreatedTopic(topicResponse.Result, emailNotification));
        }
        
        private Task<CreateTopicResponse> CreateSnsTopic(string topicName)
        {
            var createTopicRequest = new CreateTopicRequest(topicName);
            return _snsClient.CreateTopicAsync(createTopicRequest);
        }

        private Task PublishNotificationToCreatedTopic(CreateTopicResponse topicResponse,
            EmailNotificationDto emailNotification)
        {
            return SubscribeToTopic(topicResponse, AwsNotificationProtocolName,
                    emailNotification.Emails)
                .ContinueWith(response => PublishMessageToTopic(emailNotification.Content, topicResponse.TopicArn))
                .ContinueWith(response => DeleteTopic(topicResponse.TopicArn));
        }
        
        private Task<ConfirmSubscriptionResponse[]> SubscribeToTopic(CreateTopicResponse topicResponse, string protocol,
            IEnumerable<string> endpoints)
        {
            var subscriptionConfirmationResponses = endpoints
                .Select(endpoint => new SubscribeRequest(topicResponse.TopicArn, protocol, endpoint))
                .Select(subscribeRequest =>  _snsClient.SubscribeAsync(subscribeRequest)
                    .ContinueWith(subscribeResponse => _snsClient.ConfirmSubscriptionAsync(topicResponse.TopicArn, subscribeResponse.Result.SubscriptionArn)).Result);
            return Task.WhenAll(subscriptionConfirmationResponses.ToArray());
        }

        private Task PublishMessageToTopic(string content, string topicArn)
        {
            var pubRequest = new PublishRequest {Message = content, TopicArn = topicArn};
            return _snsClient.PublishAsync(pubRequest);
        }
        
        private Task DeleteTopic(string topicArn)
        {
            return _snsClient.DeleteTopicAsync(topicArn);
        }
    }
}