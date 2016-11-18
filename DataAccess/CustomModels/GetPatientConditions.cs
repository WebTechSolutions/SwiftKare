using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.CustomModels
{
    public class GetPatientConditions
    {
        public long conditionID { get; set; }
        public string conditionName { get; set; }
        public Nullable<long> patientID { get; set; }
        public Nullable<System.DateTime> reportedDate { get; set; }
    }
}
