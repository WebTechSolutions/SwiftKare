using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.CustomModels
{
    public partial class GetMessageModel
    {

        public long msgID { get; set; }
        public string message { get; set; }
        public string subject { get; set; }
        public string @from { get; set; }
        public string senderName { get; set; }
        public Nullable<DateTime> sentTime { get; set; }
        public List<MessageFileModel> msgFiles { get; set; }

        public bool hasAttachment
        {
            get
            {
                return (msgFiles != null && msgFiles.Count > 0);
            }
        }

    }
}
