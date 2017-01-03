using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.CustomModels
{
    public class AlertModel
    {
        public long alertID { get; set; }
        public string alertText { get; set; }
        public DateTime? alertDate { get; set; }
        public bool? isRead { get; set; }
    }
    public class DeleteAlertModel
    {
        public long alertID { get; set; }
        public string userID { get; set; }
       
    }
}
