using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using PushSharp.Apple;
using System.Configuration;
using Orcus.PushNotifications;

namespace PushNotiProject.Models
{
    public class APNSNotificationsRepository
    {
        public void sendNotification(APNSNotification model, String[] devicesIds,  byte[] certificate, String password)
        {
            IIOSPushNotificationService pushNotificationService = new IOSPushNotificationService();

            pushNotificationService.Configure(ApnsConfiguration.ApnsServerEnvironment.Production, certificate, password);

            pushNotificationService.OnNofificationFailed += (notification, exception) =>
            {
                //handle failing notification here
            };

            pushNotificationService.OnFeedbackReceived += (token, timestamp) =>
            {
                // token is not valid anymore
                // remove device token from your database
                // timestamp is the time the token was reported as expired                
            };

            pushNotificationService.Send(devicesIds, model);
        }
    }
}