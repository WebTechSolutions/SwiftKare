using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SendGrid;
using System.Net.Mail;
using System.Net;
using System.Threading.Tasks;
using System.Configuration;
using PushSharp.Apple;
using Orcus.PushNotifications;
using PushNotiProject.Models;
using System.Web.Script.Serialization;
using System.IO;
using System.Text;
using DataAccess;

namespace RestAPIs.Helper
{
    
    public class EmailHelper
    {
        #region Declarations

        public string toEmailAddress { get; set; }
        public string bccEmailAddress { get; set; }
        public string fromEmailAddress { get; set; }
        public string fromUserName { get; set; }
        public string subject { get; set; }
        public string message { get; set; }
        public string attachmentPath { get; set; }

        private string sendGridUserName = ConfigurationManager.AppSettings["SendGridUserName"].ToString();
        private string sendGridPassword = ConfigurationManager.AppSettings["SendGridPassword"].ToString();
        #endregion

        #region Constructors

        public EmailHelper()
        {
            fromEmailAddress = ConfigurationManager.AppSettings["SendGridFromEmailAddress"].ToString();
            fromUserName = ConfigurationManager.AppSettings["SendGridFromName"].ToString();
        }

        public EmailHelper(string toEmailAddress, string subject, string message) : this()
        {
            this.toEmailAddress = toEmailAddress;
            this.subject = subject;
            this.message = message;
        }

        public EmailHelper(string fromEmailAddress,string toEmailAddress, string bccEmailAddress, string subject, string message)
            : this(toEmailAddress, subject, message)
        {
          this.bccEmailAddress = bccEmailAddress;
          this.fromEmailAddress = fromEmailAddress;
        }
        //public EmailHelper(string toEmailAddress, string subject, string message, string attachmentPath)
        //    : this(toEmailAddress, subject, message)
        //{
        //    this.attachmentPath = attachmentPath;
        //}

        #endregion

        #region Functions

        public void SendMessage()
        {
            SendGridMessage myMessage = new SendGridMessage();
            myMessage.AddTo(toEmailAddress);
            myMessage.From = new MailAddress(fromEmailAddress, (string.IsNullOrEmpty(fromUserName) ? fromEmailAddress : fromUserName));

            if (!string.IsNullOrEmpty(bccEmailAddress))
            {
                myMessage.AddBcc(bccEmailAddress);
            }

            myMessage.Subject = subject;
            myMessage.Html = message;

            if (!string.IsNullOrEmpty(attachmentPath))
            {
                myMessage.AddAttachment(attachmentPath);
            }

            var credentials = new NetworkCredential(sendGridUserName, sendGridPassword);

            // Create an Web transport for sending email.
            var transportWeb = new Web(credentials);

            // Send the email, which returns an awaitable task.
            var oRet = transportWeb.DeliverAsync(myMessage);
        }

        #endregion
    }
    public class pushModel
    {
        public Nullable<long> doctorID { get; set; }
        public Nullable<long> patientID { get; set; }
        public bool sendtoPatient { get; set; }
        public bool sendtoDoctor { get; set; }
        public string PPushTitle { get; set; }
        public string DPushTitle { get; set; }
        public string PPushMessage { get; set; }
        public string DPushMessage { get; set; }
        

    }
    public class PushHelper
    {
        private SwiftKareDBEntities db = new SwiftKareDBEntities();
        //pushNotificationHelper.SendPushNotification(diOSToken,dAndroidToken,piOSToken,pAndroidToken,"Push Title","Push Message",doctorID,patientID);

        public void sendPush(pushModel pm)
        {
            var Ptokens = (from p in db.Patients
                         where p.patientID == pm.patientID
                         select new { PiOSToken = p.iOSToken, PandroidToken = p.AndroidToken }).FirstOrDefault();
            var Dtokens = (from p in db.Doctors
                           where p.doctorID == pm.doctorID
                           select new { DiOSToken = p.iOSToken, DandroidToken = p.AndroidToken }).FirstOrDefault();
            string PiOSToken = "";
            string PandroidToken = "";
            string DiOSToken = "";
            string DandroidToken = "";
            if (Ptokens.PiOSToken != null) {  PiOSToken = Ptokens.PiOSToken; }
            if (Ptokens.PandroidToken != null) {  PandroidToken = Ptokens.PandroidToken; }
            if (Ptokens.PiOSToken != null)  DiOSToken = Dtokens.DiOSToken;
            if (Ptokens.PiOSToken != null)  DandroidToken = Dtokens.DandroidToken;

            if (pm.sendtoPatient)
            {
                if (PiOSToken != "" || PiOSToken != null) SendPushMessage(PiOSToken, pm.PPushTitle, pm.PPushMessage, "i");
                if (PandroidToken != "" || PandroidToken != null) SendPushMessage(PandroidToken, pm.PPushTitle, pm.PPushTitle, "a");
            }

            if (pm.sendtoDoctor)
            {
                if (DiOSToken != "" || DiOSToken != "null") SendPushMessage(DiOSToken, pm.DPushTitle, pm.DPushMessage, "i");
                if (DandroidToken != "" || DandroidToken!="null") SendPushMessage(DandroidToken, pm.DPushTitle, pm.DPushMessage, "a");
            }
        }
        private void SendPushMessage(String deviceId, String pushTitle, String pushMessage, String platform, String AndroidpushSubTitle = "")
        {
            if (deviceId != null)
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
              //  string Androidmsg = "{\"Title\": \"" + pushTitle + "\",\"SubTitle\": \"" + subTitle + "\",\"Message\": \"" + pushMessage + "\", \"vibrate\": \"1\", \"sound\": \"1\" , \"largeIcon\": \"large_icon\" , \"smallIcon\": \"small_icon\"}";
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
                        body = pushMessage,
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