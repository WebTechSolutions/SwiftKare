using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PushSharp.Apple;
using PushSharp.Core;



namespace Orcus.PushNotifications
{
    public class IOSPushNotificationService : IIOSPushNotificationService
    {
        private ApnsConfiguration _config;

        public void Send(string[] deviceTokens, object payload)
        {
            if (_config == null)
            {
                throw new NullReferenceException("Not configured");
            }

            // instantiating services and configuring them
            var broker = new ApnsServiceBroker(_config);
            var feedbackService = new FeedbackService(_config);
            broker.OnNotificationFailed += NofificationFailed;
            feedbackService.FeedbackReceived += FeedbackReceived;

            // start sending notifications
            broker.Start();
            foreach (var token in deviceTokens)
            {
                broker.QueueNotification(new ApnsNotification
                {
                    DeviceToken = token,
                    Payload = JObject.FromObject(payload)
                });
            }

            broker.Stop();

            // check feedback service for expired tokens
            feedbackService.Check();
        }

        public void Configure(ApnsConfiguration.ApnsServerEnvironment environment, string p12FilePaht,
            string p12FilePassword)
        {
            _config = new ApnsConfiguration(environment, p12FilePaht, p12FilePassword);
        }
        public void Configure(PushSharp.Apple.ApnsConfiguration.ApnsServerEnvironment environment, byte[] certificate, string p12FilePassword)
        {
            _config = new ApnsConfiguration(environment, certificate, p12FilePassword);
        }

        public event NotificationFailureDelegate<ApnsNotification> OnNofificationFailed;

        public event FeedbackService.FeedbackReceivedDelegate OnFeedbackReceived;

        protected virtual void NofificationFailed(ApnsNotification notification, AggregateException exception)
        {
            if (OnNofificationFailed != null)
            {
                OnNofificationFailed.Invoke(notification, exception);
            }
        }

        protected virtual void FeedbackReceived(string devicetoken, DateTime timestamp)
        {
            if (OnFeedbackReceived != null)
            {
                OnFeedbackReceived.Invoke(devicetoken, timestamp);
            }
        }
    }
}