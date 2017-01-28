using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.CustomModels
{
    public class MessageCustomModel
    {
        public string message { get; set; }
        public string @from { get; set; }
        public string to { get; set; }
        public Nullable<long> replyLink { get; set; }
        public string subject { get; set; }
        public MessageFileModel[] msgFile { get; set; }

    }
    public class HelpTicket
    {
        public string message { get; set; }
        public string sender { get; set; }
        public string reciever { get; set; }
        public string subject { get; set; }
        public string attachmentPath { get; set; }

    }
}
