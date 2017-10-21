using System;
using System.Diagnostics;
using System.Globalization;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR.Hubs;
using Microsoft.AspNet.SignalR;
using System.Collections.Concurrent;
using System.Web;
using WebApp.Helper;
using WebApp.Models;
using System.Linq;
using WebApp.Models.SignalR;
using System.Collections.Generic;


namespace WebApp.Hub
{
    public class SmartHub
    {
    }

    public class ChatHub : Microsoft.AspNet.SignalR.Hub
    {
        public static ConcurrentDictionary<string, UserInfo> AllUserList =
            new ConcurrentDictionary<string, UserInfo>();

        static List<MessageInfo> MessageList = new List<MessageInfo>();

        public override Task OnDisconnected(bool trythis)
        {
            UserInfo Value;
            AllUserList.TryRemove(Context.ConnectionId, out Value);
            MessageList.Clear();
           // List<string> grp = new List<string>();
            IList<string> grp = new List<string>();
           // grp.Add("patients");
           // grp.Add("doctors");
            GetUsersPatient();
            GetUsersDoctor();
            return Clients.Group("NoGroup").showConnected(AllUserList);
        }

        public override Task OnConnected()
        {
            string clientId = Context.ConnectionId;
            string data = clientId;
           // Clients.Caller.receiveMessage("ChatHub", data, 0);
           // Clients.Client(clientId).receiveMessage("ChatHub", clientId, 0);
            return base.OnConnected();
        }


        public void SendMessageToDoctor(MessageInfo message)
        {
            var ConnectionId = Context.ConnectionId;
            message.SenderConnectionId = ConnectionId;
            message.SenderType = "Patient";
            message.MsgDate = DateTime.Now.ToString();
            //MessageList.Add(message);
            //Clients.Caller.receiveMessage(message.UserName, message.Message, ConnectionId);
            Clients.Client(message.ReceiverConnectionId).receiveMessage(message.UserName, message.Message, message.SenderConnectionId, message.SenderId);
            //SendPush
            //ApiConsumerHelper.GetResponseString("api/Account/SendPush1?docID="+message.ReceiverId+"&patID="+message.SenderId+"&sendtoPatient="+false+"&sendtoDoctor="+true);
            // ApiConsumerHelper.GetResponseString("api/Account/SendPush1?docID=" + message.ReceiverId );
            //   WebApp.Controllers.AppointmentController ac = new Controllers.AppointmentController();
            // ac.SendPush(message.ReceiverId);
            // System.Net.WebRequest request = System.Net.WebRequest.Create("http://localhost:13040/api/SendPush?docID=27");

            if (message.Message.ToString().Contains("call"))
            {
                using (var wb = new System.Net.WebClient())
                {
                    var data = new System.Collections.Specialized.NameValueCollection();
                    data["abc"] = "";

                    var response = wb.UploadValues("http://localhost:13040/api/Account/SendPush?docID=" + message.ReceiverId + "&patID=" + message.SenderId + "&sendtoPatient=" + false + "&sendtoDoctor=" + true + "&message=Incoming Consultation Call.", "POST", data);
                }
            }
            //, message.SenderId, false, true
            // (long docID, long patID, bool senttoPatient, bool sendtoDoctor
        }

        public void SendMessageToPatient(MessageInfo message)
        {
            var ConnectionId = Context.ConnectionId;
            message.SenderConnectionId = ConnectionId;
            message.SenderType = "Doctor";
            message.MsgDate = DateTime.Now.ToString();
            //Clients.Caller.receiveMessage(message.UserName, message.Message, ConnectionId);
            Clients.Client(message.ReceiverConnectionId).receiveMessage(message.UserName, message.Message, message.SenderConnectionId, message.SenderId);
            if (message.Message.ToString().Contains("consultID"))
            {
                using (var wb = new System.Net.WebClient())
                {
                    var data = new System.Collections.Specialized.NameValueCollection();
                    data["abc"] = "";

                    var response = wb.UploadValues("http://localhost:13040/api/Account/SendPush?docID=" + message.SenderId + "&patID=" + message.ReceiverId + "&sendtoPatient=" + true + "&sendtoDoctor=" + false + "&message=Incoming Consultation Call.", "POST", data);
                }
            }
        }

        public void CallPatient(MessageInfo message) {
            var ConnectionId = Context.ConnectionId;
            message.SenderConnectionId = ConnectionId;
            message.SenderType = "Doctor";
            message.MsgDate = DateTime.Now.ToString();
            //Clients.Caller.receiveMessage(message.UserName, message.Message, ConnectionId);
            Clients.Client(message.ReceiverConnectionId).incomingCallFromDoctor(message.UserName, message.Message, message.SenderConnectionId, message.SenderId, message.consultID, message.DoctorName);
        }

        public void PatientAcceptedCall(MessageInfo message)
        {
            var ConnectionId = Context.ConnectionId;
            Clients.Client(message.ReceiverConnectionId).patientAcceptedCall(message.PatientName, message.SenderId, message.consultID, message.UserName);
        }

        public void PatientRejectedCall(MessageInfo message)
        {
            var ConnectionId = Context.ConnectionId;
            Clients.Client(message.ReceiverConnectionId).patientRejectedCall(message.PatientName, message.SenderId, message.consultID,message.UserName);
        }

        public void PatientRedirectedToCall(string connId, string tokboxInfo, string patId, string patientName) {
            Clients.Client(connId).patientRedirectedToCallHandle(tokboxInfo, patId, patientName);
        }

        public void SendMessage(MessageInfo message)
        {
            var ConnectionId = Context.ConnectionId;
            message.SenderConnectionId = ConnectionId;
            message.MsgDate = DateTime.Now.ToString();
            //Clients.Caller.receiveMessage(message.UserName, message.Message, ConnectionId);
            Clients.Client(message.ReceiverConnectionId).receiveMessage(message.UserName, message.Message, message.SenderConnectionId);
        }

        public void AdminJoin()//patient
        {
            Groups.Add(Context.ConnectionId, "patients");
        }

        public void AdminJoinDoctor()//doctor
        {
            Groups.Add(Context.ConnectionId, "doctors");
        }

        public void GetUsers()
        {
            Clients.Group("doctors").showConnected(AllUserList);
        }

        public void GetUsersDoctor()
        {
            var oRet = AllUserList.Where(x => x.Value.UserType.ToLower() == "doctor");
            Clients.Group("patients").showConnected(oRet);
        }

        public void GetUsersPatient()
        {
            var oRet = AllUserList.Where(x => x.Value.UserType.ToLower() == "patient");
            Clients.Group("doctors").showConnected(oRet);
        }


        public void Join(UserInfo data)
        {
            string conn= Context.ConnectionId;
            data.Connected = DateTime.Now.ToString("f");
            data.ConnectionId = conn;
            data.UserGroup = "doctors";
            

            data.Ip = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
            AllUserList.TryAdd(conn, data);
           
            if (data.UserType.ToString().Trim().ToLower().Equals("doctor"))
                Groups.Add(conn, "doctors");
            else
                Groups.Add(conn, "patients");
            
        }
        public void Disconnect(string connectionId)
        {
            UserInfo Value;
            AllUserList.TryRemove(connectionId,out Value);
            GetUsersPatient();
            GetUsersDoctor();
        }

        public UserInfo TestJoin()
        {
            UserInfo data = new UserInfo();
            data.Connected = DateTime.Now.ToString("f");
            data.ConnectionId = Context.ConnectionId;
            data.UserGroup = "doctors";
            data.UserName = "test@test.com";
            data.UserType = "Doctor";
            data.LastName = "LastNAme JamTest ";
            data.FirstName = "FirstNAmt JamTest";
            data.Id = 12345;
            data.UserGroup = "doctors";
            //data.Sid = SessionHandler.UserInfo.Id;
            //data.UserName = SessionHandler.UserInfo.Email;
            //data.FirstName = SessionHandler.UserInfo.FirstName;
            //data.LastName = SessionHandler.UserInfo.LastName;

            data.Ip = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
            AllUserList.TryAdd(Context.ConnectionId, data);
            Clients.Group("doctors").showConnected(AllUserList);
            return data;
        }
    }
}