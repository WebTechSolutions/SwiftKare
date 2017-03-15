using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.CustomModels
{
    public class PatientFamilyHX_Custom
    {
        public Nullable<long> patientID { get; set; }
        public string name { get; set; }
        public string relationship { get; set; }
    }
    public class GetPatientFamilyHX
    {
        public Nullable<long> fhxID { get; set; }
        public Nullable<long> patientID { get; set; }
        public string name { get; set; }
        public string relationship { get; set; }
        public long familyHXItemID { get; set; }
    }
}
