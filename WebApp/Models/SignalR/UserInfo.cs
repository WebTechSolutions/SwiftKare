using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApp.Models.SignalR
{
    public class UserInfo
    {
        public long Id { get; set; }
        public string ConnectionId { get; set; }
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Connected { get; set; }
        public string Ip { get; set; }
        public string UserGroup { get; set; }
        public int UserID { get; set; }
        public int AdminID { get; set; }
    }
}