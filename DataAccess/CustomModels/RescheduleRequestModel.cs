using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.CustomModels
{
    public class RescheduleRequestModel
    {
        public string userID { get; set; }
        public long appID { get; set; }
        public string appType { get; set; }
    }
}
