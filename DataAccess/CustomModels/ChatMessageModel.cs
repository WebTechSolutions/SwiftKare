using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.CustomModels
{
   public class ChatMessageModel
    {
        public string sender { get; set; }
        public string reciever { get; set; }
        public string message { get; set; }
        public long consultID { get; set; }
    }
}
