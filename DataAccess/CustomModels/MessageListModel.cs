using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.CustomModels
{
    public class MessageListModel
    {
        public Nullable<long> messageID { get; set; }
        public string @from { get; set; }
        public Nullable<long> replyLink { get; set; }
        public string senderName { get; set; }
        public Nullable<bool> hasAttachment { get; set; }
        public Nullable<bool> isRead { get; set; }
        public Nullable<System.DateTime> sentTime { get; set; }
        public string subject { get; set; }

      
    }
}
