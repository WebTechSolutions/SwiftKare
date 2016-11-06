using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.CustomModels
{
  public  class PatientPharmacy_Custom
    {
        public long patientID { get; set; }
        public string pharmacy { get; set; }
        public Nullable<long> pharmacyid { get; set; }
        public string userId { get; set; }
    }
}
