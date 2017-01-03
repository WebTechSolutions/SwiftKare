using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.CustomModels
{
    public class CreateConsultModel
    {
        public string userID { get; set; }
        public long appID { get; set; }
        public string sessionID { get; set; }
        public string token { get; set; }

        public long doctorId { get; set; }
        public long patientId { get; set; }
    }

    public class ConsultROSModel
    {
        public string userID { get; set; }
        public long consultID { get; set; }
        public string sysitemname { get; set; }
        public long sysitemid { get; set; }
    }
}
