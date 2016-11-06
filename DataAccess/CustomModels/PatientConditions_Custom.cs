using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.CustomModels
{
   public  class PatientConditions_Custom
    {
        public long conditionID { get; set; }
        public string conditionName { get; set; }
        public Nullable<long> patientID { get; set; }
        public string userId { get; set; }
    }
}
