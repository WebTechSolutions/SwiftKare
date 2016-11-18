using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.CustomModels
{
   public class GetPatientAllergies
    {
        public long allergiesID { get; set; }
        public long patientID { get; set; }
        public string allergyName { get; set; }
        public string severity { get; set; }
        public string reaction { get; set; }
        public Nullable<DateTime> reporteddate { get; set; }
    }
}
