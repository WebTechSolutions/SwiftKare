using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.CustomModels
{
    public class LiveReqLogModel
    {
        public Nullable<long> patientID { get; set; }
        public Nullable<long> doctorID { get; set; }
        public string message { get; set; }
        public string From { get; set; }
    }
}
