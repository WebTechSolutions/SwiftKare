using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.CustomModels
{
         public partial class GetMessageModel
        {
        public GetMessageModel()
        {
            this.msgFiles = new HashSet<MessageFile>();
        }
            public long msgID { get; set; }
            public string message { get; set; }
            public string @from { get; set; }
            public string to { get; set; }
            public string status { get; set; }
            public Nullable<bool> isRead { get; set; }
            public Nullable<long> replyLink { get; set; }
            public virtual ICollection<MessageFile> msgFiles { get; set; }

    }
    }
