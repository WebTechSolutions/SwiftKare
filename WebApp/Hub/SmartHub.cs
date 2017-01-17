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

            return Clients.Group("doctors").showConnected(AllUserList);
        }

        public override Task OnConnected()
        {
            string clientId = Context.ConnectionId;
            string data = clientId;
            Clients.Caller.receiveMessage("ChatHub", data, 0);
            Clients.Client(clientId).receiveMessage("ChatHub", clientId, 0);
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
        }

        public void SendMessageToPatient(MessageInfo message)
        {
            var ConnectionId = Context.ConnectionId;
            message.SenderConnectionId = ConnectionId;
            message.SenderType = "Doctor";
            message.MsgDate = DateTime.Now.ToString();
            //Clients.Caller.receiveMessage(message.UserName, message.Message, ConnectionId);
            Clients.Client(message.ReceiverConnectionId).receiveMessage(message.UserName, message.Message, message.SenderConnectionId, message.SenderId);
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
            Clients.Client(message.ReceiverConnectionId).patientAcceptedCall(message.UserName, message.SenderId, message.AppId);
        }

        public void PatientRejectedCall(MessageInfo message)
        {
            var ConnectionId = Context.ConnectionId;
            Clients.Client(message.ReceiverConnectionId).patientRejectedCall(message.UserName, message.SenderId, message.AppId);
        }

        public void PatientRedirectedToCall(string connId, string consultId, string patId, string patientName) {
            Clients.Client(connId).patientRedirectedToCallHandle(consultId, patId, patientName);
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
            Groups.Add(Context.ConnectionId, "doctors");
        }

        public void GetUsers()
        {
            Clients.Group("doctors").showConnected(AllUserList);
        }

        public void GetUsersDoctor()
        {
            var oRet = AllUserList.Where(x => x.Value.UserType == "Doctor");
            Clients.Group("doctors").showConnected(oRet);
        }

        public void GetUsersPatient()
        {
            var oRet = AllUserList.Where(x => x.Value.UserType == "Patient");
            Clients.Group("doctors").showConnected(oRet);
        }


        public void Join(UserInfo data)
        {
            data.Connected = DateTime.Now.ToString("f");
            data.ConnectionId = Context.ConnectionId;
            data.UserGroup = "doctors";


            //data.Sid = SessionHandler.UserInfo.Id;
            //data.UserName = SessionHandler.UserInfo.Email;
            //data.FirstName = SessionHandler.UserInfo.FirstName;
            //data.LastName = SessionHandler.UserInfo.LastName;

            data.Ip = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
            AllUserList.TryAdd(Context.ConnectionId, data);
            Clients.Group("doctors").showConnected(AllUserList);
        }
    }
}