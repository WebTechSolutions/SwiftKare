namespace Orcus.PushNotifications
{
    public interface IPushNotificationService
    {
        void Send(string[] deviceTokens, object payload);
    }
}