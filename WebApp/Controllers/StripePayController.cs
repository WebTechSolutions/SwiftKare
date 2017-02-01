using Stripe;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PushSharp.Apple;
using Orcus.PushNotifications;
using WebApp.Models;
using System.Web.Script.Serialization;
using System.IO;
using System.Net;
using PushNotiProject.Models;
using System.Text;

namespace WebApp.Controllers
{
    public class StripePayController : Controller
    {
        // GET: StripPay
        public ActionResult Index()
        {
            ViewBag.PublisherKey = ConfigurationManager.AppSettings["StripePayPublisherKey"].ToString();
            ViewBag.Amount = 2000;
            return View();
        }

        [HttpPost]
        public string ProceedWithPay(string tokenId)
        {
            var isSuceed =  Helper.StripePayHelper.PerformStripeCharge(tokenId, 2000);
            return isSuceed ? "succeed" : "failed";
        }

        protected void SendPushMessage(String deviceId, String pushTitle, String pushMessage, String platform, String AndroidpushSubTitle = "")
        {
            switch (platform)
            {
                case "a":
                    callAndriodPushNotification(deviceId, pushTitle, AndroidpushSubTitle, pushMessage);
                    break;
                case "i":
                    callIOSPushNotification(deviceId, "high", pushTitle, 1, "default", pushMessage);
                    break;
            }
        }

        private static void callIOSPushNotification(String deviceIds, String Priority, String Title, int Badge, String Sound, String message)
        {
            String[] devicesArray = deviceIds.Split(new char[] { ',' });

            if (!String.IsNullOrEmpty(Priority))
            {
                Priority = Priority.ToLower();

                if (Priority.Substring(0, 1) == "h")
                {
                    Priority = "high";
                }
                else
                {
                    Priority = "low";
                }
            }

            try
            {
                var nRepo = new APNSNotificationsRepository();

                var certificateFilePath = ConfigurationManager.AppSettings["certificatePath"].ToString();
                var certificatePassword = ConfigurationManager.AppSettings["certificatePassword"].ToString();

                var certificateData = System.IO.File.ReadAllBytes(certificateFilePath);

                //iterate over array of deviceIds and create array of single deviceId to send notification                
                foreach (var singleDeviceId in devicesArray)
                {
                    var localDeviceArray = new String[] { singleDeviceId };

                    nRepo.sendNotification(new PushNotiProject.Models.APNSNotification
                    {
                        ContentAvailable = true,
                        Priority = 10,
                        Aps = new PN_AppModel
                        {
                            alert = message,
                            title = Title,
                            badge = Badge,
                            sound = string.IsNullOrEmpty(Sound) ? "default" : Sound
                        }
                    }, localDeviceArray, certificateData, certificatePassword);
                }
            }
            catch (Exception exp)
            {

            }
        }

        public void callAndriodPushNotification(string deviceId, string pushTitle, string subTitle, string pushMessage)
        {
            try
            {
                string Androidmsg = "{\"Title\": \"" + pushTitle + "\",\"SubTitle\": \"" + subTitle + "\",\"Message\": \"" + pushMessage + "\", \"vibrate\": \"1\", \"sound\": \"1\" , \"largeIcon\": \"large_icon\" , \"smallIcon\": \"small_icon\"}";
                //config base
                //string applicationID = "AAAAAoTS7yU:APA91bHCK8HtGKTsTafr-1aGkeFhWSQjrBfPtu4UXV8QyvtJa9HqebfRsDT7F4l94KMIuYk-daScTOL-2TnB8oLdBlEWYNxzyXsmBcFtxv9BnMWirKFlsXUByNtMWIjbxyY1orXgFwtu";
                //config base
                //string senderId = "10818350885";

                String applicationID = ConfigurationManager.AppSettings["andriodAppId"].ToString();
                String senderId = ConfigurationManager.AppSettings["andriodSenderId"].ToString();

                WebRequest tRequest = WebRequest.Create("https://fcm.googleapis.com/fcm/send");
                tRequest.Method = "post";
                tRequest.ContentType = "application/json";
                var data = new
                {
                    to = deviceId,
                    notification = new
                    {
                        body = Androidmsg,
                        title = pushTitle,
                        sound = "Enabled"
                    }
                };
                var serializer = new JavaScriptSerializer();
                var json = serializer.Serialize(data);
                Byte[] byteArray = Encoding.UTF8.GetBytes(json);
                tRequest.Headers.Add(string.Format("Authorization: key={0}", applicationID));
                tRequest.Headers.Add(string.Format("Sender: id={0}", senderId));
                tRequest.ContentLength = byteArray.Length;
                using (Stream dataStream = tRequest.GetRequestStream())
                {
                    dataStream.Write(byteArray, 0, byteArray.Length);
                    using (WebResponse tResponse = tRequest.GetResponse())
                    {
                        using (Stream dataStreamResponse = tResponse.GetResponseStream())
                        {
                            using (StreamReader tReader = new StreamReader(dataStreamResponse))
                            {
                                String sResponseFromServer = tReader.ReadToEnd();
                                string str = sResponseFromServer;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                string str = ex.Message;
            }
        }

    }
    public class AndroidFCMPushNotificationStatus
    {
        public bool Successful
        {
            get;
            set;
        }

        public string Response
        {
            get;
            set;
        }
        public Exception Error
        {
            get;
            set;
        }

    }
}