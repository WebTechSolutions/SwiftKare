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

        public static ConcurrentDictionary<string, UserInfo> DoctorsList = new ConcurrentDictionary<string, UserInfo>();//doctorsList
        static List<MessageInfo> MessageList = new List<MessageInfo>();

        public override Task OnDisconnected(bool trythis)
        {
            UserInfo Value;
            DoctorsList.TryRemove(Context.ConnectionId, out Value);
            MessageList.Clear();
            //MessageList.Remove(MessageList.SingleOrDefault(o => o.ConnectionId == Context.ConnectionId));
            return Clients.Group("doctors").showConnected(DoctorsList);
        }
        public override Task OnConnected()
        {
            //string username = Context.QueryString["username"].ToString();
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
            Clients.Client(message.ReceiverConnectionId).receiveMessage(message.UserName, message.Message, message.SenderConnectionId);
        }

        public void SendMessageToPatient(MessageInfo message)
        {
            var ConnectionId = Context.ConnectionId;
            message.SenderConnectionId = ConnectionId;
            message.SenderType = "Doctor";
            message.MsgDate = DateTime.Now.ToString();
            //Clients.Caller.receiveMessage(message.UserName, message.Message, ConnectionId);
            Clients.Client(message.ReceiverConnectionId).receiveMessage(message.UserName, message.Message, message.SenderConnectionId);
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
            Clients.Group("doctors").showConnected(DoctorsList);
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
            DoctorsList.TryAdd(Context.ConnectionId, data);
            Clients.Group("doctors").showConnected(DoctorsList);
        }
    }
}