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

        public string subject
        {
            get
            {
                return this.message;
            }
        }
        public string message { get; set; }
        public string @from { get; set; }
        public string to { get; set; }
        public string status { get; set; }
        public Nullable<bool> isRead { get; set; }
        public Nullable<long> replyLink { get; set; }

        public string fromName { get { return this.from; } }
        public Nullable<bool> hasAttachment { get { return (msgFiles != null && msgFiles.Count > 0); } }
        public Nullable<bool> isMarkedFeatured { get { return true; } }

        public DateTime sendTimeUtc { get { return DateTime.UtcNow; } }

        public virtual ICollection<MessageFile> msgFiles { get; set; }
    }
}
