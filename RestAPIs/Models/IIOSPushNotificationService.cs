using PushSharp.Apple;
using PushSharp.Core;

namespace Orcus.PushNotifications
{
    public interface IIOSPushNotificationService : IPushNotificationService
    {
        event NotificationFailureDelegate<ApnsNotification> OnNofificationFailed;
        event FeedbackService.FeedbackReceivedDelegate OnFeedbackReceived;
        void Configure(ApnsConfiguration.ApnsServerEnvironment environment, string p12FilePaht, string p12FilePassword);
        void Configure(ApnsConfiguration.ApnsServerEnvironment environment, byte [] certificate, string p12FilePassword);
    }
}