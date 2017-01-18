using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApp.Models.SignalR
{
    public class MessageInfo
    {
        public string UserName { get; set; }
        public long SenderId { get; set; }
        public long ReceiverId { get; set; }

        public string PatientName { get; set; }
        public string DoctorName { get; set; }

        public string SenderConnectionId { get; set; }   //hub context connectionId
        public string ReceiverConnectionId { get; set; } //hub-context connectionId

        public string SenderType { get; set; }
        public string Message { get; set; }
        public string MsgDate { get; set; }

        public string consultID { get; set; }
       
    }
}