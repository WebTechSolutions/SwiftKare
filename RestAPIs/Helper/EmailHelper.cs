using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SendGrid;
using System.Net.Mail;
using System.Net;
using System.Threading.Tasks;
using System.Configuration;

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

        public EmailHelper(string toEmailAddress, string subject, string message, string attachmentPath)
            : this(toEmailAddress, subject, message)
        {
            this.attachmentPath = attachmentPath;
        }

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
}