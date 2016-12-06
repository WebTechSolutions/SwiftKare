using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.CustomModels
{
    public class MessageListModel
    {
        public long messageID { get; set; }
        public string subject { get; set; }
        public string fromName { get; set; }
        
        public Nullable<System.DateTime> sentTime { get; set; }
        public Nullable<bool> hasAttachment { get; set; }

        public Nullable<bool> isRead { get; set; }
    }
}
